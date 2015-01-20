// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for editor menu script.
	/// </summary>
	[ScriptTemplate("Editor Menu", 200)]
	public sealed class EditorMenuTemplate : ScriptTemplateGenerator {

		private string _menuItem;

		/// <summary>
		/// Initialize new <see cref="EditorMenuTemplate"/> instance.
		/// </summary>
		public EditorMenuTemplate() {
			_menuItem = EditorPrefs.GetString("ScriptTemplates.Shared.MenuItem", "");
		}

		private void UpdateEditorPrefs() {
			EditorPrefs.SetString("ScriptTemplates.Shared.MenuItem", _menuItem);
		}

		/// <inheritdoc/>
		public override bool WillGenerateEditorScript {
			get { return true; }
		}

		/// <inheritdoc/>
		public override void OnGUI() {
			EditorGUI.BeginChangeCheck();

			GUILayout.Label("Output Options:", EditorStyles.boldLabel);

			EditorGUILayout.PrefixLabel("Menu Item (optional)");
			_menuItem = EditorGUILayout.TextField(_menuItem);

			EditorGUILayout.Space();

			OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
			OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

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

			sb.BeginNamespace("static class " + scriptName + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (OutputInitializeOnLoad || OutputStaticConstructor)
				sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

			if (!string.IsNullOrEmpty(_menuItem)) {
				string menuName = _menuItem;
				if (!menuName.Contains("/"))
					menuName = "Window/" + menuName;

				sb.AppendLine("[MenuItem(\"" + menuName + "\")]");
				sb.AppendLine("private static void " + menuName.Replace("/", "_").Replace(" ", "_") + "()" + OpeningBraceInsertion);
				sb.AppendLine("}\n");
			}

			sb.EndNamespace("}");

			if (!string.IsNullOrEmpty(ns))
				sb.EndNamespace("}");

			return sb.ToString();
		}
	
	}

}
