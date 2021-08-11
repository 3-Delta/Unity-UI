using System.IO;

namespace Ludiq.PeekCore
{
	public class XmlDocumentationTagsFastSerializer : FastSerializer<XmlDocumentationTags>
	{
		public static XmlDocumentationTagsFastSerializer instance { get; } = new XmlDocumentationTagsFastSerializer();

		public override XmlDocumentationTags Instantiate()
		{
			return new XmlDocumentationTags();
		}

		public override void Write(XmlDocumentationTags value, BinaryWriter writer)
		{
			writer.WriteNullable(value.summary);
			writer.WriteNullable(value.returns);
			writer.WriteNullable(value.remarks);
			writer.Write(value.inherit);

			writer.Write(value.parameters.Count);

			foreach (var kvp in value.parameters)
			{
				writer.Write(kvp.Key);
				writer.WriteNullable(kvp.Value);
			}

			writer.Write(value.typeParameters.Count);

			foreach (var kvp in value.typeParameters)
			{
				writer.Write(kvp.Key);
				writer.WriteNullable(kvp.Value);
			}
		}

		public override void Read(ref XmlDocumentationTags value, BinaryReader reader)
		{
			value.summary = reader.ReadNullableString();
			value.returns = reader.ReadNullableString();
			value.remarks = reader.ReadNullableString();
			value.inherit = reader.ReadBoolean();

			var parametersCount = reader.ReadInt32();

			for (int i = 0; i < parametersCount; i++)
			{
				value.parameters.Add(reader.ReadString(), reader.ReadNullableString());
			}

			var typeParametersCount = reader.ReadInt32();

			for (int i = 0; i < typeParametersCount; i++)
			{
				value.typeParameters.Add(reader.ReadString(), reader.ReadNullableString());
			}
		}
	}
}
