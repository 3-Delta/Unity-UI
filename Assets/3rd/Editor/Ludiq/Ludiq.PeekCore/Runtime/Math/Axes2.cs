using System;
using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	[Flags]
	public enum Axes2
	{
		None = 0,
		Horizontal = 1,
		Vertical = 2,
		Both = Horizontal | Vertical
	}

	public static class XAxes2
	{
		public static bool Contains(this Axes2 axes, Axis2 axis)
		{
			// Avoid HasFlag for speed and memory
			switch (axis)
			{
				case Axis2.Horizontal: return (axes & Axes2.Horizontal) != 0;
				case Axis2.Vertical: return (axes & Axes2.Vertical) != 0;
				default: throw new UnexpectedEnumValueException<Axis2>(axis);
			}
		}

		public static IEnumerable<Axis2> Split(this Axes2 axes)
		{
			if (axes.Contains(Axis2.Horizontal))
			{
				yield return Axis2.Horizontal;
			}

			if (axes.Contains(Axis2.Vertical))
			{
				yield return Axis2.Vertical;
			}
		}
	}
}
