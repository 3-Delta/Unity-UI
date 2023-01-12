using System;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	[Serializable]
	public sealed class MouseShortcut : Shortcut
	{
		[SerializeField]
		private MouseShortcutButton _button = MouseShortcutButton.Left;

		[SerializeField]
		private ShortcutModifiers _modifiers = ShortcutModifiers.None;

		[SerializeField]
		private MouseShortcutAction _action = MouseShortcutAction.Click;

		public MouseShortcut()
		{

		}

		public MouseShortcut(MouseShortcutButton button, ShortcutModifiers modifiers, MouseShortcutAction action = MouseShortcutAction.Click)
		{
			this.button = button;
			this.modifiers = modifiers;
			this.action = action;
		}
		
		public MouseShortcutButton button
		{
			get => _button;
			set => _button = value;
		}
		
		public ShortcutModifiers modifiers
		{
			get => _modifiers;
			set => _modifiers = value;
		}
		
		public MouseShortcutAction action
		{
			get => _action;
			set => _action = value;
		}

		private Vector2 pressPosition;

		public bool checkRelease { get; set; }

		public bool requireStaticRelease { get; set; }

		public override bool Check(Event e)
		{
			if (!enabled)
			{
				return false;
			}

			var targeted = (button == MouseShortcutButton.Right ? e.IsContextMouseButton() : (e.button == button.ToInt())) &&
			               CompareModifiers(modifiers, e.modifiers);

			if (!targeted)
			{
				return false;
			}

			if (action == MouseShortcutAction.DoubleClick && e.clickCount != 2)
			{
				return false;
			}

			if (e.type == EventType.MouseDown)
			{
				pressPosition = e.mousePosition;
			}

			if (action == MouseShortcutAction.Click && checkRelease)
			{
				if (requireStaticRelease && Vector2.Distance(e.mousePosition, pressPosition) > 1)
				{
					return false;
				}
				
				return e.type == EventType.MouseUp;
			}

			return e.type == EventType.MouseDown;
		}

		public override bool Preview(Event e)
		{
			return enabled && modifiers != ShortcutModifiers.None && CompareModifiers(modifiers, e.modifiers);
		}
	}
}