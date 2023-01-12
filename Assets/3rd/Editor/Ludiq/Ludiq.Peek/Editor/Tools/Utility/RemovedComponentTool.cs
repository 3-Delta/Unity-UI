using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class RemovedComponentTool : Tool
	{
		public RemovedComponent target { get; }

		public RemovedComponentTool(RemovedComponent target) : base()
		{
			this.target = target;

			label = target.assetComponent.name;
			icon = target.assetComponent.Icon()?[IconSize.Small];
			tooltip = target.assetComponent.GetType().DisplayName() + " (Removed)";

			overlay = PeekPlugin.Icons.prefabOverlayRemoved?[IconSize.Small];
		}

		public override bool isDimmed => true;

		public override bool isActive => false;

		public override void Open(ToolControl control)
		{
			var menu = new GenericMenu();

			menu.AddItem(new GUIContent("Apply"), false, () => target.Apply());
			menu.AddItem(new GUIContent("Revert"), false, () => target.Revert());

			menu.ShowAsContext();
		}

		public override void Close(ToolControl control) { }
	}
}