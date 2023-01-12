using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public struct ProbeFilter
	{
		public bool raycast { get; set; }
		public bool overlap { get; set; }
		public bool handles { get; set; }
		public bool proBuilder { get; set; }

		public static ProbeFilter @default { get; } = new ProbeFilter
		{
			raycast = true,
			overlap = true,
			handles = true,
			proBuilder = true,
		};
	}
}
