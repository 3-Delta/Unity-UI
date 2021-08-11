using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class VectorInspector : Inspector
	{
		protected VectorInspector(Accessor accessor) : base(accessor) { }

		public static class Styles
		{
			public static float compactThreshold = 120;
			public static float compactSpacing = 2;
		}
	}
}