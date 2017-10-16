using System;
using System.Collections;

using System.ComponentModel;
using System.IO;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

using HutongGames.PlayMakerEditorUtils;

namespace HutongGames.PlayMakerEditor
{

	public class ProjectToolsTest
	{
		// Change MenuRoot to move the Playmaker Menu
		// E.g., MenuRoot = "Plugins/PlayMaker/"
		public const string MenuRoot = "PlayMaker/Addons/";
		

		static UIToolsFeedbackBridge feedback = new UIToolsFeedbackBridge() ;

		/*
		static IEnumerator DoSerializeAllScenes()
		{
			AbortSerializingProcessFlag = false;
			SerializingProcessActive = true;
			LoadLevelContentXml();
			yield return null;
			EditorCoroutine f1e = EditorCoroutine.startManual(PerformSerializeAllScenes());
			while (f1e.routine.MoveNext()) {
				yield return f1e.routine.Current;
			}
			yield return null ;
			SaveLevelContentxml();
			_chapterIdFilter = null;
			SerializingProcessActive = false;
			
		}
	*/


		#region Debug Flow
		[MenuItem(MenuRoot + "Tools/Turn Off All DebugFlow in Build", false,64)]
		public static void TurnOffAllDebugFlow()
		{
		//	ProjectToolsUI.Init();
			//ProjectToolsUI.Instance.DoTurnOffAllDebugFlow();
			EditorCoroutine.start(DoTurnOffAllDebugFlow());

		}

		public static IEnumerator DoTurnOffAllDebugFlow()
		{
			if (!EditorApplication.SaveCurrentSceneIfUserWantsTo ()) {
				yield break;
			}

			string OpenedScene = EditorApplication.currentScene;

			feedback.StartProcedure("Load Prefabs and Launch Routine",true);

			EditorCoroutine _LoadPrefabsWithPlayMakerFSMComponents_cr = EditorCoroutine.startManual(LoadPrefabsWithPlayMakerFSMComponents());
			while (_LoadPrefabsWithPlayMakerFSMComponents_cr.routine.MoveNext()) {
				yield return _LoadPrefabsWithPlayMakerFSMComponents_cr.routine.Current;
			}
		
			yield return null;

			EditorCoroutine _saveAllFsminBuild_cr = EditorCoroutine.startManual(TurnOffAllDebugFlowInBuild());
			while (_saveAllFsminBuild_cr.routine.MoveNext()) {
				yield return _saveAllFsminBuild_cr.routine.Current;
			}

			yield return null;

			if (OpenedScene != EditorApplication.currentScene) {
				EditorApplication.OpenScene(OpenedScene);
			}

			// could trigger a Dialog in the ui for the user to acknowledge it's done.
			feedback.EndProcedure("Load Prefabs and Launch Routine",true);
		}

		private static IEnumerator TurnOffAllDebugFlowInBuild()
		{
			feedback.StartProcedure("Turn Off All FSMs DebugFlow In Build");
			foreach (var scene in EditorBuildSettings.scenes)
			{
				feedback.LogAction("Open Scene: " + scene.path,true);
			
				bool _ok = false;
				try{

					EditorApplication.OpenScene(scene.path);
					FsmEditor.RebuildFsmList();
					_ok = true;
				}catch(Exception e)
				{
					Debug.LogWarning("error : "+e.Message);
				}

				if (_ok)
				{
					feedback.LogAction("Will TurnOffDebugFlowInLoadedFSMs");

					EditorCoroutine _TurnOffDebugFlowInLoadedFSMs_cr = EditorCoroutine.startManual(TurnOffDebugFlowInLoadedFSMs());
					while (_TurnOffDebugFlowInLoadedFSMs_cr.routine.MoveNext()) {
						yield return _TurnOffDebugFlowInLoadedFSMs_cr.routine.Current;
					}

					EditorApplication.SaveScene();
				}
			
				yield return null;
			}
			
			feedback.EndProcedure("Turn Off All FSMs DebugFlow In Build");
		}


