using System.IO;

namespace Ludiq.PeekCore
{
	public abstract class FastSerializer<T>
	{
		public abstract T Instantiate();

		public abstract void Read(ref T value, BinaryReader reader);

		public abstract void Write(T value, BinaryWriter writer);

		public T Read(BinaryReader reader)
		{
			var value = Instantiate();
			Read(ref value, reader);
			return value;
		}
	}
}
