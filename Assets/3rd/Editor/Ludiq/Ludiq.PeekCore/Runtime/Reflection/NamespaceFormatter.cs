using Ludiq.PeekCore;
using Ludiq.OdinSerializer;

[assembly: RegisterFormatter(typeof(NamespaceFormatter))]

namespace Ludiq.PeekCore
{
	public class NamespaceFormatter : MinimalBaseFormatter<Namespace>
	{
		private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();

		protected override void Read(ref Namespace value, IDataReader reader)
		{
			value = StringSerializer.ReadValue(reader);
		}

		protected override void Write(ref Namespace value, IDataWriter writer)
		{
			StringSerializer.WriteValue(value.FullName, writer);
		}
	}
}