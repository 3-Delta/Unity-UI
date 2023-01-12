using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(float), typeof(FloatInspector))]

namespace Ludiq.PeekCore
{
	public class FloatInspector : ContinuousNumberInspector<float>
	{
		public FloatInspector(Accessor accessor) : base(accessor) { }
	}
}