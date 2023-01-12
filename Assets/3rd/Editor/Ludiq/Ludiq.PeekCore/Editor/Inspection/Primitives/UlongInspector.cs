using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(ulong), typeof(UlongInspector))]

namespace Ludiq.PeekCore
{
	public class UlongInspector : ContinuousNumberInspector<ulong>
	{
		public UlongInspector(Accessor accessor) : base(accessor) { }
	}
}