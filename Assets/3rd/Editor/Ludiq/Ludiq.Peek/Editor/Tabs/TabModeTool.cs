using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class TabModeTool : Tool
	{
		public override Texture2D icon
		{
			get
			{
				switch (PeekPlugin.Configuration.tabsMode)
				{
					case TabsMode.Pinned: return PeekPlugin.Icons.pinOn?[IconSize.Small];
					case TabsMode.Popup: return PeekPlugin.Icons.pin?[IconSize.Small];
					default: throw PeekPlugin.Configuration.tabsMode.Unexpected();
				}
			}
		}

		public override string tooltip => PeekPlugin.Configuration.tabsMode.DisplayName();

		public override bool isShortcuttable => false;

		public override void Open(ToolControl control)
		{
			if (PeekPlugin.Configuration.tabsMode == TabsMode.Pinned)
			{
				PeekPlugin.Configuration.tabsMode = TabsMode.Popup;
			}
			else if (PeekPlugin.Configuration.tabsMode == TabsMode.Popup)
			{
				PeekPlugin.Configuration.tabsMode = TabsMode.Pinned;
			}

			PeekPlugin.Configuration.Save();
		}
	}
}