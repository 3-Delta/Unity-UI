using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_2_0_0a1), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_2_0_0a3), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_2_0_0a4), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_2_0_0a5), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_2_0_0a1 : PluginChangelog
	{
		public Changelog_2_0_0a1(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "2.0.0a1";
		public override DateTime date => new DateTime(2018, 11, 07);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Odin Serializer Dependency";
				yield return "[Added] Layout Swapping Hotkeys (Ctrl/Cmd+Space)";
				yield return "[Added] Unity Message Proxies";
				yield return "[Added] VectorXInt Support";
				yield return "[Changed] Folder Structure";
			}
		}
	}
	
	internal class Changelog_2_0_0a3 : PluginChangelog
	{
		public Changelog_2_0_0a3(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "2.0.0a3";
		public override DateTime date => new DateTime(2018, 12, 21);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Breadcrumbs to fuzzy window";
				yield return "[Added] Category search in fuzzy window";
				yield return "[Added] Script reference resolution for all Bolt 1 scripts";
				yield return "[Added] Options panel to Extractor";
				yield return "[Added] Nested types inclusion for hierarchical type extraction";
				yield return "[Added] Nested type icons now use fallback to their parent type";
				yield return "[Changed] Merged AOT Pre-Build in Generation process";
				yield return "[Optimized] General search and option fetching speed";
				yield return "[Fixed] Fuzzy Finder rendering glitches";
			}
		}
	}
	
	internal class Changelog_2_0_0a4 : PluginChangelog
	{
		public Changelog_2_0_0a4(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "2.0.0a4";
		public override DateTime date => new DateTime(2019, 04, 03);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Odin Serializer to replace FullSerializer";
				yield return "[Added] Generics, Indexers and Events to core reflection library";
				yield return "[Added] Integration with new Project Settings and Preferences windows";
				yield return "[Added] Support for multiple icons and separators in fuzzy finder";
				yield return "[Optimized] Plugin container initialization";
				yield return "[Optimized] Core reflection library";
				yield return "[Optimized] Fuzzy finder options population";
				yield return "[Fixed] XML Documentation that included tags";
			}
		}
	}
	
	internal class Changelog_2_0_0a5 : PluginChangelog
	{
		public Changelog_2_0_0a5(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "2.0.0a5";
		public override DateTime date => new DateTime(2019, 05, 03);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Support for gradients";
				yield return "[Added] Separators in type option tree root";
				yield return "[Added] Editor preference to display obsolete options";
				yield return "[Fixed] Odin Serializer dependency error in Unity 2019";
				yield return "[Fixed] Object inspector not displaying generic argument selectors";
				yield return "[Fixed] Code generation error when casting negative numeric primitives";
				yield return "[Fixed] Code generation error when using identifiers starting with illegal characters such as numbers";
				yield return "[Fixed] Various AOT stubs generation errors";
				yield return "[Fixed] Various conversion and instantiation errors related to open-constructed generic types";
				yield return "[Fixed] Various inspector and accessor related issues";
				yield return "[Fixed] Unused variable warnings in generated code";
				yield return "[Fixed] Legacy AOT collections failing to deserialize with Odin";
			}
		}
	}
}