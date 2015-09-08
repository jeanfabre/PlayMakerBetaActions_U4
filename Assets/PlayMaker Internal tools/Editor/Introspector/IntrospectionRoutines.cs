﻿using System;
using System.Collections;

using System.ComponentModel;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

using HutongGames.PlayMaker;

using HutongGames.PlayMakerEditorUtils;

namespace HutongGames.PlayMakerEditor
{
	public class IntrospectionRoutines
	{
		// Change MenuRoot to move the Playmaker Menu
		// E.g., MenuRoot = "Plugins/PlayMaker/"
		public const string MenuRoot = "PlayMaker Test/";

		public static void StartIntrospectAllLoadedFSMsProcedure()
		{
			EditorCoroutine.start(DoIntrospectAllLoadedFSMs());
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

		static IEnumerator DoIntrospectAllLoadedFSMs()
		{
			Debug.Log("------ Start DoIntrospectAllLoadedFSMs --------- ");

			Debug.Log("-- init IntrospectionXmlProxy ");
			IntrospectionXmlProxy.Init();

			Debug.Log("-- start LoadPrefabsWithPlayMakerFSMComponents ");

			LoadPrefabsWithPlayMakerFSMComponents();
			yield return null;

			Debug.Log("-- start IntrospectAllLoadedFSMs ");
			IntrospectAllLoadedFSMs();

			yield return null;

			Debug.Log("-- start IntrospectAllTemplates ");

			IntrospectAllTemplates();

			yield return null;

			Debug.Log("------ End DoIntrospectAllLoadedFSMs --------- ");

		}


		[MenuItem(MenuRoot + "Tools/Introspect All FSMs in Build", false, 32)]
		public static void StartIntrospectAllFSMsInBuildProcedure()
		{
			ProjectToolsUI.Init();
			ProjectToolsUI.Instance.DoIntrospectAllFsmsInBuild();
		}


		public static IEnumerator DoIntrospectAllFSMsInBuild()
		{
			feedback.StartProcedure("DoIntrospectAllFSMsInBuild");

			feedback.LogAction("-- init xml ");

			yield return EditorCoroutine.start(IntrospectionXmlProxy.Init());

			feedback.LogAction("-- init xml done ");


			yield return EditorCoroutine.start(IntrospectGlobalVariables());

			yield return EditorCoroutine.start(IntrospectGlobalEvents());

			/*
			EditorCoroutine _IntrospectGlobalVariables_cr = EditorCoroutine.startManual(IntrospectGlobalVariables());
			while (_IntrospectGlobalVariables_cr.routine.MoveNext()) {
				yield return _IntrospectGlobalVariables_cr.routine.Current;
			}
			*/

			/*
			yield return null;

			EditorCoroutine _LoadPrefabsWithPlayMakerFSMComponents_cr = EditorCoroutine.startManual(LoadPrefabsWithPlayMakerFSMComponents());
			while (_LoadPrefabsWithPlayMakerFSMComponents_cr.routine.MoveNext()) {
				yield return _LoadPrefabsWithPlayMakerFSMComponents_cr.routine.Current;
			}
			*/

			yield return null;

			EditorCoroutine _IntrospectAllFsminBuild_cr = EditorCoroutine.startManual(IntrospectAllFSMsInBuild());
			while (_IntrospectAllFsminBuild_cr.routine.MoveNext()) {
				yield return _IntrospectAllFsminBuild_cr.routine.Current;
			}

			yield return null;

			EditorCoroutine _IntrospectAllTemplates_cr = EditorCoroutine.startManual(IntrospectAllTemplates());
			while (_IntrospectAllTemplates_cr.routine.MoveNext()) {
				yield return _IntrospectAllTemplates_cr.routine.Current;
			}
			
			yield return null;

			feedback.LogAction("-- save xml");
			IntrospectionXmlProxy.SaveInFile();

			// could trigger a Dialog in the ui for the user to acknowledge it's done.
			feedback.EndProcedure("DoIntrospectAllFSMsInBuild");
		}



		//[MenuItem(MenuRoot + "Tools/Scan Scenes", false, 33)]
		public static void ScanScenesInProject()
		{
			FindAllScenes();
		}

		private static IEnumerator IntrospectGlobalVariables()
		{
			feedback.StartProcedure("Introspect Global Variables");

			// create the Global Variables node:


			XmlElement _gvElement = IntrospectionXmlProxy.AddElement(IntrospectionXmlProxy.XmlDocument.DocumentElement,"GlobalVariables");

			foreach(NamedVariable _var in FsmVariables.GlobalVariables.GetAllNamedVariables())
			{

				// create the Variable node
				XmlElement _element =  IntrospectionXmlProxy.AddElement(_gvElement,"Variable");

				_element.SetAttribute("IsGlobal","true");

				// feed content to the variable node

				// create the name node
				IntrospectionXmlProxy.AddElement(_element,"Name", _var.Name);

				// create the display node
				IntrospectionXmlProxy.AddElement(_element,"DisplayName", _var.GetDisplayName());

				// create the hashcode node
				IntrospectionXmlProxy.AddElement(_element,"HashCode", _var.GetHashCode().ToString());

				// create the tooltip node
				if (!string.IsNullOrEmpty(_var.Tooltip))
				{
					IntrospectionXmlProxy.AddElement(_element,"Tooltip", _var.Tooltip);
				}

				// create the variable type node
				IntrospectionXmlProxy.AddElement(_element,"VariableType", _var.VariableType.ToString());

				if (_var.ShowInInspector)
				{
					IntrospectionXmlProxy.AddElement(_element,"ShowInInspector", "true");
				}
				if (_var.NetworkSync)
				{
					IntrospectionXmlProxy.AddElement(_element,"NetworkSync", "true");
				}

				yield return null;
			}

			feedback.EndProcedure("Introspect Global Variables");
		}


		private static IEnumerator IntrospectGlobalEvents()
		{
			feedback.StartProcedure("Introspect Global Events");

			XmlElement _geElement = IntrospectionXmlProxy.AddElement(IntrospectionXmlProxy.XmlDocument.DocumentElement,"GlobalEvents");


			foreach(FsmEvent _event in PlayMaker.FsmEvent.EventList)
			{
				if (_event.IsApplicationEvent || _event.IsSystemEvent || _event.IsMouseEvent)
				{
					continue;
				}

				// create the Event node
				XmlElement _element =  IntrospectionXmlProxy.AddElement(_geElement,"Event");

				if (_event.IsGlobal)
				{
					_element.SetAttribute("IsGlobal","true");
				}


				// create the name node
				IntrospectionXmlProxy.AddElement(_element,"Name", _event.Name);
				
				// create the path node
				IntrospectionXmlProxy.AddElement(_element,"Path", _event.Path);
				
				// create the hashcode node
				IntrospectionXmlProxy.AddElement(_element,"HashCode", _event.GetHashCode().ToString());

				yield return null;
			}
			
			feedback.EndProcedure("Introspect Global Events");
		}


		private static IEnumerator IntrospectAllTemplates()
		{
			feedback.StartProcedure("Introspect All Templates");

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

			feedback.EndProcedure("Introspect All Templates");

		}
		
		private static IEnumerator IntrospectAllLoadedFSMs()
		{
			feedback.StartProcedure("Introspect All Loaded FSMs");

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

			feedback.EndProcedure("Introspect All Loaded FSMs");
		}


		
		private static IEnumerator IntrospectAllFSMsInBuild()
		{
			feedback.StartProcedure("Introspect All FSMs In Build");
			foreach (var scene in EditorBuildSettings.scenes)
			{
				feedback.LogAction("Open Scene: " + scene.path);
				try{
					EditorApplication.OpenScene(scene.path);
					FsmEditor.RebuildFsmList();
					// HERE WE INTROSPECT FURTHER
					//SaveAllLoadedFSMs();
					Debug.Log("Introspecting scene "+scene.path);

					EditorApplication.SaveScene();
				}catch(Exception e)
				{
					Debug.LogWarning("error : "+e.Message);
				}
				yield return null;
			}

			feedback.EndProcedure("Introspect All FSMs In Build");
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

