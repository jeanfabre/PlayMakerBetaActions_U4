// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using HutongGames.PlayMakerEditorUtils;

using Rotorz.ReorderableList;

namespace HutongGames.PlayMakerEditor
{
	public class EnumCreatorWizard : EditorWindow
	{ 

		EnumCreator enumCreator = new EnumCreator();


		public EnumCreator.EnumDefinition currentEnum = new EnumCreator.EnumDefinition();

		void StartEditingEnum(EnumFileDetails enumDetails)
		{
			_sourceDetails = enumDetails;

			currentEnum = new EnumCreator.EnumDefinition();

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

		#region UI

		bool showForm;

		static string _unfocusControlName ="Unfocus";

		public void OnGUI()
		{
			FsmEditorStyles.Init();

			// unfocus invisible field
			GUI.SetNextControlName(_unfocusControlName);
			GUI.TextField(new Rect(0,-100,100,20),"");
		
			FsmEditorGUILayout.ToolWindowLargeTitle(this, "Enum Creator");

			OnGUI_HorizontalSplitView();
			/*
			OnGUI_ToolBar();

			if (showForm)
			{
				OnGUI_DoEnumDefinitionForm();
			}

			*/
		}


		[SerializeField]
		float currentScrollViewHeight = -1f;
		bool resize = false;
		Rect cursorChangeRect;

		void OnGUI_HorizontalSplitView()
		{
		
			GUILayout.BeginVertical();

				OnGUI_DoEditableEnumList(currentScrollViewHeight);

			OnGUI_HorizontalResizeScrollView();

				OnGUI_DoEnumDefinitionForm();

			GUILayout.EndVertical();

			if (resize) Repaint();
		}

		void OnGUI_HorizontalResizeScrollView(){

			if (currentScrollViewHeight<0)
			{
				currentScrollViewHeight = 100;

				cursorChangeRect = new Rect(0,currentScrollViewHeight,this.position.width,5f);
			}

			FsmEditorGUILayout.Divider();

			GUI.Box(cursorChangeRect,"","box");
		
			FsmEditorGUILayout.Divider();

			GUILayout.Space(5);

			if (resize)
			{
				EditorGUIUtility.AddCursorRect(this.position,MouseCursor.ResizeVertical);

				currentScrollViewHeight = Mathf.Max(65,Event.current.mousePosition.y);

				if(Event.current.type == EventType.MouseUp)
				{
					resize = false;  
				}

			}else{
				EditorGUIUtility.AddCursorRect(cursorChangeRect,MouseCursor.ResizeVertical);
				if( Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition)){
					resize = true;
					currentScrollViewHeight = Event.current.mousePosition.y;

				}
			}
    
			cursorChangeRect.Set(cursorChangeRect.x,currentScrollViewHeight,this.position.width,cursorChangeRect.height);

		}

		[SerializeField]
		Vector2 EnumListScrollPosition;

		Dictionary<string,EnumFileDetails> _list;
		void OnGUI_DoEditableEnumList(float height)
		{

			GUILayout.Label("Editable Enums");

			//Rect _lastRect = GUILayoutUtility.GetLastRect();
			// I am failing to get the proper height, because of the title and label above the scrollview... 
			// so 61 is the top banner and the label above...
			EnumListScrollPosition = GUILayout.BeginScrollView(EnumListScrollPosition,GUILayout.Height(height-61));

			if (_list!=null)
			{
				foreach(KeyValuePair<string,EnumFileDetails> i in _list)
				{
					OnGUI_DoEditableEnumItem(i.Key,i.Value);
				}
			}

			GUILayout.EndScrollView();
		}

		EnumFileDetails _sourceDetails;


