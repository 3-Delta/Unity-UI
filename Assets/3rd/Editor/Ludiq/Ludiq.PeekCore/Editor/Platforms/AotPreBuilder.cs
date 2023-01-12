using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ludiq.PeekCore.CodeDom;
using UnityEngine;
using UnityEngine.Scripting;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class AotPreBuilder
	{
		// Automatically generates the link.xml file to prevent stripping.
		// Currently only used for plugin assemblies, because blanket preserving 
		// all setting assemblies sometimes causes the IL2CPP process to fail. 
		// For settings assemblies, the AOT stubs are good enough to fool
		// the static code analysis without needing this full coverage.
		// https://docs.unity3d.com/Manual/iphone-playerSizeOptimization.html
		// However, for FullSerializer, we need to preserve our custom assemblies.
		// This is mostly because IL2CPP will attempt to transform non-public
		// property setters used in deserialization into read-only accessors
		// that return false on PropertyInfo.CanWrite, but only in stripped builds.
		// Therefore, in stripped builds, FS will skip properties that should be
		// deserialized without any error (and that took hours of debugging to figure out).
		public static void GenerateLinker(string path)
		{
			var linker = new XDocument();

			var linkerNode = new XElement("linker");

			foreach (var pluginAssembly in PluginContainer.plugins
														  .SelectMany(plugin => plugin.GetType()
																					  .GetAttributes<PluginRuntimeAssemblyAttribute>()
																					  .Select(a => a.assemblyName))
														  .Distinct())
			{
				var assemblyNode = new XElement("assembly");
				var fullnameAttribute = new XAttribute("fullname", pluginAssembly);
				var preserveAttribute = new XAttribute("preserve", "all");
				assemblyNode.Add(fullnameAttribute);
				assemblyNode.Add(preserveAttribute);
				linkerNode.Add(assemblyNode);
			}

			linker.Add(linkerNode);

			PathUtility.CreateDirectoryIfNeeded(LudiqCore.Paths.transientGenerated);
			
			VersionControlUtility.Unlock(path);

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			// Using ToString instead of Save to omit the <?xml> declaration,
			// which doesn't appear in the Unity documentation page for the linker.
			File.WriteAllText(path, linker.ToString());
		}

		public static IEnumerable<object> FindAllProjectStubs()
		{
			// Plugins
			
			foreach (var pluginStub in FindAllPluginStubs())
			{
				yield return pluginStub;
			}

			// Assets
			
			foreach (var assetStub in FindAllAssetStubs())
			{
				yield return assetStub;
			}
		}

		private static IEnumerable<object> FindAllPluginStubs()
		{
			return PluginContainer.plugins.SelectMany(p => p.aotStubs);
		}

		private static IEnumerable<object> FindAllAssetStubs()
		{
			return UnityAPI.Await(() => LinqUtility.Concat<object>
			(
				AssetUtility.FindAllAssetsOfType<IAotStubbable>()
				            .SelectMany(aot => aot.aotStubs),

				AssetUtility.FindAllAssetsOfType<GameObject>()
				            .SelectMany(go => go.GetComponents<IAotStubbable>()
				            .SelectMany(component => component.aotStubs))
			).ToArray());
		}

		public static IEnumerable<object> FindAllSceneStubs()
		{
			return UnityObjectUtility.FindObjectsOfTypeInAllScenes<IAotStubbable>()
									 .SelectMany(aot => aot.aotStubs);
		}

		public static void GenerateStubScript(string scriptPath, IEnumerable<object> stubs)
		{
			Ensure.That(nameof(stubs)).IsNotNull(stubs);

			var stubWriters = stubs.Select(s => AotStubWriterProvider.instance.GetDecorator(s)).ToHashSet();

			var unit = new CodeCompileUnit();
			unit.StartDirectives.Add(new CodePragmaWarningDirective(CodePragmaWarningSetting.Disable, new[] { 219 })); // Disable unused variable warning

			var @namespace = new CodeNamespace("Ludiq.Generated.Aot");

			unit.Namespaces.Add(@namespace);

			var @class = new CodeClassTypeDeclaration(CodeMemberModifiers.Public, "AotStubs");

			@class.CustomAttributes.Add(new CodeAttributeDeclaration(Code.TypeRef(typeof(PreserveAttribute))));

			@namespace.Types.Add(@class);

			var usedMethodNames = new HashSet<string>();

			foreach (var stubWriter in stubWriters.OrderBy(sw => sw.stubMethodComment))
			{
				if (stubWriter.skip)
				{
					continue;
				}

				var methodName = stubWriter.stubMethodName;

				var i = 0;

				while (usedMethodNames.Contains(methodName))
				{
					methodName = stubWriter.stubMethodName + "_" + i++;
				}

				usedMethodNames.Add(methodName);

				var method = new CodeMethodMember(CodeMemberModifiers.Public | CodeMemberModifiers.Static, Code.TypeRef(typeof(void)), methodName, Enumerable.Empty<CodeParameterDeclaration>(), stubWriter.GetStubStatements().ToArray());
				method.CustomAttributes.Add(new CodeAttributeDeclaration(Code.TypeRef(typeof(PreserveAttribute), true)));
				method.Comments.Add(new CodeComment(stubWriter.stubMethodComment));

				@class.Members.Add(method);
			}

			PathUtility.CreateDirectoryIfNeeded(LudiqCore.Paths.transientGenerated);
			
			VersionControlUtility.Unlock(scriptPath);

			if (File.Exists(scriptPath))
			{
				File.Delete(scriptPath);
			}

			using (var scriptWriter = new StreamWriter(scriptPath))
			{
				CodeGenerator.GenerateCodeFromCompileUnit(unit, new TextCodeWriter(scriptWriter), new CodeGeneratorOptions(indentString: "\t"));
			}
		}
	}
}