using System;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	[Serializable]
	public abstract class Shortcut
	{
		[SerializeField]
		private bool _enabled = true;

		protected Shortcut()
		{

		}
		
		public virtual bool enabled
		{
			get => _enabled;
			set => _enabled = value;
		}

		public abstract bool Check(Event e);

		public abstract bool Preview(Event e);

		public bool Check()
		{
			return Check(Event.current);
		}

		public bool Preview()
		{
			return Preview(Event.current);
		}

		public static bool CompareModifiers(ShortcutModifiers s, EventModifiers e)
		{
			// Ignore unsupported modifiers
			e &= ~EventModifiers.CapsLock;
			e &= ~EventModifiers.FunctionKey;
			e &= ~EventModifiers.Numeric;

			return s.ToEventModifiers() == e;
		}
	}
}