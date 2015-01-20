// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScriptTemplates {

	/// <summary>
	/// Describes script template generator.
	/// </summary>
	public sealed class ScriptGeneratorDescriptor {

		private static List<ScriptGeneratorDescriptor> s_Descriptors;
		private static ReadOnlyCollection<ScriptGeneratorDescriptor> s_DescriptorsReadOnly;

		static ScriptGeneratorDescriptor() {
			// Gather script template generator types.
			s_Descriptors = new List<ScriptGeneratorDescriptor>();
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				foreach (var type in assembly.GetTypes())
					if (type.IsDefined(typeof(ScriptTemplateAttribute), true))
						s_Descriptors.Add(new ScriptGeneratorDescriptor() {
							Type = type,
							Attribute = (ScriptTemplateAttribute)type.GetCustomAttributes(typeof(ScriptTemplateAttribute), true).First()
						});

			// Sort descriptor by priority!
			s_Descriptors.Sort((a, b) => a.Attribute.Priority - b.Attribute.Priority);

			// We only want to expose read-only access to the collection ;)
			s_DescriptorsReadOnly = new ReadOnlyCollection<ScriptGeneratorDescriptor>(s_Descriptors);
		}

		/// <summary>
		/// Gets read-only collection of script template descriptors.
		/// </summary>
		public static IList<ScriptGeneratorDescriptor> Descriptors {
			get { return s_DescriptorsReadOnly; }
		}

		/// <summary>
		/// Type of template generator.
		/// </summary>
		public Type Type { get; private set; }
		/// <summary>
		/// Associated attribute which includes description of template generator.
		/// </summary>
		public ScriptTemplateAttribute Attribute { get; private set; }

		/// <summary>
		/// Initialize new <see cref="ScriptGeneratorDescriptor"/> instance.
		/// </summary>
		private ScriptGeneratorDescriptor() {
		}

		/// <summary>
		/// Create new instance of script generator.
		/// </summary>
		/// <returns>
		/// The new <see cref="ScriptTemplateGenerator"/> instance.
		/// </returns>
		public ScriptTemplateGenerator CreateInstance() {
			return Activator.CreateInstance(Type) as ScriptTemplateGenerator;
		}

	}

}
