// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;

namespace ScriptTemplates {

	/// <summary>
	/// Marks class as script template.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	public sealed class ScriptTemplateAttribute : Attribute {

		/// <summary>
		/// Gets description of script template.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Gets priority of template generator providing control over ordering in user interfaces.
		/// </summary>
		public int Priority { get; private set; }

		/// <summary>
		/// Initializes a new <see cref="ScriptTemplateAttribute"/> instance.
		/// </summary>
		/// <param name="description">Description of script template.</param>
		/// <param name="priority">Provides some control over ordering in user interfaces</param>
		public ScriptTemplateAttribute(string description, int priority = 1000) {
			Description = description;
			Priority = priority;
		}

	}

}
