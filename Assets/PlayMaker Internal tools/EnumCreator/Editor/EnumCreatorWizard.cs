﻿
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using HutongGames.PlayMakerEditorUtils;
using HutongGames.PlayMaker.Utils.Enum;

using Rotorz.ReorderableList;

namespace HutongGames.PlayMakerEditor
{
	public class EnumCreatorWizard : EditorWindow
	{
		
	
		#region Logic 

		static string Template_MainStructure = @"// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
// DO NOT EDIT, THIS CONTENT IS AUTOMATICALLY GENERATED
// [TAG]
// Please use PlayMaker Enum Creator Wizard to edit this enum definition

namespace [NAMESPACE]
{
	public enum [ENUM_NAME]
	{
[ENUM_ENTRIES]		
	}
}
";

		static string Template_EnumEntry = "\t\t{0}{1}";

		[Serializable]
		public class EnumDefinition
		{
			public string NameSpace = "Net.FabreJean";
			public string Name = "MyEnum";
			public string FolderPath = "PlayMaker Custom Scripts/";
			public List<string> entries = new List<string>();

			public string ScriptLiteral ="";
		}

		public EnumDefinition currentEnum = new EnumDefinition();

		/// <summary>
		/// Create a new script featuring the new enum.
		/// </summary>
		void DoCreateEnum()
		{
			BuildScriptLiteral();
		
			string fileName = currentEnum.Name+".cs";
			string outputPath = Path.Combine(Application.dataPath, currentEnum.FolderPath);


			// Ensure that this path actually exists.
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);


			string filePath = Path.Combine(outputPath,fileName);
			File.WriteAllText(filePath, currentEnum.ScriptLiteral);
			AssetDatabase.Refresh();
		}


		void BuildScriptLiteral()
		{
			string scriptLiteral = Template_MainStructure;

			scriptLiteral = Template_MainStructure.Replace("[TAG]","["+"PLAYMAKER_ENUM]"); // we recompose the tag to avoid detection of this very script
			scriptLiteral = Template_MainStructure.Replace("[NAMESPACE]",currentEnum.NameSpace);
		
			scriptLiteral = scriptLiteral.Replace("[ENUM_NAME]",currentEnum.Name);
	
			string _entriesLiteral = "";
			for(int i=0;i<currentEnum.entries.Count;i++)
			{
				_entriesLiteral += string.Format(Template_EnumEntry,currentEnum.entries[i],"");
				if (i+1<currentEnum.entries.Count)
				{
					_entriesLiteral += ",\r\n";
				}

			}
			scriptLiteral = scriptLiteral.Replace("[ENUM_ENTRIES]",_entriesLiteral);

			currentEnum.ScriptLiteral = scriptLiteral;
		}