		void OnGUI_DoEditableEnumItem(string filePath,EnumFileDetails details)
		{
			GUILayout.BeginHorizontal("box",GUILayout.ExpandHeight(false));

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
			Color _orig = Color.clear;
			ReBuildPreview = false;

			// FOLDER
			_orig = GUI.color;
			if (!currentEnum.FolderPathValidation.success)
			{
				GUI.color = Color.red;
			}
			GUILayout.Label("Project Folder: <color=#B20000><b>"+currentEnum.FolderPathValidation.message+"</b></color>");
			currentEnum.FolderPath = GUILayout.TextField(currentEnum.FolderPath);


			// NAMESPACE
			_orig = GUI.color;
			if (!currentEnum.NameSpaceValidation.success)
			{
				GUI.color = Color.red;
			}
			GUILayout.Label("NameSpace: <color=#B20000><b>"+currentEnum.NameSpaceValidation.message+"</b></color>");
			string _nameSpace = GUILayout.TextField(currentEnum.NameSpace);
			GUI.color = _orig;
			if (!string.Equals(_nameSpace,currentEnum.NameSpace))
			{
				currentEnum.NameSpace = _nameSpace;
				ReBuildPreview = true;
			}

			// NAME
			_orig = GUI.color;
			if (!currentEnum.NameValidation.success)
			{
				GUI.color = Color.red;
			}
			GUILayout.Label("Enum Name: <color=#B20000><b>"+currentEnum.NameValidation.message+"</b></color>");
			string _name = GUILayout.TextField(currentEnum.Name);
			GUI.color = _orig;
			if (!string.Equals(_name,currentEnum.Name))
			{
				currentEnum.Name = _name;
				ReBuildPreview = true;
			}

			// ENTRIES

			int count = currentEnum.entries.Count;

			List<string> _origEntries = new List<string>(currentEnum.entries);
			ReorderableListGUI.Title("Enum Entries:  <color=#B20000><b>"+currentEnum.EntriesValidation.message+"</b></color>");
			ReorderableListGUI.ListField(currentEnum.entries,DrawListItem);

			if (currentEnum.entries.Count != count || _origEntries != currentEnum.entries)
			{
				ReBuildPreview = true;
			}

			FsmEditorGUILayout.Divider();

			if (!currentEnum.DefinitionValidation.success)
			{
				GUILayout.Label("<color=#B20000><b>"+currentEnum.DefinitionValidation.message+"</b></color>");
			}

			GUILayout.BeginHorizontal();

				if (GUILayout.Button("Cancel Changes"))
				{
					StartEditingEnum(_sourceDetails);
				}

				GUILayout.Space(50);

			if (currentEnum.DefinitionValidation.success)
			{
				if (GUILayout.Button("Create")) // Label "Save Changes" when we detected that we are editing an existing enum
				{
					enumCreator.CreateEnum(currentEnum);
				}
			}else{
			
				Color _color = GUI.color;

				_color.a = 0.5f;
				GUI.color = _color;
				GUILayout.Label("Create","Button");
				_color.a = 1f;
				GUI.color =_color;
			}

			GUILayout.EndHorizontal();

			FsmEditorGUILayout.Divider();

			GUILayout.Label("Code Source Preview:");
			GUILayout.TextArea(currentEnum.EnumLiteralPreview);

			if (ReBuildPreview || string.IsNullOrEmpty(currentEnum.ScriptLiteral))
			{
				currentEnum.ValidateDefinition();

				enumCreator.BuildScriptLiteral(currentEnum);
				Repaint();
			}

		}

		private string DrawListItem(Rect position, string value) {
			// Text fields do not like `null` values!
			if (value == null)
			{
				value = "";
				ReBuildPreview = true;
			}
				

			Color _origColor = GUI.color;

			bool hasValidationResult =  currentEnum.EntryValidations.ContainsKey(value);
			if (hasValidationResult)
			{
				EnumCreator.ValidationResult _validationResult = currentEnum.EntryValidations[value];

				if (!_validationResult.success)
				{
					GUI.color = Color.red;
				}
			}

			// check if that index is validated
			string _newValue = EditorGUI.TextField(position, value);

			GUI.color = _origColor;


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
			Debug.Log("Init");
			Instance = this;
			
			InitWindowTitle();
			position =  new Rect(120,120,300,292);
			// initial fixed size
			minSize = new Vector2(300, 292);

			// set style ot use rich text.
			GUIStyle labelStyle = GUI.skin.GetStyle("Label");
			labelStyle.richText = true;
		}
		
		public void InitWindowTitle()
		{
			title = "Enum Creator";
		}

		protected virtual void OnEnable()
		{
			Debug.Log("OnEnable");
			_list = ClassFileFinder.FindEnumFiles();
		}
		
		
		#endregion Window Management
	}
}