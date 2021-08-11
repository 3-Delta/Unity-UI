using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(long), typeof(LongInspector))]

namespace Ludiq.PeekCore
{
	public class LongInspector : ContinuousNumberInspector<long>
	{
		public LongInspector(Accessor accessor) : base(accessor) { }
	}
}