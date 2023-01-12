using Ludiq.PeekCore;
using Ludiq.OdinSerializer;

#pragma warning disable 618

[assembly: RegisterFormatter(typeof(AotDictionaryFormatter))]

namespace Ludiq.PeekCore
{
	// Odin doesn't seem to like OrderedDictionary at all, so we're handling it manually.

	public sealed class AotDictionaryFormatter : MinimalBaseFormatter<AotDictionary>
	{
		private static readonly Serializer<object> ObjectSerializer = Serializer.Get<object>();

		protected override AotDictionary GetUninitializedObject()
		{
			return new AotDictionary();
		}

		protected override void Read(ref AotDictionary dictionary, IDataReader reader)
		{
			reader.EnterArray(out var length);

			for (int i = 0; i < length; i++)
			{
				var key = ObjectSerializer.ReadValue(reader);
				var value = ObjectSerializer.ReadValue(reader);

				dictionary.Add(key, value);
			}

			reader.ExitArray();
		}

		protected override void Write(ref AotDictionary dictionary, IDataWriter writer)
		{
			writer.BeginArrayNode(dictionary.Count);

			var keys = new object[dictionary.Keys.Count];
			dictionary.Keys.CopyTo(keys, 0);

			for (int i = 0; i < dictionary.Count; i++)
			{
				var key = keys[i];
				var value = dictionary[i];
				
				ObjectSerializer.WriteValue(key, writer);
				ObjectSerializer.WriteValue(value, writer);
			}

			writer.EndArrayNode();
		}
	}
}
