using UnityEngine;

namespace Ludiq.PeekCore
{
	// http://www.stefanbader.ch/faster-line-segment-intersection-for-unity3dc/
	public struct Line2
	{
		public Vector2 a;

		public Vector2 b;

		public Line2(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
		}

		public bool Intersects(Line2 o)
		{
			return Intersect(this, o);
		}

		public static bool Intersect(Line2 la, Line2 lb)
		{
			var a1 = la.a;
			var a2 = la.b;
			var b1 = lb.a;
			var b2 = lb.b;

			var a = a2 - a1;
			var b = b1 - b2;
			var c = a1 - b1;

			var alphaNumerator = b.y * c.x - b.x * c.y;
			var alphaDenominator = a.y * b.x - a.x * b.y;
			var betaNumerator = a.x * c.y - a.y * c.x;
			var betaDenominator = alphaDenominator;

			var intersect = true;

			if (alphaDenominator == 0 || betaDenominator == 0)
			{
				intersect = false;
			}
			else
			{
				if (alphaDenominator > 0)
				{
					if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
					{
						intersect = false;
					}
				}
				else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
				{
					intersect = false;
				}

				if (intersect && betaDenominator > 0)
				{
					if (betaNumerator < 0 || betaNumerator > betaDenominator)
					{
						intersect = false;
					}
				}
				else if (betaNumerator > 0 || betaNumerator < betaDenominator)
				{
					intersect = false;
				}
			}

			return intersect;
		}
	}

	public static class XLine2
	{
		public static Line2 LeftLine(this Rect rect)
		{
			return new Line2(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax));
		}

		public static Line2 RightLine(this Rect rect)
		{
			return new Line2(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax));
		}

		public static Line2 TopLine(this Rect rect)
		{
			return new Line2(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin));
		}

		public static Line2 BottomLine(this Rect rect)
		{
			return new Line2(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax));
		}

		public static Line2 Line(this Rect rect, Edge edge)
		{
			switch (edge)
			{
				case Edge.Left: return rect.LeftLine();
				case Edge.Right: return rect.RightLine();
				case Edge.Top: return rect.TopLine();
				case Edge.Bottom: return rect.BottomLine();
				default: throw new UnexpectedEnumValueException<Edge>(edge);
			}
		}

		public static bool Intersects(this Rect rect, Line2 line)
		{
			return
				line.Intersects(rect.LeftLine()) ||
				line.Intersects(rect.RightLine()) ||
				line.Intersects(rect.TopLine()) ||
				line.Intersects(rect.BottomLine());
		}
	}
}
