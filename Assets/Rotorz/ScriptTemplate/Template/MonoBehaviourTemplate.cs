// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for MonoBehaviour script.
	/// </summary>
	[ScriptTemplate("Mono Behaviour", 0)]
	public sealed class MonoBehaviourTemplate : ScriptTemplateGenerator {

		private bool _outputAwakeMethod;
		private bool _outputStartMethod;
		private bool _outputUpdateMethod;
		private bool _outputOnDestroyMethod;

		/// <summary>
		/// Initialize new <see cref="MonoBehaviourTemplate"/> instance.
		/// </summary>
		public MonoBehaviourTemplate() {
			_outputAwakeMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Awake", false);
			_outputStartMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Start", true);
			_outputUpdateMethod = EditorPrefs.GetBool("ScriptTemplates.Message.Update", true);
			_outputOnDestroyMethod = EditorPrefs.GetBool("ScriptTemplates.Message.OnDestroy", false);
		}

		private void UpdateEditorPrefs() {
			EditorPrefs.SetBool("ScriptTemplates.Message.Awake", _outputAwakeMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.Start", _outputStartMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.Update", _outputUpdateMethod);
			EditorPrefs.SetBool("ScriptTemplates.Message.OnDestroy", _outputOnDestroyMethod);
		}

		/// <inheritdoc/>
		public override bool WillGenerateEditorScript {
			get { return false; }
		}

		/// <inheritdoc/>
		public override void OnGUI() {
			EditorGUI.BeginChangeCheck();

			GUILayout.Label("Output Options:", EditorStyles.boldLabel);

			OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor);

			EditorGUILayout.Space();

			_outputAwakeMethod = EditorGUILayout.ToggleLeft("Awake Method", _outputAwakeMethod);
			_outputStartMethod = EditorGUILayout.ToggleLeft("Start Method", _outputStartMethod);
			_outputUpdateMethod = EditorGUILayout.ToggleLeft("Update Method", _outputUpdateMethod);
			_outputOnDestroyMethod = EditorGUILayout.ToggleLeft("OnDestroy Method", _outputOnDestroyMethod);

			if (EditorGUI.EndChangeCheck())
				UpdateEditorPrefs();
		}

		/// <inheritdoc/>
		public override string GenerateScript(string scriptName, string ns) {
			var sb = CreateScriptBuilder();

			sb.AppendLine("using UnityEngine;");
			sb.AppendLine();
			sb.AppendLine("using System.Collections;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine();

			if (!string.IsNullOrEmpty(ns))
				sb.BeginNamespace("namespace " + ns + OpeningBraceInsertion);

			sb.BeginNamespace("public class " + scriptName + " : MonoBehaviour" + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (OutputStaticConstructor)
				sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

			if (_outputAwakeMethod)
				sb.AppendLine("private void Awake()" + OpeningBraceInsertion + "\n}\n");
			if (_outputStartMethod)
				sb.AppendLine("private void Start()" + OpeningBraceInsertion + "\n}\n");
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
