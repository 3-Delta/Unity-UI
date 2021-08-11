using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(sbyte), typeof(SbyteInspector))]

namespace Ludiq.PeekCore
{
	public class SbyteInspector : DiscreteNumberInspector<sbyte>
	{
		public SbyteInspector(Accessor accessor) : base(accessor) { }
	}
}