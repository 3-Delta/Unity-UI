using System;
using System.IO;

namespace Ludiq.PeekCore
{
	public static class FastSerializerUtilities
	{
		public static void WriteNullable(this BinaryWriter writer, string val)
		{
			var hasVal = val != null;

			writer.Write(hasVal);

			if (hasVal)
			{
				writer.Write(val);
			}
		}

		public static string ReadNullableString(this BinaryReader reader)
		{
			if (reader.ReadBoolean())
			{
				return reader.ReadString();
			}
			else
			{
				return null;
			}
		}

		public static void Write(this BinaryWriter writer, TypeData data)
		{
			writer.Write((int)data.code);
			writer.WriteNullable(data.name);
		}

		public static TypeData ReadTypeData(this BinaryReader reader)
		{
			var data = new TypeData();
			data.code = (TypeCode)reader.ReadInt32();
			data.name = reader.ReadNullableString();
			return data;
		}
	}
}
