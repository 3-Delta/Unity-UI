using System;
using System.Collections;
using UnityEngine.Scripting;

namespace Ludiq.PeekCore
{
	[Obsolete("Using strongly typed List<T> instead.")]
	public sealed class AotList : ArrayList
	{
		public AotList() : base() { }
		public AotList(int capacity) : base(capacity) { }
		public AotList(ICollection c) : base(c) { }

		[Preserve]
		public static void AotStubs()
		{
			var list = new AotList();

			list.Add(default(object));
			list.Remove(default(object));
			var item = list[default(int)];
			list[default(int)] = default(object);
			list.Contains(default(object));
			list.Clear();
			var count = list.Count;
		}
	}
}
