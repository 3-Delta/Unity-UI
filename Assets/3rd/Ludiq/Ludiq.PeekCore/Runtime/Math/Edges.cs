using System;
using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	[Flags]
	public enum Edges
	{
		None = 0,
		Top = 1,
		Bottom = 2,
		Left = 4,
		Right = 8,
		All = Top | Bottom | Left | Right
	}

	public static class XEdges
	{
		public static bool Contains(this Edges edges, Edge edge)
		{
			// Avoiding HasFlag for speed and memory 
			switch (edge)
			{
				case Edge.Top: return (edges & Edges.Top) != 0;
				case Edge.Bottom: return (edges & Edges.Bottom) != 0;
				case Edge.Left: return (edges & Edges.Left) != 0;
				case Edge.Right: return (edges & Edges.Right) != 0;
				default: throw new UnexpectedEnumValueException<Edge>(edge);
			}
		}

		public static IEnumerable<Edge> Split(this Edges edges)
		{
			if (edges.Contains(Edge.Top))
			{
				yield return Edge.Top;
			}

			if (edges.Contains(Edge.Bottom))
			{
				yield return Edge.Bottom;
			}

			if (edges.Contains(Edge.Left))
			{
				yield return Edge.Left;
			}

			if (edges.Contains(Edge.Right))
			{
				yield return Edge.Right;
			}
		}
	}
}