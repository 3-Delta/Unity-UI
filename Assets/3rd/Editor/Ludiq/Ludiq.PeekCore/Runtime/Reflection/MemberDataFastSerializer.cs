using System.Linq;
using System.IO;

namespace Ludiq.PeekCore.Bolt
{
	public sealed class MemberDataFastSerializer : FastSerializer<MemberData>
	{
		public static MemberDataFastSerializer instance { get; } = new MemberDataFastSerializer();

		public override MemberData Instantiate()
		{
			return new MemberData();
		}

		public override void Write(MemberData value, BinaryWriter writer)
		{
			writer.Write((byte)value.source);
			writer.Write(value.reflectedType);
			writer.Write(value.targetType);
			writer.Write(value.name);
			writer.Write(value.isOpenConstructed);
			writer.Write(value.isExtension);

			if (value.parameterTypeNames != null)
			{
				writer.Write(true);
				writer.Write(value.parameterTypeNames.Length);

				foreach (var parameterTypeName in value.parameterTypeNames)
				{
					writer.Write(parameterTypeName);
				}
			}
			else
			{
				writer.Write(false);
			}

			if (value.parameterOpenTypeNames != null)
			{
				writer.Write(true);
				writer.Write(value.parameterOpenTypeNames.Length);

				foreach (var parameterTypeName in value.parameterOpenTypeNames)
				{
					writer.Write(parameterTypeName);
				}
			}
			else
			{
				writer.Write(false);
			}

			if (value.genericMethodTypeArgumentNames != null)
			{
				writer.Write(true);
				writer.Write(value.genericMethodTypeArgumentNames.Length);

				if (value.genericMethodTypeArgumentNames.All(x => x != null))
				{
					writer.Write(true);
					foreach (var typeName in value.genericMethodTypeArgumentNames)
					{
						writer.Write(typeName);
					}
				}
				else
				{
					writer.Write(false);
				}
			}
			else
			{
				writer.Write(false);
			}
		}

		public override void Read(ref MemberData value, BinaryReader reader)
		{
			value.source = (Member.Source)reader.ReadByte();
			value.reflectedType = reader.ReadTypeData();
			value.targetType = reader.ReadTypeData();
			value.name = reader.ReadString();
			value.isOpenConstructed = reader.ReadBoolean();
			value.isExtension = reader.ReadBoolean();

			if (reader.ReadBoolean())
			{
				value.parameterTypeNames = new string[reader.ReadInt32()];

				for (int i = 0; i < value.parameterTypeNames.Length; i++)
				{
					value.parameterTypeNames[i] = reader.ReadString();
				}
			}
			else
			{
				value.parameterTypeNames = null;
			}

			if (reader.ReadBoolean())
			{
				value.parameterOpenTypeNames = new string[reader.ReadInt32()];

				for (int i = 0; i < value.parameterOpenTypeNames.Length; i++)
				{
					value.parameterOpenTypeNames[i] = reader.ReadString();
				}
			}
			else
			{
				value.parameterOpenTypeNames = null;
			}

			if (reader.ReadBoolean())
			{
				value.genericMethodTypeArgumentNames = new string[reader.ReadInt32()];

				if (reader.ReadBoolean())
				{
					for (int i = 0; i < value.genericMethodTypeArgumentNames.Length; i++)
					{
						value.genericMethodTypeArgumentNames[i] = reader.ReadString();
					}
				}
			}
			else
			{
				value.genericMethodTypeArgumentNames = null;
			}
		}
	}
}
