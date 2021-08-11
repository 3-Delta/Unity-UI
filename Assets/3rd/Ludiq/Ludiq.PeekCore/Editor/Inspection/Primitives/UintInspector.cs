using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(uint), typeof(UintInspector))]

namespace Ludiq.PeekCore
{
	public class UintInspector : DiscreteNumberInspector<uint>
	{
		public UintInspector(Accessor accessor) : base(accessor) { }
	}
}