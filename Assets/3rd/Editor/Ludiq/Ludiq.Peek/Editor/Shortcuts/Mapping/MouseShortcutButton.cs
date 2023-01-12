using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public enum MouseShortcutButton
	{
		Left = 0,

		Middle = 1,

		Right = 2
	}

	public static class XMouseShortcutButton
	{
		public static int ToInt(this MouseShortcutButton button)
		{
			return (int)button;
		}
	}
}