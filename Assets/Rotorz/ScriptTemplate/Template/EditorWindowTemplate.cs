// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for editor window script.
	/// </summary>
	[ScriptTemplate("Editor Window", 100)]
	public sealed class EditorWindowTemplate : ScriptTemplateGenerator {

		private bool _outputOnEnableMethod;
		private bool _outputOnDisableMethod;
		private bool _outputOnGUIMethod;
		private bool _outputUpdateMethod;
		private bool _outputOnDestroyMethod;

		private string _menuItem;
		private bool _utility;

		/// <summary>
		/// Initialize new <see cref="EditorWindowTemplate"/> instance.
		/// </summary>
		public EditorWindowTemplate() {
			_outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
			_outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
			_outputOnGUIMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnGUI", false);
			_outputUpdateMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Update", false);
			_outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);

			_menuItem = EditorPrefs.GetString("ScriptTemplates.Shared.MenuItem", "");
			_utility = EditorPrefs.GetBool("ScriptTemplates.EditorWindow.Utility", false);
		}

		private void UpdateEditorPrefs() {
			EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", _outputOnEnableMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", _outputOnDisableMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.OnGUI", _outputOnGUIMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.Update", _outputUpdateMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", _outputOnDestroyMethod);

			EditorPrefs.SetString("ScriptTemplates.Shared.MenuItem", _menuItem);
			EditorPrefs.SetBool("ScriptTemplates.EditorWindow.Utility", _utility);
		}

		/// <inheritdoc/>
		public override bool WillGenerateEditorScript {
			get { return true; }
		}

		/// <inheritdoc/>
		public override void OnGUI() {
			EditorGUI.BeginChangeCheck();

			GUILayout.Label("Output Options:", EditorStyles.boldLabel);

			OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
			OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);
			
			EditorGUILayout.Space();

			_outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", _outputOnEnableMethod);
			_outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", _outputOnDisableMethod);
			_outputOnGUIMethod = EditorGUILayout.ToggleLeft("OnGUI Method", _outputOnGUIMethod);
			_outputUpdateMethod = EditorGUILayout.ToggleLeft("Update Method", _outputUpdateMethod);
			_outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", _outputOnDestroyMethod);

			EditorGUILayout.Space();

			EditorGUILayout.PrefixLabel("Menu Item (optional)");
			_menuItem = EditorGUILayout.TextField(_menuItem);

			EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_menuItem));
			_utility = EditorGUILayout.ToggleLeft("Utility Window", _utility);
			EditorGUI.EndDisabledGroup();

			if (EditorGUI.EndChangeCheck()) {
				_menuItem = _menuItem.Trim();

				UpdateEditorPrefs();
			}
		}

		/// <inheritdoc/>
		public override string GenerateScript(string scriptName, string ns) {
			var sb = CreateScriptBuilder();

			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("using UnityEditor;");
			sb.AppendLine();
			sb.AppendLine("using System.Collections;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine();

			if (!string.IsNullOrEmpty(ns))
				sb.BeginNamespace("namespace " + ns + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (OutputInitializeOnLoad)
				sb.AppendLine("[InitializeOnLoad]");

			sb.BeginNamespace("public class " + scriptName + " : EditorWindow" + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (OutputInitializeOnLoad || OutputStaticConstructor)
				sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

			if (!string.IsNullOrEmpty(_menuItem)) {
				string menuName = _menuItem;
				if (!menuName.Contains("/"))
					menuName = "Window/" + menuName;

				string utilityArg = _utility ? "true, " : "";

				sb.AppendLine("[MenuItem(\"" + menuName + "\")]");
				sb.AppendLine("private static void ShowWindow()" + OpeningBraceInsertion);
				sb.AppendLine("\tGetWindow<" + scriptName + ">(" + utilityArg + "\"" + _menuItem + "\");");
				sb.AppendLine("}\n");
			}

			if (_outputOnEnableMethod)
				sb.AppendLine("private void OnEnable()" + OpeningBraceInsertion + "\n}\n");
			if (_outputOnDisableMethod)
				sb.AppendLine("private void OnDisable()" + OpeningBraceInsertion + "\n}\n");
			if (_outputOnGUIMethod)
				sb.AppendLine("private void OnGUI()" + OpeningBraceInsertion + "\n}\n");
			if (_outputUpdateMethod)
				sb.AppendLine("private void Update()" + OpeningBraceInsertion + "\n}\n");
			if (_outputOnDestroyMethod)
				sb.AppendLine("private void OnDestroy()" + OpeningBraceInsertion + "\n}\n");
			
			sb.EndNamespace("}");

			if (!string.IsNullOrEmpty(ns))
				sb.EndNamespace("}");

			return sb.ToString();
		}
	
	}

}
