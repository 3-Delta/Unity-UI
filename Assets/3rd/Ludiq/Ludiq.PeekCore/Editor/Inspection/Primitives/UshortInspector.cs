using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(ushort), typeof(UshortInspector))]

namespace Ludiq.PeekCore
{
	public class UshortInspector : DiscreteNumberInspector<ushort>
	{
		public UshortInspector(Accessor accessor) : base(accessor) { }
	}
}