// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;

namespace ScriptTemplates {

	/// <summary>
	/// Indiciates visiblity of a type.
	/// </summary>
	public enum TypeVisibility {
		Public,
		Private,
		Internal,
	}

	/// <summary>
	/// Base class for a script template generator.
	/// </summary>
	/// <remarks>
	/// <para>Custom template generators must be marked with the <see cref="ScriptTemplateAttribute"/>
	/// so that they can be used within the "Create Script" window.</para>
	/// </remarks>
	[InitializeOnLoad]
	public abstract class ScriptTemplateGenerator {
		
		static ScriptTemplateGenerator() {
			LoadSharedPreferences();
		}

		#region Shared Preferences

		private static bool s_BracesOnNewLine;

		private static bool s_EditorScript;
		private static bool s_InitializeOnLoad;
		private static bool s_StaticConstructor;

		private static TypeVisibility s_TypeVisibility;
		private static bool s_StaticClass;
		private static bool s_PartialClass;

		private static void LoadSharedPreferences() {
			s_BracesOnNewLine = EditorPrefs.GetBool("ScriptTemplates.Shared.BracesOnNewLine", false);

			s_EditorScript = EditorPrefs.GetBool("ScriptTemplates.Shared.EditorScript", false);
			s_InitializeOnLoad = EditorPrefs.GetBool("ScriptTemplates.Shared.InitializeOnLoad", false);
			s_StaticConstructor = EditorPrefs.GetBool("ScriptTemplates.Shared.StaticConstructor", false);

			s_TypeVisibility = (TypeVisibility)EditorPrefs.GetInt("ScriptTemplates.Shared.TypeVisibility", (int)TypeVisibility.Public);
			s_StaticClass = EditorPrefs.GetBool("ScriptTemplates.Shared.StaticClass", false);
			s_PartialClass = EditorPrefs.GetBool("ScriptTemplates.Shared.PartialClass", false);
		}

		/// <summary>
		/// Gets or sets whether opening brace character '{' should begin on a new line.
		/// </summary>
		protected static bool BracesOnNewLine {
			get { return s_BracesOnNewLine; }
			set {
				if (value != s_BracesOnNewLine)
					EditorPrefs.SetBool("ScriptTemplates.Shared.BracesOnNewLine", s_BracesOnNewLine = value);
			}
		}

		/// <summary>
		/// When applicable indicates whether script should be for editor usage.
		/// </summary>
		protected static bool IsEditorScript {
			get { return s_EditorScript; }
			set {
				if (value != s_EditorScript) {
					EditorPrefs.SetBool("ScriptTemplates.Shared.EditorScript", s_EditorScript = value);
					if (!value)
						OutputInitializeOnLoad = false;
				}
			}
		}
		/// <summary>
		/// Indicates that output class should be marked with <c>InitializeOnLoad</c> attribute.
		/// </summary>
		protected static bool OutputInitializeOnLoad {
			get { return s_InitializeOnLoad; }
			set {
				if (value != s_InitializeOnLoad)
					EditorPrefs.SetBool("ScriptTemplates.Shared.InitializeOnLoad", s_InitializeOnLoad = value);
			}
		}
		/// <summary>
		/// Indicates if static constructor should be added to output class.
		/// </summary>
		protected static bool OutputStaticConstructor {
			get { return s_StaticConstructor; }
			set {
				if (value != s_StaticConstructor) {
					EditorPrefs.SetBool("ScriptTemplates.Shared.StaticConstructor", s_StaticConstructor = value);
					if (!value)
						OutputInitializeOnLoad = false;
				}
			}
		}

		/// <summary>
		/// Visibility for output type.
		/// </summary>
		protected static TypeVisibility TypeVisibility {
			get { return s_TypeVisibility; }
			set {
				if (value != s_TypeVisibility)
					EditorPrefs.SetInt("ScriptTemplates.Shared.TypeVisibility", (int)(s_TypeVisibility = value));
			}
		}
		/// <summary>
		/// Indicates whether output class should be marked as static.
		/// </summary>
		protected static bool StaticClass {
			get { return s_StaticClass; }
			set {
				if (value != s_StaticClass) {
					EditorPrefs.SetBool("ScriptTemplates.Shared.StaticClass", s_StaticClass = value);
					if (value)
						PartialClass = false;
				}
			}
		}
		/// <summary>
		/// Indicates whether output class should be marked as partial.
		/// </summary>
		protected static bool PartialClass {
			get { return s_PartialClass; }
			set {
				if (value != s_PartialClass) {
					EditorPrefs.SetBool("ScriptTemplates.Shared.PartialClass", s_PartialClass = value);
					if (value)
						StaticClass = false;
				}
			}
		}

		/// <summary>
		/// Gets characters for opening brace insertion.
		/// </summary>
		protected static string OpeningBraceInsertion {
			get { return BracesOnNewLine ? "\n{" : " {"; }
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether an editor script will be generated.
		/// </summary>
		public abstract bool WillGenerateEditorScript { get; }

		/// <summary>
		/// Draws interface and handles GUI events allowing user to provide additional inputs.
		/// </summary>
		public virtual void OnGUI() { }

		/// <summary>
		/// Draws interface and handles GUI events for standard option inputs.
		/// </summary>
		public void OnStandardGUI() {
			BracesOnNewLine = EditorGUILayout.ToggleLeft("Places braces on new lines", BracesOnNewLine);
		}

		/// <summary>
		/// Create new <see cref="ScriptBuilder"/> instance to build script.
		/// </summary>
		/// <returns>
		/// The <see cref="ScriptBuilder"/> instance.
		/// </returns>
		public ScriptBuilder CreateScriptBuilder() {
			return new ScriptBuilder();
		}

		/// <summary>
		/// Generate C# source code for template.
		/// </summary>
		/// <param name="scriptName">Name of script.</param>
		/// <param name="ns">Namespace for new script.</param>
		/// <returns>
		/// The generated source code.
		/// </returns>
		/// <seealso cref="CreateScriptBuilder"/>
		public abstract string GenerateScript(string scriptName, string ns);

	}

}
