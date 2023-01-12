using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(int), typeof(IntInspector))]

namespace Ludiq.PeekCore
{
	public class IntInspector : DiscreteNumberInspector<int>
	{
		public IntInspector(Accessor accessor) : base(accessor) { }
	}
}