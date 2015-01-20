// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ScriptTemplates {

	/// <summary>
	/// Helps you to build script files with support for automatically indenting source code.
	/// </summary>
	public sealed class ScriptBuilder {

		private StringBuilder _sb = new StringBuilder();

		private int _indentLevel = 0;
		private string _indent = "";
		private string _indentChars = "\t";
		private string _indentCharsNewLine = "\n";

		/// <summary>
		/// Gets or sets sequence of characters to use when indenting text.
		/// </summary>
		/// <remarks>
		/// <para>Changing this value will not affect text which has already been appended.</para>
		/// </remarks>
		public string IndentChars {
			get { return _indentChars; }
			set {
				int restoreIndent = _indentLevel;
				_indentLevel = -1;
				_indentChars = value;
				IndentLevel = restoreIndent;
			}
		}

		/// <summary>
		/// Gets or sets current indent level within script.
		/// </summary>
		public int IndentLevel {
			get { return _indentLevel; }
			set {
				value = Mathf.Max(0, value);
				if (value != _indentLevel) {
					if (value < _indentLevel)
						_sb.Length -= _indentChars.Length;

					_indentLevel = value;
					_indent = "";
					for (int i = 0; i < value; ++i)
						_indent += _indentChars;

					_indentCharsNewLine = "\n" + _indent;
				}
			}
		}

		/// <summary>
		/// Clear output and start over.
		/// </summary>
		/// <remarks>
		/// <para>This method also resets indention to 0.</para>
		/// </remarks>
		public void Clear() {
			_sb.Length = 0;
			IndentLevel = 0;
		}

		/// <summary>
		/// Append text to script and begin new line.
		/// </summary>
		/// <param name="text">Text.</param>
		public void AppendLine(string text) {
			Append(text);
			_sb.Append(_indentCharsNewLine);
		}

		/// <summary>
		/// Append blank line to script.
		/// </summary>
		public void AppendLine() {
			_sb.Append(_indentCharsNewLine);
		}

		/// <summary>
		/// Append text to script.
		/// </summary>
		/// <param name="text">Text.</param>
		public void Append(string text) {
			_sb.Append(text.Replace("\n", _indentCharsNewLine));
		}

		/// <summary>
		/// Begin namespace scope and automatically indent.
		/// </summary>
		/// <param name="text">Text.</param>
		public void BeginNamespace(string text) {
			Append(text);
			_sb.AppendLine();
			++IndentLevel;
			_sb.Append(_indentCharsNewLine);
		}

		/// <summary>
		/// End namespace scope and unindent.
		/// </summary>
		/// <param name="text">Text.</param>
		public void EndNamespace(string text) {
			--IndentLevel;
			_sb.Append(text.Replace("\n", _indentCharsNewLine));
			_sb.AppendLine();
			_sb.Append(_indent);
			AppendLine();
		}

		/// <summary>
		/// Get generated source code as string.
		/// </summary>
		/// <returns>
		/// The string.
		/// </returns>
		public override string ToString() {
			return _sb.ToString().Trim() + "\n";
		}

	}

}
