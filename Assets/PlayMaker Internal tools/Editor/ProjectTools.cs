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

	public class UIToolsFeedbackBridge
	{

		public void LogAction(string message)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.SetCurrentAction(message);
			}
			
			Debug.Log("Action: "+message);
		}

		public void StartProcedure(string name)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.StartProcedure(name);
			}else{
				Debug.Log("-------- missing instance");
			}

			Debug.Log("Start Procedure: "+name);
		}

		public void EndProcedure(string name)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.EndProcedure(name);
			}

			Debug.Log("End Procedure: "+name);
		}
	}

	public class ProjectToolsTest
	{
		// Change MenuRoot to move the Playmaker Menu
		// E.g., MenuRoot = "Plugins/PlayMaker/"
		public const string MenuRoot = "PlayMaker Test/";
		
		//[MenuItem(MenuRoot + "Tools/Re-Save All Loaded FSMs", false, 31)]
		public static void ReSaveAllLoadedFSMs()
		{
			EditorCoroutine.start(DoReSaveAllLoadedFSMs());
		}

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


		[MenuItem(MenuRoot + "Tools/Re-Save All FSMs in Build", false, 32)]
		public static void ReSaveAllFSMsInBuild()
		{
			ProjectToolsUI.Init();
			ProjectToolsUI.Instance.DoResaveAllFsmsInBuild();
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



		//[MenuItem(MenuRoot + "Tools/Scan Scenes", false, 33)]
		public static void ScanScenesInProject()
		{
			FindAllScenes();
		}
		
		private static IEnumerator SaveAllTemplates()
		{
			feedback.StartProcedure("Re-Saving All Templates");

			feedback.LogAction("Rebuild Fsm List");

			FsmEditorUtility.BuildTemplateList();

			yield return null;

			foreach (var template in FsmEditorUtility.TemplateList)
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