		private static IEnumerator TurnOffDebugFlowInLoadedFSMs()
		{
			feedback.StartProcedure("Turn Off DebugFlow In Loaded FSMs");
			
			feedback.LogAction("Rebuild Fsm List");
			
			FsmEditor.RebuildFsmList();
			
			yield return null;
			
			foreach (var fsm in FsmEditor.FsmList)
			{
				feedback.LogAction("Checking DebugFlow for "+fsm.GameObjectName+"/"+fsm.Name);

				// check if we deal with a prefab and wether we need to change this or not
				bool isPrefabOriginal = PrefabUtility.GetPrefabParent(fsm.GameObject) == null && PrefabUtility.GetPrefabObject(fsm.GameObject) != null;
				bool isPrefabInstance = PrefabUtility.GetPrefabParent(fsm.GameObject) != null && PrefabUtility.GetPrefabObject(fsm.GameObject) != null;
				bool isDisconnectedPrefabInstance = PrefabUtility.GetPrefabParent(fsm.GameObject) != null && PrefabUtility.GetPrefabObject(fsm.GameObject) == null;

				feedback.LogAction(""+fsm.GameObjectName+" isPrefabOriginal"+isPrefabOriginal+" isPrefabInstance="+isPrefabInstance +" isDisconnectedPrefabInstance=" +isDisconnectedPrefabInstance,true);
				if (isPrefabOriginal || ! isPrefabInstance || isDisconnectedPrefabInstance)
				{
					if (fsm.EnableDebugFlow)
					{
						feedback.LogAction("Turning off DebugFlow for "+fsm.GameObjectName+"/"+fsm.Name,true);

						fsm.EnableDebugFlow = false;
					}
				}

				yield return null;
			}
			
			feedback.EndProcedure("Turn Off DebugFlow In Loaded FSMs");
		}

		#endregion Debug Flow

		#region Saving

		#if ECOSYSTEM_TOOLS_BETA

		//[MenuItem(MenuRoot + "Tools/Re-Save All Loaded FSMs", false, 31)]
		public static void ReSaveAllLoadedFSMs()
		{
			EditorCoroutine.start(DoReSaveAllLoadedFSMs());
		}

		[MenuItem(MenuRoot + "Tools/Re-Save All FSMs in Build", false, 32)]
		public static void ReSaveAllFSMsInBuild()
		{
			ProjectToolsUI.Init();
			ProjectToolsUI.Instance.DoResaveAllFsmsInBuild();
		}


		static IEnumerator DoReSaveAllLoadedFSMs()
		{
			Debug.Log("------ Start DoReSaveAllLoadedFSMs --------- ");

			Debug.Log("-- start LoadPrefabsWithPlayMakerFSMComponents ");

			LoadPrefabsWithPlayMakerFSMComponents();
			yield return null;

			Debug.Log("-- start SaveAllLoadedFSMs ");
			SaveAllLoadedFSMs();

			yield return null;

			Debug.Log("-- start SaveAllTemplates ");

			SaveAllTemplates();

			yield return null;

			Debug.Log("------ End DoReSaveAllLoadedFSMs --------- ");

		}

		public static IEnumerator DoReSaveAllFSMsInBuild()
		{
			feedback.StartProcedure("DoReSaveAllFSMsInBuild");

			EditorCoroutine _LoadPrefabsWithPlayMakerFSMComponents_cr = EditorCoroutine.startManual(LoadPrefabsWithPlayMakerFSMComponents());
			while (_LoadPrefabsWithPlayMakerFSMComponents_cr.routine.MoveNext()) {
				yield return _LoadPrefabsWithPlayMakerFSMComponents_cr.routine.Current;
			}

			yield return null;

			EditorCoroutine _saveAllFsminBuild_cr = EditorCoroutine.startManual(SaveAllFSMsInBuild());
			while (_saveAllFsminBuild_cr.routine.MoveNext()) {
				yield return _saveAllFsminBuild_cr.routine.Current;
			}

			yield return null;

			EditorCoroutine _SaveAllTemplates_cr = EditorCoroutine.startManual(SaveAllTemplates());
			while (_SaveAllTemplates_cr.routine.MoveNext()) {
				yield return _SaveAllTemplates_cr.routine.Current;
			}
			
			yield return null;

			// could trigger a Dialog in the ui for the user to acknowledge it's done.
			feedback.EndProcedure("DoReSaveAllFSMsInBuild");
		}

