using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ludiq.PeekCore.CodeDom;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class SerializedPropertyProviderProvider : SingleDecoratorProvider<Type, ISerializedPropertyProvider, RegisterSerializedPropertyProviderAttribute>
	{
		protected override ISerializedPropertyProvider CreateDecorator(Type providerType, Type type)
		{
			var targetObject = ScriptableObject.CreateInstance(providerType);
			targetObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave; // HideAndDontSave would define NotEditable
			return (ISerializedPropertyProvider)targetObject;
		}

		protected override IEnumerable<Assembly> registrationAssemblies => Codebase.ludiqAssemblies;

		protected override bool cache => false;

		protected override Type GetDecoratedType(Type decorated)
		{
			return decorated;
		}

		static SerializedPropertyProviderProvider()
		{
			instance = new SerializedPropertyProviderProvider();
		}

		public static SerializedPropertyProviderProvider instance { get; private set; }

		public void GenerateProviderScripts()
		{
			if (Directory.Exists(LudiqCore.Paths.propertyProviders))
			{
				foreach (var file in Directory.GetFiles(LudiqCore.Paths.propertyProviders))
				{
					File.Delete(file);
				}
			}

			if (Directory.Exists(LudiqCore.Paths.propertyProvidersEditor))
			{
				foreach (var file in Directory.GetFiles(LudiqCore.Paths.propertyProvidersEditor))
				{
					File.Delete(file);
				}
			}

			PathUtility.CreateDirectoryIfNeeded(LudiqCore.Paths.propertyProviders);
			PathUtility.CreateDirectoryIfNeeded(LudiqCore.Paths.propertyProvidersEditor);

			foreach (var type in Codebase.ludiqTypes.Where(SerializedPropertyUtility.HasCustomDrawer))
			{ 
				var directory = Codebase.IsEditorType(type) ? LudiqCore.Paths.propertyProvidersEditor : LudiqCore.Paths.propertyProviders;
				var path = Path.Combine(directory, GetProviderScriptName(type) + ".cs");
				
				VersionControlUtility.Unlock(path);
				File.WriteAllText(path, GenerateProviderSource(type));
			}

			AssetDatabase.Refresh();
		}

		private static string GetProviderScriptName(Type type)
		{
			// The file name has to match the class name for Unity
			return "PropertyProvider_" + type.CSharpFullName().Replace(".", "_");
		}

		private static string GenerateProviderSource(Type type)
		{
			/* Example: 
			
			namespace Ludiq.PeekCore.Generated.PropertyProviders
			{
				[Ludiq.SerializedPropertyProvider(typeof(MyNamespace.MyType))]
				public class MyNamespace_MyType : SerializedPropertyProvider<MyNamespace.MyType> { }
			}
			
			*/

			Ensure.That(nameof(type)).IsNotNull(type);

			var unit = new CodeCompileUnit();

			var @namespace = new CodeNamespace("Ludiq.Generated.PropertyProviders");

			unit.Namespaces.Add(@namespace);

			var @class = new CodeClassTypeDeclaration(CodeMemberModifiers.Public, GetProviderScriptName(type));

			@class.BaseTypes.Add(Code.TypeRef(typeof(SerializedPropertyProvider<>).MakeGenericType(type)));

			var serializedPropertyProviderAttribute = new CodeAttributeDeclaration(Code.TypeRef(typeof(RegisterSerializedPropertyProviderAttribute), true), new[] { new CodeAttributeArgument(new CodeTypeOfExpression(new CodeTypeReference(type, true))) });

			@class.CustomAttributes.Add(serializedPropertyProviderAttribute);

			@namespace.Types.Add(@class);

			using (var stringWriter = new StringWriter())
			{
				CodeGenerator.GenerateCodeFromCompileUnit(unit, new TextCodeWriter(stringWriter), new CodeGeneratorOptions(indentString: "\t"));
				return stringWriter.ToString();
			}
		}
	}
}