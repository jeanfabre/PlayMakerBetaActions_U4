// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for basic C# class.
	/// </summary>
	[ScriptTemplate("C# Class")]
	public sealed class ClassTemplate : BasicTemplate {

		/// <summary>
		/// Initialize new <see cref="ClassTemplate"/> instance.
		/// </summary>
		public ClassTemplate() : base("class") {
		}

	}

}
