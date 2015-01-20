// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for basic C# struct.
	/// </summary>
	[ScriptTemplate("C# Struct")]
	public sealed class StructTemplate : BasicTemplate {

		/// <summary>
		/// Initialize new <see cref="StructTemplate"/> instance.
		/// </summary>
		public StructTemplate()
			: base("struct") {
		}

	}

}
