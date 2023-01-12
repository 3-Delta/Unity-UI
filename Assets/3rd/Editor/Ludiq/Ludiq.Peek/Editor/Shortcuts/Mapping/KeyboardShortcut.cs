using System;
using System.Text;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	[Serializable, Inspectable]
	public sealed class KeyboardShortcut : Shortcut
	{
		[SerializeField]
		private KeyCode _keyCode = KeyCode.None;

		[SerializeField]
		private ShortcutModifiers _modifiers = ShortcutModifiers.None;

		[SerializeField]
		private KeyboardShortcutAction _action = KeyboardShortcutAction.Press;

		public KeyboardShortcut()
		{

		}

		public KeyboardShortcut(KeyCode keyCode, ShortcutModifiers modifiers, KeyboardShortcutAction action = KeyboardShortcutAction.Press)
		{
			this.keyCode = keyCode;
			this.modifiers = modifiers;
			this.action = action;
		}

		[Inspectable]
		public KeyCode keyCode
		{
			get => _keyCode;
			set => _keyCode = value;
		}

		[Inspectable]
		public ShortcutModifiers modifiers
		{
			get => _modifiers;
			set => _modifiers = value;
		}

		[Inspectable]
		public KeyboardShortcutAction action
		{
			get => _action;
			set => _action = value;
		}
		
		public override bool Check(Event e)
		{
			if (!enabled)
			{
				return false;
			}

			var targeted = e.keyCode == keyCode &&
			               CompareModifiers(modifiers, e.modifiers);
			
			if (action == KeyboardShortcutAction.Press)
			{
				return targeted && e.type == EventType.KeyDown;
			}
			else
			{
				throw action.Unexpected();
			}
		}

		public override bool Preview(Event e)
		{
			return enabled && modifiers != ShortcutModifiers.None && CompareModifiers(modifiers, e.modifiers);
		}
	}
}