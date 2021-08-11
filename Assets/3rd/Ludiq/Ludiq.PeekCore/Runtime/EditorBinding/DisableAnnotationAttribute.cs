using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DisableAnnotationAttribute : Attribute
	{
		public bool disableIcon { get; set; } = true;
		public bool disableGizmo { get; set; } = false;
	}
}