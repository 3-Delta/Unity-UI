using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(byte), typeof(ByteInspector))]

namespace Ludiq.PeekCore
{
	public class ByteInspector : DiscreteNumberInspector<byte>
	{
		public ByteInspector(Accessor accessor) : base(accessor) { }
	}
}