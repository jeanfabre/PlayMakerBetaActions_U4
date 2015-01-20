// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for editor window script.
	/// </summary>
	[ScriptTemplate("Scriptable Object", 300)]
	public sealed class ScriptableObjectTemplate : ScriptTemplateGenerator {

		private bool _outputOnEnableMethod;
		private bool _outputOnDisableMethod;
		private bool _outputOnDestroyMethod;

		/// <summary>
		/// Initialize new <see cref="ScriptableObjectTemplate"/> instance.
		/// </summary>
		public ScriptableObjectTemplate() {
			_outputOnEnableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnEnable", false);
			_outputOnDisableMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDisable", false);
			_outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);
		}

		private void UpdateEditorPrefs() {
			EditorPrefs.SetBool("ScriptTemplates.Message.OnEnable", _outputOnEnableMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.OnDisable", _outputOnDisableMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", _outputOnDestroyMethod);
		}

		/// <inheritdoc/>
		public override bool WillGenerateEditorScript {
			get { return IsEditorScript; }
		}

		/// <inheritdoc/>
		public override void OnGUI() {
			EditorGUI.BeginChangeCheck();

			GUILayout.Label("Output Options:", EditorStyles.boldLabel);

			IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);

			EditorGUI.BeginDisabledGroup(!IsEditorScript);
			OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
			EditorGUI.EndDisabledGroup();

			OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);

			EditorGUILayout.Space();

			_outputOnEnableMethod = EditorGUILayout.ToggleLeft("OnEnable Method", _outputOnEnableMethod);
			_outputOnDisableMethod = EditorGUILayout.ToggleLeft("OnDisable Method", _outputOnDisableMethod);
			_outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", _outputOnDestroyMethod);

			if (EditorGUI.EndChangeCheck()) {
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
			if (IsEditorScript && OutputInitializeOnLoad)
				sb.AppendLine("[InitializeOnLoad]");

			sb.BeginNamespace("public class " + scriptName + " : ScriptableObject" + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor)
				sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

			if (_outputOnEnableMethod)
				sb.AppendLine("private void OnEnable()" + OpeningBraceInsertion + "\n}\n");
			if (_outputOnDisableMethod)
				sb.AppendLine("private void OnDisable()" + OpeningBraceInsertion + "\n}\n");
			if (_outputOnDestroyMethod)
				sb.AppendLine("private void OnDestroy()" + OpeningBraceInsertion + "\n}\n");
			
			sb.EndNamespace("}");

			if (!string.IsNullOrEmpty(ns))
				sb.EndNamespace("}");

			return sb.ToString();
		}
	
	}

}
