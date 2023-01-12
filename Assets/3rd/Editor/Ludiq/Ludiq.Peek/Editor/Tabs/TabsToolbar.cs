using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class TabsToolbar : Toolbar
	{
		private readonly TabModeTool modeTool;

		public TabsToolbar() : base()
		{
			mainTool = modeTool = new TabModeTool();
		}

		private readonly LazyDictionary<Type, TabTool> tabTools = new LazyDictionary<Type, TabTool>(t => new TabTool(t));

		protected override void UpdateTools(IList<ITool> tools)
		{
			tools.Add(modeTool);

			var tabTypes = ListPool<Type>.New();

			try
			{
				foreach (var tabInLayout in PeekPlugin.Configuration.tabsInLayout)
				{
					if (!Codebase.TryDeserializeType(tabInLayout, out var tabType))
					{
						continue;
					}

					if (PeekPlugin.Configuration.tabsBlacklist.Contains(tabType))
					{
						continue;
					}

					if (!tabTypes.Contains(tabType))
					{
						tabTypes.Add(tabType);
					}
				}

				foreach (var tabType in PeekPlugin.Configuration.tabsWhitelist)
				{
					if (tabType == null)
					{
						continue;
					}

					if (!tabTypes.Contains(tabType))
					{
						tabTypes.Add(tabType);
					}
				}

				tabTypes.Sort(compareTabTypes);

				foreach (var tabType in tabTypes)
				{
					var tabTool = tabTools[tabType];

					tools.Add(tabTool);
				}
			}
			finally
			{
				tabTypes.Free();
			}
		}
		
		private static readonly Comparison<Type> compareTabTypes = CompareTabTypes;

		private static int CompareTabTypes(Type a, Type b)
		{
			var ia = PeekPlugin.Configuration.tabsOrder.IndexOf(a);
			var ib = PeekPlugin.Configuration.tabsOrder.IndexOf(b);

			// No specific order override for either => ignore
			if (ia < 0 && ib < 0)
			{
				return 0;
			}

			// Override for A, but not for B => A comes first
			if (ia >= 0 && ib < 0)
			{
				return -1;
			}

			// Override for B, but not for A => B comes first
			if (ia < 0 && ib >= 0)
			{
				return +1;
			}

			// Overrides for both => Compare indices
			return ia.CompareTo(ib);
		}
	}
}