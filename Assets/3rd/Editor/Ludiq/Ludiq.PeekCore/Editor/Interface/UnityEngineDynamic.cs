using System.Reflection;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class UnityEngineDynamic
	{
		public static readonly Assembly UnityGUIAssembly;

		public static readonly dynamic GUIClip;

		static UnityEngineDynamic()
		{
			UnityGUIAssembly = typeof(GUI).Assembly;
			GUIClip = UnityGUIAssembly.GetType("UnityEngine.GUIClip", true).AsDynamicType();
		}
	}
}