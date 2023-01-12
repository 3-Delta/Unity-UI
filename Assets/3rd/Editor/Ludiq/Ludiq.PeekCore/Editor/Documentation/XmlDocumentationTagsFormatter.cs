using System.Collections.Generic;
using Ludiq.PeekCore;
using Ludiq.OdinSerializer;

[assembly: RegisterFormatter(typeof(XmlDocumentationTags))]

namespace Ludiq.PeekCore
{
	public class XmlDocumentationTagsFormatter : MinimalBaseFormatter<XmlDocumentationTags>
	{
		private static readonly Serializer<string> StringSerializer = Serializer.Get<string>();
		private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();
		private static readonly Serializer<Dictionary<string, string>> StringDictionarySerializer = Serializer.Get<Dictionary<string, string>>();

		protected override XmlDocumentationTags GetUninitializedObject()
		{
			return new XmlDocumentationTags();
		}

		protected override void Read(ref XmlDocumentationTags value, IDataReader reader)
		{
			value.summary = StringSerializer.ReadValue(reader);
			value.returns = StringSerializer.ReadValue(reader);
			value.remarks = StringSerializer.ReadValue(reader);
			value.inherit = BoolSerializer.ReadValue(reader);
			value.parameters = StringDictionarySerializer.ReadValue(reader);
			value.typeParameters = StringDictionarySerializer.ReadValue(reader);
		}

		protected override void Write(ref XmlDocumentationTags value, IDataWriter writer)
		{
			StringSerializer.WriteValue(value.summary, writer);
			StringSerializer.WriteValue(value.returns, writer);
			StringSerializer.WriteValue(value.remarks, writer);
			BoolSerializer.WriteValue(value.inherit, writer);
			StringDictionarySerializer.WriteValue(value.parameters, writer);
			StringDictionarySerializer.WriteValue(value.typeParameters, writer);
		}
	}
}