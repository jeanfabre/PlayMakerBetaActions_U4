// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptTemplates {

	/// <summary>
	/// Base class for basic C# source files.
	/// </summary>
	public abstract class BasicTemplate : ScriptTemplateGenerator {

		private string _typeKeyword;

		/// <summary>
		/// Initialize new <see cref="ClassTemplate"/> instance.
		/// </summary>
		/// <param name="typeKeyword">Keyword of type; for instance, 'class'.</param>
		public BasicTemplate(string typeKeyword) {
			_typeKeyword = typeKeyword;
		}

		/// <inheritdoc/>
		public override bool WillGenerateEditorScript {
			get { return IsEditorScript; }
		}

		/// <inheritdoc/>
		public override void OnGUI() {
			GUILayout.Label("Output Options:", EditorStyles.boldLabel);

			IsEditorScript = EditorGUILayout.ToggleLeft("Editor Script", IsEditorScript);
			if (!IsEditorScript)
				OutputInitializeOnLoad = false;

			if (_typeKeyword == "class") {
				EditorGUI.BeginDisabledGroup(!IsEditorScript);
				OutputInitializeOnLoad = EditorGUILayout.ToggleLeft("Initialize On Load", OutputInitializeOnLoad);
				EditorGUI.EndDisabledGroup();

				OutputStaticConstructor = EditorGUILayout.ToggleLeft("Static Constructor", OutputStaticConstructor || OutputInitializeOnLoad);
			}

			EditorGUILayout.Space();

			EditorGUILayout.PrefixLabel("Visibility");
			TypeVisibility = (TypeVisibility)EditorGUILayout.EnumPopup(TypeVisibility);

			if (_typeKeyword == "class") {
				StaticClass = EditorGUILayout.ToggleLeft("Static", StaticClass);
				PartialClass = EditorGUILayout.ToggleLeft("Partial", PartialClass);
			}
		}

		/// <inheritdoc/>
		public override string GenerateScript(string scriptName, string ns) {
			var sb = CreateScriptBuilder();

			sb.AppendLine("using UnityEngine;");
			if (IsEditorScript)
				sb.AppendLine("using UnityEditor;");
			sb.AppendLine();
			sb.AppendLine("using System.Collections;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine();

			if (!string.IsNullOrEmpty(ns))
				sb.BeginNamespace("namespace " + ns + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (_typeKeyword == "class" && (IsEditorScript && OutputInitializeOnLoad))
				sb.AppendLine("[InitializeOnLoad]");

			// Build type declaration string.
			var declaration = new List<string>();

			if (TypeVisibility == TypeVisibility.Public)
				declaration.Add("public");
			else if (TypeVisibility == TypeVisibility.Internal)
				declaration.Add("internal");

			if (_typeKeyword == "class") {
				if (StaticClass)
					declaration.Add("static");
				else if (PartialClass)
					declaration.Add("partial");
			}

			declaration.Add(_typeKeyword);
			declaration.Add(scriptName);

			sb.BeginNamespace(string.Join(" ", declaration.ToArray()) + OpeningBraceInsertion);

			// Automatically initialize on load?
			if (_typeKeyword == "class" && (IsEditorScript && OutputInitializeOnLoad || OutputStaticConstructor))
				sb.AppendLine("static " + scriptName + "()" + OpeningBraceInsertion + "\n}\n");

			sb.EndNamespace("}");

			if (!string.IsNullOrEmpty(ns))
				sb.EndNamespace("}");

			return sb.ToString();
		}
	
	}

}
