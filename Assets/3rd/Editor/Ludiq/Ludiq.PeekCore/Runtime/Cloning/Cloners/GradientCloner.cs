using System;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class GradientCloner : Cloner<Gradient>
	{
		public override bool Handles(Type type)
		{
			return type == typeof(Gradient);
		}

		public override Gradient ConstructClone(Type type, Gradient original)
		{
			return new Gradient();
		}

		public override void FillClone(Type type, ref Gradient clone, Gradient original, CloningContext context)
		{
			clone.SetKeys(original.colorKeys, original.alphaKeys);

			try
			{
				clone.mode = original.mode;
			}
			catch (UnityException)
			{
				// We're off the main thread, nothing we can do
			}
		}
	}
}