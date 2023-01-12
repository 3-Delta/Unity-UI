using System;

namespace Ludiq.PeekCore
{
	// A small structure that is much faster to serialize and deserialize
	// for common types that are part of TypeCode
	public struct TypeData
	{
		public TypeCode code;

		public string name;

		public string ToName()
		{
			if (code == TypeCode.Object)
			{
				return name;
			}
			else
			{
				return RuntimeCodebase.SerializeType(code.ToType());
			}
		}
	}
}