		void StartEditingEnum(EnumFileDetails enumDetails)
		{
			_sourceDetails = enumDetails;

			currentEnum = new EnumDefinition();

			// nameSpace
			currentEnum.NameSpace = enumDetails.nameSpace;

			currentEnum.Name = enumDetails.enumName;

			Type _type = System.Type.GetType(currentEnum.NameSpace+"."+currentEnum.Name+", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
		
			Debug.Log(currentEnum.Name+" : "+_type);

			currentEnum.entries = new List<string>();

			FieldInfo[] fields = _type.GetFields();
			
			foreach (var field in fields) {
				if (field.Name.Equals("value__")) continue;

				currentEnum.entries.Add(field.Name);
				//Debug.Log(field.Name + ":" + field.GetRawConstantValue());
			}

			Repaint();
			ReBuildPreview = true;
			GUI.FocusControl(_unfocusControlName);

		}

		#endregion Logic

		#region UI

		bool showForm;

		static string _unfocusControlName ="Unfocus";

		public void OnGUI()
		{
			FsmEditorStyles.Init();
			FsmEditorGUILayout.ToolWindowLargeTitle(this, "Enum Creator");

			// unfocus invisible field
			GUI.SetNextControlName(_unfocusControlName);
			GUI.TextField(new Rect(0,-100,100,20),"");


			OnGUI_ToolBar();

			if (showForm)
			{
				OnGUI_DoEnumDefinitionForm();
			}
		}
		
		void OnGUI_ToolBar()
		{
			FsmEditorGUILayout.Divider();
			
			GUILayout.Space(5);

			OnGUI_DoEditableEnumList();

			FsmEditorGUILayout.Divider();
			
			GUILayout.Space(5);

			if (!showForm)
			{
				if ( GUILayout.Button("New Enum") )
				{
					currentEnum = new EnumDefinition();
					showForm = true;
				}
			}

		}

		Dictionary<string,EnumFileDetails> _list;
		void OnGUI_DoEditableEnumList()
		{
			if (GUILayout.Button("Find Editable Enums in Projects"))
			{
				_list = ClassFileFinder.FindEnumFiles();
			}

			if (_list!=null)
			{
				foreach(KeyValuePair<string,EnumFileDetails> i in _list)
				{
					OnGUI_doEditableEnumItem(i.Key,i.Value);
				}
			}

		}

		EnumFileDetails _sourceDetails;

		void OnGUI_doEditableEnumItem(string filePath,EnumFileDetails details)
		{
			GUILayout.BeginHorizontal();

			GUILayout.Label(details.nameSpace+"."+details.enumName);

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Select in Project"))
			{
				var _object = AssetDatabase.LoadAssetAtPath("Assets/"+details.projectPath,typeof(UnityEngine.Object));

				EditorGUIUtility.PingObject(_object.GetInstanceID());
				Selection.activeInstanceID = _object.GetInstanceID();
			}

			if (GUILayout.Button("Edit"))
			{
				StartEditingEnum(details);
			}

			GUILayout.EndHorizontal();
		}

		bool ReBuildPreview;

		void OnGUI_DoEnumDefinitionForm()
		{

			ReBuildPreview = false;

			GUILayout.Label("Project Folder:");
			currentEnum.FolderPath = GUILayout.TextField(currentEnum.FolderPath);

			GUILayout.Label("NameSpace:");
		
			string _nameSpace = GUILayout.TextField(currentEnum.NameSpace);
			if (!string.Equals(_nameSpace,currentEnum.NameSpace))
			{
				currentEnum.NameSpace = _nameSpace;
				ReBuildPreview = true;
			}

			GUILayout.Label("Enum Name:");
			string _name = GUILayout.TextField(currentEnum.Name);
			if (!string.Equals(_name,currentEnum.Name))
			{
				currentEnum.Name = _name;
				ReBuildPreview = true;
			}


			ReorderableListGUI.Title("Enum Entries");
			ReorderableListGUI.ListField(currentEnum.entries,DrawListItem);
			FsmEditorGUILayout.Divider();

			GUILayout.BeginHorizontal();

				if (GUILayout.Button("Cancel Changes"))
				{
					StartEditingEnum(_sourceDetails);
				}

				GUILayout.Space(50);

				if (GUILayout.Button("Create")) // Label "Save Changes" when we detected that we are editing an existing enum
				{
					DoCreateEnum();
				}

			GUILayout.EndHorizontal();

			FsmEditorGUILayout.Divider();

			GUILayout.Label("Code Source Preview:");
			GUILayout.TextArea(currentEnum.ScriptLiteral);

			if (ReBuildPreview || string.IsNullOrEmpty(currentEnum.ScriptLiteral))
			{
				BuildScriptLiteral();
				Repaint();
			}

		}

		private string DrawListItem(Rect position, string value) {
			// Text fields do not like `null` values!
			if (value == null)
				value = "";

			string _newValue = EditorGUI.TextField(position, value);
			if (!string.Equals(_newValue,value))
			{
				ReBuildPreview = true;
			}
			return _newValue;
		}

		#endregion
		
		#region Window Management

		public static EnumCreatorWizard Instance;

		// Add menu named "My Window" to the Window menu
		[MenuItem (ProjectToolsTest.MenuRoot + "Tools/Enum Creator Wizard")]
		static public void Init () {
			
			// Get existing open window or if none, make a new one:
			Instance = (EnumCreatorWizard)EditorWindow.GetWindow (typeof (EnumCreatorWizard));

			Instance.Initialize();
		}
		
		public void Initialize()
		{
			Instance = this;
			
			InitWindowTitle();
			position =  new Rect(120,120,300,292);
			// initial fixed size
			minSize = new Vector2(300, 292);
		}
		
		public void InitWindowTitle()
		{
			title = "Enum Creator";
		}
		
		
		#endregion Window Management
	}
}