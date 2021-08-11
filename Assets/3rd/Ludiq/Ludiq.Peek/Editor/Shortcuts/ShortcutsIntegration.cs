#define PEEK_PRIMARY_SHORTCUTS // Alt+[0-9]
#define PEEK_SECONDARY_SHORTCUTS // Alt+Shift+[0-9]

using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.ShortcutManagement;
using UnityShortcutModifiers = UnityEditor.ShortcutManagement.ShortcutModifiers;
#endif

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class ShortcutsIntegration
	{
		public static ToolbarControl primaryToolbar { get; set; }

		public static ToolbarControl secondaryToolbar { get; set; }

#if PEEK_PRIMARY_SHORTCUTS

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 1", KeyCode.Alpha1, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 1 &1")]
#endif
		private static void PrimaryShortcut1()
		{
			primaryToolbar?.TriggerShortcut(1);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 2", KeyCode.Alpha2, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 2 &2")]
#endif
		private static void PrimaryShortcut2()
		{
			primaryToolbar?.TriggerShortcut(2);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 3", KeyCode.Alpha3, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 3 &3")]
#endif
		private static void PrimaryShortcut3()
		{
			primaryToolbar?.TriggerShortcut(3);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 4", KeyCode.Alpha4, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 4 &4")]
#endif
		private static void PrimaryShortcut4()
		{
			primaryToolbar?.TriggerShortcut(4);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 5", KeyCode.Alpha5, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 5 &5")]
#endif
		private static void PrimaryShortcut5()
		{
			primaryToolbar?.TriggerShortcut(5);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 6", KeyCode.Alpha6, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 6 &6")]
#endif
		private static void PrimaryShortcut6()
		{
			primaryToolbar?.TriggerShortcut(6);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 7", KeyCode.Alpha7, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 7 &7")]
#endif
		private static void PrimaryShortcut7()
		{
			primaryToolbar?.TriggerShortcut(7);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 8", KeyCode.Alpha8, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 8 &8")]
#endif
		private static void PrimaryShortcut8()
		{
			primaryToolbar?.TriggerShortcut(8);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 9", KeyCode.Alpha9, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 9 &9")]
#endif
		private static void PrimaryShortcut9()
		{
			primaryToolbar?.TriggerShortcut(9);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Primary Toolbar 0", KeyCode.Alpha0, UnityShortcutModifiers.Alt)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Primary 0 &0")]
#endif
		private static void PrimaryShortcut0()
		{
			primaryToolbar?.TriggerShortcut(0);
		}

#endif

#if PEEK_SECONDARY_SHORTCUTS

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 1", KeyCode.Alpha1, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 1 &#1")]
#endif
		private static void SecondaryShortcut1()
		{
			secondaryToolbar?.TriggerShortcut(1);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 2", KeyCode.Alpha2, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 2 &#2")]
#endif
		private static void SecondaryShortcut2()
		{
			secondaryToolbar?.TriggerShortcut(2);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 3", KeyCode.Alpha3, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 3 &#3")]
#endif
		private static void SecondaryShortcut3()
		{
			secondaryToolbar?.TriggerShortcut(3);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 4", KeyCode.Alpha4, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 4 &#4")]
#endif
		private static void SecondaryShortcut4()
		{
			secondaryToolbar?.TriggerShortcut(4);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 5", KeyCode.Alpha5, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 5 &#5")]
#endif
		private static void SecondaryShortcut5()
		{
			secondaryToolbar?.TriggerShortcut(5);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 6", KeyCode.Alpha6, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 6 &#6")]
#endif
		private static void SecondaryShortcut6()
		{
			secondaryToolbar?.TriggerShortcut(6);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 7", KeyCode.Alpha7, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 7 &#7")]
#endif
		private static void SecondaryShortcut7()
		{
			secondaryToolbar?.TriggerShortcut(7);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 8", KeyCode.Alpha8, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 8 &#8")]
#endif
		private static void SecondaryShortcut8()
		{
			secondaryToolbar?.TriggerShortcut(8);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 9", KeyCode.Alpha9, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 9 &#9")]
#endif
		private static void SecondaryShortcut9()
		{
			secondaryToolbar?.TriggerShortcut(9);
		}

#if UNITY_2019_1_OR_NEWER
		[Shortcut("Peek/Secondary Toolbar 0", KeyCode.Alpha0, UnityShortcutModifiers.Alt | UnityShortcutModifiers.Shift)]
#else
		[MenuItem("Tools/Peek/Shortcuts/Secondary 0 &#0")]
#endif
		private static void SecondaryShortcut0()
		{
			secondaryToolbar?.TriggerShortcut(0);
		}

#endif
	}
}