		private static IEnumerator SaveAllLoadedFSMs()
		{
			feedback.StartProcedure("Save All Loaded FSMs");
			
			feedback.LogAction("Rebuild Fsm List");
			
			FsmEditor.RebuildFsmList();
			
			yield return null;
			
			foreach (var fsm in FsmEditor.FsmList)
			{
				//Debug.Log("Re-save FSM: " + FsmEditorUtility.GetFullFsmLabel(fsm));
				//FsmEditor.SetFsmDirty(fsm, false);
				
				feedback.LogAction("Save action for fsm: "+fsm.GameObjectName+"/"+fsm.Name);
				FsmEditor.SaveActions(fsm);
				yield return null;
			}
			
			feedback.EndProcedure("Save All Loaded FSMs");
		}

		private static IEnumerator SaveAllFSMsInBuild()
		{
			feedback.StartProcedure("Save All FSMs In Build");
			foreach (var scene in EditorBuildSettings.scenes)
			{
				feedback.LogAction("Open Scene: " + scene.path);
				try{
					EditorApplication.OpenScene(scene.path);
					FsmEditor.RebuildFsmList();
					SaveAllLoadedFSMs();
					EditorApplication.SaveScene();
				}catch(Exception e)
				{
					Debug.LogWarning("error : "+e.Message);
				}
				yield return null;
			}
			
			feedback.EndProcedure("Save All FSMs In Build");
		}

		private static IEnumerator SaveAllTemplates()
		{
			feedback.StartProcedure("Re-Saving All Templates");
			
			feedback.LogAction("Rebuild Fsm List");
			
			Templates.InitList();
			
			//FsmEditorUtility.BuildTemplateList();
			
			yield return null;
			
			foreach (var template in Templates.List)
			{
				try
				{
					feedback.LogAction("Set Fsm Dirty"+ template.fsm.Name);
					FsmEditor.SetFsmDirty(template.fsm, false);
					feedback.LogAction("Re-save Template: " + template.name);
				}catch(Exception e)
				{
					Debug.LogWarning("error : "+e.Message);
				}
			}
			
			feedback.EndProcedure("Re-Saving All Templates");
			
		}

		#endif //ECOSYSTEM_TOOLS_BETA
		#endregion Saving


		//[MenuItem(MenuRoot + "Tools/Scan Scenes", false, 33)]
		public static void ScanScenesInProject()
		{
			FindAllScenes();
		}

		private static IEnumerator LoadPrefabsWithPlayMakerFSMComponents()
		{
			feedback.StartProcedure("LoadPrefabsWithPlayMakerFSMComponents");

			feedback.LogAction("Finding Prefabs with PlayMakerFSMs");
			
			var searchDirectory = new DirectoryInfo(Application.dataPath);
			var prefabFiles = searchDirectory.GetFiles("*.prefab", SearchOption.AllDirectories);
			
			yield return null;

			feedback.StartProcedure("Get Dependancies for all Prefabs");

			foreach (var file in prefabFiles)
			{
				var filePath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
				//Debug.Log(filePath + "\n" + Application.dataPath);
				
				var dependencies = AssetDatabase.GetDependencies(new[] { filePath });
				foreach (var dependency in dependencies)
				{
					if (dependency.Contains("/PlayMaker.dll"))
					{
						feedback.LogAction("Found Prefab with FSM: " + filePath);
						AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
					}

					yield return null;
				}

				yield return null;
			}

			feedback.EndProcedure("Get Dependancies for all Prefabs");

			feedback.LogAction("Rebuild FsmList");

			FsmEditor.RebuildFsmList();

			yield return null;

			feedback.EndProcedure("LoadPrefabsWithPlayMakerFSMComponents");
		}

		
		[Localizable(false)]
		private static void FindAllScenes()
		{
			Debug.Log("Finding all scenes...");
			
			var searchDirectory = new DirectoryInfo(Application.dataPath);
			var assetFiles = searchDirectory.GetFiles("*.unity", SearchOption.AllDirectories);
			
			foreach (var file in assetFiles)
			{
				var filePath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
				var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
				if (obj == null)
				{
					//Debug.Log(filePath + ": null!");
				}
				else if (obj.GetType() == typeof(Object))
				{
					Debug.Log(filePath);// + ": " + obj.GetType().FullName);
				}
				//var obj = AssetDatabase.
			}
		}



	}
	
	
}

