
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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

		static string Template_Header = @"// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
// DO NOT EDIT, THIS CONTENT IS AUTOMATICALLY GENERATED
// Please use PlayMaker Enum Creator Wizard to edit this enum definition

using System;
using HutongGames.PlayMaker.Utils.Enum;

[CONTENT]
";

		static string Template_NameSpace = @"namespace [NAMESPACE]
{
[CONTENT]
}";

		static string Template_Class = @"[TAB]public abstract partial class [CLASS_NAME]: EnumClassBase 
[TAB]{
[TAB]	public enum [ENUM_NAME]
[TAB]	{
[CONTENT]
[TAB]	}
[TAB]}";
	
		static string Template_EnumEntry = "{0}\t\t{1}{2}";


		class EnumDefinition
		{
			public string NameSpace = "Net.FabreJean";
			public string Name = "MyEnum";
			public string FolderPath = "";
			public List<string> entries = new List<string>();

			public string ScriptLiteral ="";
		}

		EnumDefinition currentEnum = new EnumDefinition();

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
			string _tab = "";
			string scriptLiteral = Template_Header;
			if (!string.IsNullOrEmpty(currentEnum.NameSpace))
			{
				string _nameSpaceLiteral = Template_NameSpace.Replace("[NAMESPACE]",currentEnum.NameSpace);
				scriptLiteral = scriptLiteral.Replace("[CONTENT]",_nameSpaceLiteral);
				_tab = "\t";
			}
		
			// build the class literal
			string _classLiteral = Template_Class.Replace("[CLASS_NAME]",currentEnum.Name+"Class");
			_classLiteral = _classLiteral.Replace("[ENUM_NAME]",currentEnum.Name);
			_classLiteral = _classLiteral.Replace("[TAB]",_tab);
			// Add the class to the script
			scriptLiteral = scriptLiteral.Replace("[CONTENT]",_classLiteral);

			string _entriesLiteral = "";
			for(int i=0;i<currentEnum.entries.Count;i++)
			{
				_entriesLiteral += string.Format(Template_EnumEntry,_tab,currentEnum.entries[i],"");
				if (i+1<currentEnum.entries.Count)
				{
					_entriesLiteral += ",";
				}
				_entriesLiteral += "\r\n";
			}
			scriptLiteral = scriptLiteral.Replace("[CONTENT]",_entriesLiteral);

			currentEnum.ScriptLiteral = scriptLiteral;
		}
		#endregion

		#region UI

		bool showForm;

		public void OnGUI()
		{
			FsmEditorStyles.Init();
			FsmEditorGUILayout.ToolWindowLargeTitle(this, "Enum Creator");
			
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

		Dictionary<string,ClassFileDetails> _list;
		void OnGUI_DoEditableEnumList()
		{
			if (GUILayout.Button("Find Editable Enums in Projects"))
			{
				_list = ClassFileFinder.GetEnumerableOfType<EnumClassBase>();
			}

			if (_list!=null)
			{
				foreach(KeyValuePair<string,ClassFileDetails> i in _list)
				{
					GUILayout.Label(i.Key);
					GUILayout.Label(i.Value.nameSpace+" : "+i.Value.className+" LastUpdate: "+i.Value.updateTime);
				}
			}

		}

		bool ReBuildPreview;

		void OnGUI_DoEnumDefinitionForm()
		{

			ReBuildPreview = false;


		
			GUILayout.Label("NameSpace:");
		
			string _nameSpace = GUILayout.TextField(currentEnum.NameSpace);
			if (!string.Equals(_nameSpace,currentEnum.NameSpace))
			{
				currentEnum.NameSpace = _nameSpace;
				ReBuildPreview = true;
			}

			GUILayout.Label("Project Folder:");
			GUILayout.Label(Application.dataPath+"/");
			currentEnum.FolderPath = GUILayout.TextField(currentEnum.FolderPath);

			GUILayout.Label("Enum Name");
			string _name = GUILayout.TextField(currentEnum.Name);
			if (!string.Equals(_name,currentEnum.Name))
			{
				currentEnum.Name = _name;
				ReBuildPreview = true;
			}


			ReorderableListGUI.Title("Enum Entries");
			ReorderableListGUI.ListField(currentEnum.entries,DrawListItem);
			FsmEditorGUILayout.Divider();

			if (GUILayout.Button("Create")) // Label "Save Changes" when we detected that we are editing an existing enum
			{
				DoCreateEnum();
			}
		

			FsmEditorGUILayout.Divider();
			GUILayout.TextArea(currentEnum.ScriptLiteral);

			if (ReBuildPreview)
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