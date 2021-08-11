using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class MergedTool : Tool
	{
		public List<ITool> tools { get; } = new List<ITool>();

		public MergedTool()
		{
			label = "More";
			icon = PeekPlugin.Icons.more?[IconSize.Small];
			tooltip = "More";
			overlay = PeekPlugin.Icons.moreOverlay?[IconSize.Small];
		}

		public bool expandable { get; set; }

		public new string label
		{
			get => base.label;
			set => base.label = value;
		}

		public new string tooltip
		{
			get => base.tooltip;
			set => base.tooltip = value;
		}

		public new Texture2D icon
		{
			get => base.icon;
			set => base.icon = value;
		}

		public new Texture2D overlay
		{
			get => base.overlay;
			set => base.overlay = value;
		}

		public override bool isActive => false;

		public override void Open(ToolControl control)
		{
			var menu = new GenericMenu();
			menu.allowDuplicateNames = true;

			foreach (var tool in tools)
			{
				menu.AddItem(new GUIContent(tool.tooltip), false, () => tool.Open(control));
			}
			
			menu.DropDown(control.activatorGuiPosition);
		}

		public override void Close(ToolControl control) { }
	}
}