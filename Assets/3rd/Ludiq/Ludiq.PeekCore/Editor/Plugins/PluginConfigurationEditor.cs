using System;
using Ludiq.PeekCore;
using UnityEngine;

[assembly: RegisterEditor(typeof(PluginConfiguration), typeof(PluginConfigurationEditor))]

namespace Ludiq.PeekCore
{
	public sealed class PluginConfigurationEditor : Editor
	{
		public PluginConfigurationEditor(Accessor accessor) : base(accessor) { }

		protected override float GetInnerHeight(float width)
		{
			throw new NotImplementedException();
		}

		protected override void OnInnerGUI(Rect position)
		{
			throw new NotImplementedException();
		}
	}
}
