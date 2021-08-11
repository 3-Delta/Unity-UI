using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(decimal), typeof(DecimalInspector))]

namespace Ludiq.PeekCore
{
	public class DecimalInspector : ContinuousNumberInspector<decimal>
	{
		public DecimalInspector(Accessor accessor) : base(accessor) { }
	}
}