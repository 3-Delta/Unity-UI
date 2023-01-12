using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class PredictableAttribute : Attribute
	{
		public PredictableAttribute() { }
	}
}