using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_4_0), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_4_0f2), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_4_0f3), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_4_0f5), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_4_0f6), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_4_0f7), LudiqCore.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_4_0f9), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_1_4_0 : PluginChangelog
	{
		public Changelog_1_4_0(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.4.0";
		public override DateTime date => new DateTime(2018, 06, 12);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Approximate string matching in the fuzzy finder";
				yield return "[Added] Preserve attributes to AOT stubs script";
				yield return "[Optimized] Search speed and responsiveness";
				yield return "[Changed] Configuration panel layout";
				yield return "[Fixed] Pale icons in Unity 2018";
				yield return "[Fixed] Configuration panel display in Unity 2018";
				yield return "[Fixed] Version control file lock";
				yield return "[Fixed] AOT Pre-build looking in unused scenes";
				yield return "[Fixed] AOT Pre-build with multiple generic parameters";
				yield return "[Fixed] Various multithreading issues in the editor";
				yield return "[Fixed] List inspector in Unity 2018";
				yield return "[Fixed] .NET 4.x DLL import";
				yield return "[Fixed] Generic methods being preferred during reflection";
				yield return "[Fixed] Equality comparison between null and non-nullable types";
			}
		}
	}
	
	internal class Changelog_1_4_0f2 : PluginChangelog
	{
		public Changelog_1_4_0f2(Plugin plugin) : base(plugin) { }
	
		public override SemanticVersion version => "1.4.0f2";
		public override DateTime date => new DateTime(2018, 07, 13);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Reorderable list control textures on linear color space";
			}
		}
	}
	
	internal class Changelog_1_4_0f3 : PluginChangelog
	{
		public Changelog_1_4_0f3(Plugin plugin) : base(plugin) { }
	
		public override SemanticVersion version => "1.4.0f3";
		public override DateTime date => new DateTime(2018, 07, 31);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Enum inspectors not instantiating value";
			}
		}
	}
	
	internal class Changelog_1_4_0f5 : PluginChangelog
	{
		public Changelog_1_4_0f5(Plugin plugin) : base(plugin) { }
	
		public override SemanticVersion version => "1.4.0f5";
		public override DateTime date => new DateTime(2018, 08, 14);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Regression in AOT stubs generation for extension methods";
				yield return "[Fixed] Editor application events not being sent to out of focus windows";
				yield return "[Fixed] Editor application events being sent before plugin container had initialized";
				yield return "[Fixed] JIT support detection for standalone IL2CPP builds";
			}
		}
	}
	
	internal class Changelog_1_4_0f6 : PluginChangelog
	{
		public Changelog_1_4_0f6(Plugin plugin) : base(plugin) { }
	
		public override SemanticVersion version => "1.4.0f6";
		public override DateTime date => new DateTime(2018, 09, 06);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] UnityEngine.Resources to default types";
				yield return "[Changed] Increased default runtime max recursion depth to 100";
				yield return "[Improved] Fuzzy finder population and validation";
				yield return "[Fixed] Serialized property providers typeset";
				yield return "[Fixed] Namespace conflicts in AOT stubs by using global type references";
				yield return "[Fixed] Unity version string parsing on experimental builds";
				yield return "[Fixed] Attribute cache aborting when attribute constructors threw an error";
				yield return "[Fixed] Changelog page accessing editor styles before OnGUI";
			}
		}
	}
	
	internal class Changelog_1_4_0f7 : PluginChangelog
	{
		public Changelog_1_4_0f7(Plugin plugin) : base(plugin) { }
	
		public override SemanticVersion version => "1.4.0f7";
		public override DateTime date => new DateTime(2018, 09, 25);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Memory leak caused by recursion detection not freeing pooled instance";
				yield return "[Fixed] Error when opening the configuration window on Unity 2018.3 beta";
			}
		}
	}

	internal class Changelog_1_4_0f9 : PluginChangelog
	{
		public Changelog_1_4_0f9(Plugin plugin) : base(plugin) { }
	
		public override SemanticVersion version => "1.4.0f9";
		public override DateTime date => new DateTime(2018, 10, 11);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Improved] Within GUI detection with internal GUI depth property";
			}
		}
	}
}