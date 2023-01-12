using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(double), typeof(DoubleInspector))]

namespace Ludiq.PeekCore
{
	public class DoubleInspector : ContinuousNumberInspector<double>
	{
		public DoubleInspector(Accessor accessor) : base(accessor) { }
	}
}