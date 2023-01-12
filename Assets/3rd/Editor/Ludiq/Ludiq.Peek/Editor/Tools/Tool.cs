using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public abstract class Tool : ITool
	{
		public virtual string label { get; protected set; }

		public virtual string tooltip { get; protected set; }

		public virtual GUIStyle treeViewStyle => PeekStyles.treeViewTool;

		public virtual GUIStyle SceneViewStyle(bool closeLeft, bool closeRight) => PeekStyles.SceneViewTool(closeLeft, closeRight);

		public virtual Texture2D icon { get; protected set; }

		public virtual Texture2D preview => null;

		public virtual Vector2 iconSize { get; protected set; } = new Vector2(16, 16);

		public virtual Texture2D overlay { get; protected set; }
		
		public virtual bool isActive { get; protected set; }

		public virtual bool isDimmed => false;

		public virtual bool showText { get; set; } = false;

		public virtual bool isTransient => true;

		public virtual bool isShortcuttable => true;
		
		protected Tool() { }
		
		public virtual bool IsVisible(ToolbarControl control)
		{
			return true;
		}

		public abstract void Open(ToolControl control);

		public virtual void OpenContextual(ToolControl control) => Open(control);

		public virtual void Close(ToolControl control) { }

		public virtual void OnMove(ToolControl control) { }

		public virtual void OnGUI(ToolControl control)
		{

		}

		public virtual bool OnDragEntered(ToolControl control)
		{
			return false;
		}

		public virtual void OnDragExited(ToolControl control)
		{

		}

		public virtual bool OnDropEntered(ToolControl control)
		{
			return false;
		}

		public virtual void OnDropUpdated(ToolControl control)
		{

		}
		
		public virtual void OnDropExited(ToolControl control)
		{

		}
	}
}