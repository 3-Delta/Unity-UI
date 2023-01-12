using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(short), typeof(ShortInspector))]

namespace Ludiq.PeekCore
{
	public class ShortInspector : DiscreteNumberInspector<short>
	{
		public ShortInspector(Accessor accessor) : base(accessor) { }
	}
}