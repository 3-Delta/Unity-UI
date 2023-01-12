using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class ComponentUtility
	{
		public static bool IsEnabled(this Component component)
		{
			if (component == null)
			{
				return false;
			}
			else if (component is Behaviour behaviour)
			{
				return behaviour.enabled;
			}
			else if (component is Renderer renderer)
			{
				return renderer.enabled;
			}
			else if (component is Collider collider)
			{
				return collider.enabled;
			}
			else
			{
				return true;
			}
		}
	}
}