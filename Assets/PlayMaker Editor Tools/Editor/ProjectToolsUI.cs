﻿
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using HutongGames.PlayMakerEditorUtils;

#pragma warning disable 0219, 0414

namespace HutongGames.PlayMakerEditor
{
	public class ProjectToolsUI : BaseEditorWindow
	{

		public static ProjectToolsUI Instance;
		
		public static bool ActionInProgress
		{
			get{
				if (Instance!=null)
				{
					return Instance.Procedures.Count>0;
				}

				return false;
			}
		}

		public override void DoGUI()
		{
			FsmEditorStyles.Init();
			FsmEditorGUILayout.ToolWindowLargeTitle(this, "Tools");

			OnGUI_ToolBar();

		}

		void OnGUI_ToolBar()
		{
			FsmEditorGUILayout.Divider();

			GUILayout.Space(5);
			if (!ActionInProgress)
			{
				#if ECOSYSTEM_TOOLS_BETA
				if ( GUILayout.Button("Re-Save All Fsm In Builds") )
				{
					DoResaveAllFsmsInBuild();
				}

				#endif

				if ( GUILayout.Button("Introspect All Fsm In Builds") )
				{
					DoIntrospectAllFsmsInBuild();
				}

			}else{
				OnGUI_DoToolFeedback();
			}
		
		}

		#if ECOSYSTEM_TOOLS_BETA
		public void DoResaveAllFsmsInBuild()
		{
			EditorCoroutine.start(ProjectToolsTest.DoReSaveAllFSMsInBuild());
		}

		#endif

		public void DoIntrospectAllFsmsInBuild()
		{
			EditorCoroutine.start(IntrospectionRoutines.DoIntrospectAllFSMsInBuild());
		}

		void OnGUI_DoToolFeedback()
		{
			if (GUILayout.Button("CANCEL"))
			{
				Procedures = new List<string>();
				CurrentAction = "";
			}

			foreach(string _procedure in Procedures)
			{
				GUILayout.Label(_procedure);
				EditorGUI.indentLevel++;
			}

			GUILayout.Label(CurrentAction);

			foreach(string _procedure in Procedures)
			{
				EditorGUI.indentLevel--;
			}

			Repaint();

		}


		#region Procedure

		List<string> Procedures = new List<string>();

		public void StartProcedure(string name)
		{
			Procedures.Add(name);
		}

		public void EndProcedure(string name)
		{
			Procedures.Remove(name);
		}

		string ContextTitle = "";

		string currentContext = "";
		string CurrentAction = "";


		public void SetContextTitle(string label)
		{
			ContextTitle = label;
		}

		public void SetCurrentAction(string label)
		{
			CurrentAction = label;
		}


		#endregion Procedure

		#region Window Management

		// Add menu named "My Window" to the Window menu
		[MenuItem (ProjectToolsTest.MenuRoot + "Tools/Save Wizard")]
		static public void Init () {
			
			// Get existing open window or if none, make a new one:
			Instance = (ProjectToolsUI)EditorWindow.GetWindow (typeof (ProjectToolsUI));
		}
		
		public override void Initialize()
		{
			Instance = this;

			InitWindowTitle();
			position =  new Rect(120,120,264,292);
			// initial fixed size
			minSize = new Vector2(264, 292);
		}
		
		public void InitWindowTitle()
		{
			title = "Update Tools";
		}


		#endregion Window Management
	}
}