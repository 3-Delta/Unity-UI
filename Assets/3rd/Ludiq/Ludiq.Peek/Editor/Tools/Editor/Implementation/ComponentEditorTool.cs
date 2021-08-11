using Ludiq.Peek;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: RegisterEditorTool(typeof(Component), typeof(ComponentEditorTool<>))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class ComponentEditorTool<TComponent> : EditorTool<TComponent> where TComponent : Component
	{
		public ComponentEditorTool(TComponent[] targets) : base(targets)
		{
			_tooltip = typeof(TComponent).DisplayName();
		}

		private string _tooltip;
		
		public override string tooltip
		{
			get => _tooltip;
			protected set => base.tooltip = value;
		}

		public override Texture2D overlay
		{
			get
			{
				if (hasSingleTarget && PrefabUtility.IsAddedComponentOverride(target))
				{
					overlay = PeekPlugin.Icons.prefabOverlayAdded?[IconSize.Small];
				}

				return base.overlay;
			}
		}

		public override bool isDimmed => hasSingleTarget && !target.IsEnabled();
	}
}