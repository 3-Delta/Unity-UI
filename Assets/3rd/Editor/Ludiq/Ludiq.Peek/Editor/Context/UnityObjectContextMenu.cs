using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;
	
	public static class UnityObjectContextMenu
	{
		public static void Open(UnityObject[] targets, Rect activatorPosition)
		{
			UnityEditorDynamic.EditorUtility.DisplayObjectContextMenu(activatorPosition, targets, 0);
		}
	}
}
