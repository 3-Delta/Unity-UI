using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public struct DelayedTooltip
	{
		public Rect screenPosition;

		public GUIContent content;

		public GUIStyle style;
	}
}