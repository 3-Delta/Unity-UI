using UnityEngine;

namespace Ludiq.PeekCore
{
	public enum Edge
	{
		Top,
		Bottom,
		Left,
		Right
	}

	public static class XEdge
	{
		public static Vector2 Normal(this Edge edge)
		{
			switch (edge)
			{
				case Edge.Left:
					return Vector2.left;
				case Edge.Right:
					return Vector2.right;
				case Edge.Top:
					return Vector2.down; // IMGUI has inverted Y
				case Edge.Bottom:
					return Vector2.up; // IMGUI has inverted Y
				default:
					throw new UnexpectedEnumValueException<Edge>(edge);
			}
		}

		public static Edge Opposite(this Edge edge)
		{
			switch (edge)
			{
				case Edge.Left:
					return Edge.Right;
				case Edge.Right:
					return Edge.Left;
				case Edge.Top:
					return Edge.Bottom;
				case Edge.Bottom:
					return Edge.Top;
				default:
					throw new UnexpectedEnumValueException<Edge>(edge);
			}
		}

		public static Vector2 GetEdgeCenter(this Rect rect, Edge edge)
		{
			switch (edge)
			{
				case Edge.Left:
					return new Vector2(rect.xMin, rect.center.y);
				case Edge.Right:
					return new Vector2(rect.xMax, rect.center.y);
				case Edge.Top:
					return new Vector2(rect.center.x, rect.yMin);
				case Edge.Bottom:
					return new Vector2(rect.center.x, rect.yMax);
				default:
					throw new UnexpectedEnumValueException<Edge>(edge);
			}
		}
	}
}