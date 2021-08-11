using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public interface ITool
	{
		string label { get; }

		string tooltip { get; }

		Texture2D icon { get; }

		Texture2D preview { get; }

		Vector2 iconSize { get; }

		Texture2D overlay { get; }

		GUIStyle treeViewStyle { get; }

		GUIStyle SceneViewStyle(bool closeLeft, bool closeRight);

		bool isActive { get; }

		bool isDimmed { get; }

		bool showText { get; }
		
		bool isTransient { get; }

		bool isShortcuttable { get; }

		bool IsVisible(ToolbarControl toolbarControl);

		void Open(ToolControl control);

		void OpenContextual(ToolControl control);

		void Close(ToolControl control);

		void OnMove(ToolControl control);
		
		void OnGUI(ToolControl control);

		bool OnDragEntered(ToolControl control);
		
		void OnDragExited(ToolControl control);

		bool OnDropEntered(ToolControl control);

		void OnDropUpdated(ToolControl control);

		void OnDropExited(ToolControl control);
	}
}