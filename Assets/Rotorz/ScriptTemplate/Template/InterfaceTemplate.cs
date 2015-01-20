// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

namespace ScriptTemplates {

	/// <summary>
	/// Template generator for basic C# interface.
	/// </summary>
	[ScriptTemplate("C# Interface")]
	public sealed class InterfaceTemplate : BasicTemplate {

		/// <summary>
		/// Initialize new <see cref="InterfaceTemplate"/> instance.
		/// </summary>
		public InterfaceTemplate()
			: base("interface") {
		}

	}

}
