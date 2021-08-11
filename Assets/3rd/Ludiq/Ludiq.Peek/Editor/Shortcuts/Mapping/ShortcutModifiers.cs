using System;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	[Flags]
	public enum ShortcutModifiers
	{
		None = 0,

		Action = 1 << 0,

		Shift = 1 << 1,

		Alt = 1 << 2
	}

	public static class XShortcutModifiers
	{
		public static bool HasFlagFast(this ShortcutModifiers modifiers, ShortcutModifiers flag)
		{
			return (modifiers & flag) != 0;
		}

		public static bool HasAction(this ShortcutModifiers modifiers) => modifiers.HasFlagFast(ShortcutModifiers.Action);

		public static bool HasShift(this ShortcutModifiers modifiers) => modifiers.HasFlagFast(ShortcutModifiers.Shift);

		public static bool HasAlt(this ShortcutModifiers modifiers) => modifiers.HasFlagFast(ShortcutModifiers.Alt);

		public static EventModifiers ToEventModifiers(this ShortcutModifiers modifiers)
		{
			var em = default(EventModifiers);

			if (modifiers.HasAction())
			{
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					em |= EventModifiers.Command;
				}
				else
				{
					em |= EventModifiers.Control;
				}
			}

			if (modifiers.HasShift())
			{
				em |= EventModifiers.Shift;
			}

			if (modifiers.HasAlt())
			{
				em |= EventModifiers.Alt;
			}

			return em;
		}
	}
}