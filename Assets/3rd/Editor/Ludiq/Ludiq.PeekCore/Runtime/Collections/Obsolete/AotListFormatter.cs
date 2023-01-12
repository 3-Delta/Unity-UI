using Ludiq.PeekCore;
using Ludiq.OdinSerializer;

#pragma warning disable 618

[assembly: RegisterFormatter(typeof(AotListFormatter))]

namespace Ludiq.PeekCore
{
	// Odin doesn't seem to like ArrayList at all, so we're handling it manually.

	public sealed class AotListFormatter : MinimalBaseFormatter<AotList>
	{
		private static readonly Serializer<object> ObjectSerializer = Serializer.Get<object>();

		protected override AotList GetUninitializedObject()
		{
			return new AotList();
		}

		protected override void Read(ref AotList value, IDataReader reader)
		{
			reader.EnterArray(out var length);

			for (int i = 0; i < length; i++)
			{
				var item = ObjectSerializer.ReadValue(reader);

				value.Add(item);
			}

			reader.ExitArray();
		}

		protected override void Write(ref AotList value, IDataWriter writer)
		{
			writer.BeginArrayNode(value.Count);

			for (int i = 0; i < value.Count; i++)
			{
				ObjectSerializer.WriteValue(value[i], writer);
			}

			writer.EndArrayNode();
		}
	}
}
