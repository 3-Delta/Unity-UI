using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;
	
	public sealed class PolyShortcut
	{
		[SerializeField]
		private KeyboardShortcut _keyboardShortcut = new KeyboardShortcut();

		[SerializeField]
		private MouseShortcut _mouseShortcut = new MouseShortcut();
		
		public KeyboardShortcut keyboardShortcut => _keyboardShortcut;

		public MouseShortcut mouseShortcut => _mouseShortcut;

		public PolyShortcut() { }
		 
		public PolyShortcut(KeyCode keyCode, ShortcutModifiers modifiers, KeyboardShortcutAction action = KeyboardShortcutAction.Press)
		{
			mouseShortcut.enabled = false;
			keyboardShortcut.enabled = true;
			keyboardShortcut.keyCode = keyCode;
			keyboardShortcut.modifiers = modifiers;
			keyboardShortcut.action = action;
		}

		public PolyShortcut(MouseShortcutButton button, ShortcutModifiers modifiers, MouseShortcutAction action = MouseShortcutAction.Click)
		{
			keyboardShortcut.enabled = false;
			mouseShortcut.enabled = true;
			mouseShortcut.button = button;
			mouseShortcut.modifiers = modifiers;
			mouseShortcut.action = action;
		}

		public bool Check(Event e)
		{
			return keyboardShortcut.Check(e) ||
			       mouseShortcut.Check(e);
		}

		public bool Preview(Event e)
		{
			return keyboardShortcut.Preview(e) ||
			       mouseShortcut.Preview(e);
		}
	}
}