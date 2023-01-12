using JetBrains.Annotations;
using Ludiq.PeekCore.FullSerializer;

namespace Ludiq.PeekCore
{
	[MeansImplicitUse]
	public class SerializeAsAttribute : fsPropertyAttribute
	{
		public SerializeAsAttribute(string name) : base(name) { }
	}
}