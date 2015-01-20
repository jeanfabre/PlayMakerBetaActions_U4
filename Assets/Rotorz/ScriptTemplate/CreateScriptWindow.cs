// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ScriptTemplates {

	public class CreateScriptWindow : EditorWindow {

		[MenuItem("Window/Create Script from Template")]
		private static void ShowWindow() {
			GetWindow<CreateScriptWindow>("Create Script");
		}

		[SerializeField]
		private string _scriptName = "";
		[SerializeField]
		private string _ns = "";
		[SerializeField]
		private bool _nsForSubFolders;

		[NonSerialized]
		private string[] _templateDescriptions;

		[SerializeField]
		private int _templateIndex;
		[NonSerialized]
		private ScriptTemplateGenerator _activeGenerator;

		private void OnEnable() {
			minSize = new Vector2(230, 100);

			_nsForSubFolders = EditorPrefs.GetBool("ScriptTemplates.NamespaceToDirectory", true);

			AutoFixActiveGenerator();
		}

		private Vector2 _scrollPosition;

		private void OnGUI() {
			EditorGUILayout.Space();
			DrawTemplateSelector();
			EditorGUILayout.Space();

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			{
				DrawStandardInputs();

				EditorGUILayout.Space();
				_activeGenerator.OnGUI();

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndScrollView();

			GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));
			_activeGenerator.OnStandardGUI();

			DrawButtonStrip();
			EditorGUILayout.Space();
		}
		
		private void DrawTemplateSelector() {
			if (_templateDescriptions == null)
				_templateDescriptions = ScriptGeneratorDescriptor.Descriptors
					.Select(descriptor => descriptor.Attribute.Description)
					.ToArray();

			// Allow user to select template from popup.
			EditorGUILayout.PrefixLabel("Template:");
			EditorGUI.BeginChangeCheck();
			_templateIndex = EditorGUILayout.Popup(_templateIndex, _templateDescriptions);
			if (EditorGUI.EndChangeCheck())
				_activeGenerator = null;

			AutoFixActiveGenerator();
		}

		private void DrawStandardInputs() {
			EditorGUILayout.PrefixLabel("Script Name:");
			_scriptName = EditorGUILayout.TextField(_scriptName);

			EditorGUILayout.PrefixLabel("Namespace:");
			_ns = EditorGUILayout.TextField(_ns);

			EditorGUI.BeginChangeCheck();
			_nsForSubFolders = EditorGUILayout.ToggleLeft("Use namespace for sub-folders", _nsForSubFolders);
			if (EditorGUI.EndChangeCheck())
				EditorPrefs.SetBool("ScriptTemplates.NamespaceToDirectory", _nsForSubFolders);
		}

		private void DrawButtonStrip() {
			GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));

			if (GUILayout.Button("Save at Default Path"))
				DoSaveAtDefaultPath();
			if (GUILayout.Button("Save As"))
				DoSaveAs();

			EditorGUILayout.Space();

			if (GUILayout.Button("Copy to Clipboard"))
				DoCopyToClipboard();
		}

		private void AutoFixActiveGenerator() {
			if (_activeGenerator == null) {
				_templateIndex = Mathf.Clamp(_templateIndex, 0, ScriptGeneratorDescriptor.Descriptors.Count);
				_activeGenerator = ScriptGeneratorDescriptor.Descriptors[_templateIndex].CreateInstance();
			}
		}

		private void ResetInputs() {
			AutoFixActiveGenerator();

			_scriptName = "";
			_ns = "";
		}

		private string GetDefaultOutputPath() {
			string assetFolder = Path.Combine("Assets", _activeGenerator.WillGenerateEditorScript ? "Editor/Scripts" : "Scripts");

			// Use namespace for sub-folders?
			if (_nsForSubFolders && !string.IsNullOrEmpty(_ns))
				assetFolder = Path.Combine(assetFolder, _ns.Replace("_", "/"));

			string outputPath = Path.Combine(Directory.GetCurrentDirectory(), assetFolder);

			// Ensure that this path actually exists.
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			return outputPath;
		}

		private void DoSaveAtDefaultPath() {
			GenerateScriptFromTemplate(Path.Combine(GetDefaultOutputPath(), _scriptName + ".cs"));
		}

		private void DoSaveAs() {
			// Prompt user to specify path to save script.
			string path = EditorUtility.SaveFilePanel("Save New Script", GetDefaultOutputPath(), _scriptName + ".cs", ".cs");
			if (!string.IsNullOrEmpty(path))
				GenerateScriptFromTemplate(path);
		}

		private static bool IsClassNameUnique(string fullName) {
			if (string.IsNullOrEmpty(fullName))
				throw new InvalidOperationException("An empty or null string was specified.");

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				foreach (var type in assembly.GetTypes())
					if (type.FullName == fullName)
						return false;
			return true;
		}

		private void ValidateScriptInputs() {
			// Ensure that valid script name was specified.
			if (!Regex.IsMatch(_scriptName, @"^[A-za-z_][A-za-z_0-9]*$")) {
				EditorUtility.DisplayDialog("Invalid Script Name", string.Format("'{0}' is not a valid type name.", _scriptName), "OK");
				return;
			}
			// If a namespace was specified, ensure that it is valid!
			if (!string.IsNullOrEmpty(_ns) && !Regex.IsMatch(_ns, @"^[A-za-z_][A-za-z_0-9]*(\.[A-za-z_][A-za-z_0-9]*)*$")) {
				EditorUtility.DisplayDialog("Invalid Namespace", string.Format("'{0}' is not a valid namespace.", _ns), "OK");
				return;
			}
		}

		private void GenerateScriptFromTemplate(string path) {
			// Ensure that input focus is removed from text field.
			EditorGUIUtility.keyboardControl = 0;
			EditorGUIUtility.editingTextField = false;

			// Ensure that path ends with '.cs'.
			if (!path.EndsWith(".cs")) {
				EditorUtility.DisplayDialog("Invalid File Extension", "Could not save script because the wrong file extension was specified.\n\nPlease ensure to save as '.cs' file.", "OK");
				return;
			}
			// Ensure that base directory actually exists.
			if (!Directory.Exists(Path.GetDirectoryName(path))) {
				EditorUtility.DisplayDialog("Invalid Path", "Could not save script because the specified directory does not exist.", "OK");
				return;
			}

			ValidateScriptInputs();

			string fullName = !string.IsNullOrEmpty(_ns)
				? _ns + "." + _scriptName
				: _scriptName;
		
			// Warn user if their type name is not unique.
			if (!IsClassNameUnique(fullName))
				if (!EditorUtility.DisplayDialog("Warning: Type Already Exists!", string.Format("A type already exists with the name '{0}'.\n\nIf you proceed then you will get compilation errors in the console window.", fullName), "Proceed", "Cancel"))
					return;

			// Generate source code.
			string sourceCode = _activeGenerator.GenerateScript(_scriptName, _ns);
			// Write to file!
			File.WriteAllText(path, sourceCode, Encoding.UTF8);

			// Unity should now recompile its scripts!
			AssetDatabase.Refresh();

			Repaint();
		}

		private void DoCopyToClipboard() {
			// Ensure that input focus is removed from text field.
			EditorGUIUtility.keyboardControl = 0;
			EditorGUIUtility.editingTextField = false;

			ValidateScriptInputs();

			EditorGUIUtility.systemCopyBuffer = _activeGenerator.GenerateScript(_scriptName, _ns);
		}

	}

}
