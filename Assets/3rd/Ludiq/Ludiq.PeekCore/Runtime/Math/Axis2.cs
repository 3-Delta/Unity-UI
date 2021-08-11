using UnityEngine;

namespace Ludiq.PeekCore
{
	public enum Axis2
	{
		Horizontal,
		Vertical
	}

	public static class XAxis2
	{
		public static Axis2 Perpendicular(this Axis2 axis)
		{
			switch (axis)
			{
				case Axis2.Horizontal: return Axis2.Vertical;
				case Axis2.Vertical: return Axis2.Horizontal;
				default: throw new UnexpectedEnumValueException<Axis2>(axis);
			}
		}

		public static float Component(this Vector2 v, Axis2 axis)
		{
			switch (axis)
			{
				case Axis2.Horizontal: return v.x;
				case Axis2.Vertical: return v.y;
				default: throw new UnexpectedEnumValueException<Axis2>(axis);
			}
		}

		public static Vector2 WithComponent(this Vector2 v, float c, Axis2 axis)
		{
			switch (axis)
			{
				case Axis2.Horizontal: v.x = c; break;
				case Axis2.Vertical: v.y = c; break;
				default: throw new UnexpectedEnumValueException<Axis2>(axis);
			}

			return v;
		}

		public static Vector2 UnaryVector(this Axis2 axis)
		{
			switch (axis)
			{
				case Axis2.Horizontal: return Vector2.right;
				case Axis2.Vertical: return Vector2.up;
				default: throw new UnexpectedEnumValueException<Axis2>(axis);
			}
		}
	}
}
