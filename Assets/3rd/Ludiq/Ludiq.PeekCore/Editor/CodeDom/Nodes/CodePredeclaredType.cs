using System;

namespace Ludiq.PeekCore.CodeDom
{
	public struct CodePredeclaredType
	{
		public CodePredeclaredType(string namespaceName, string typeName)
		{
			NamespaceName = namespaceName;
			TypeName = typeName;
		}

		public CodePredeclaredType(Type type) : this(type.Namespace, type.Name) {}

		public string NamespaceName { get; }
		public string TypeName { get; }

		public override int GetHashCode() => NamespaceName.GetHashCode() * 23 + TypeName.GetHashCode();

		public override bool Equals(object other)
		{
			if (other is CodePredeclaredType otherPredeclaredType)
			{
				return this == otherPredeclaredType;
			}
			return false;
		}

		public static bool operator ==(CodePredeclaredType a, CodePredeclaredType b) => a.NamespaceName == b.NamespaceName && a.TypeName == b.TypeName;
		public static bool operator !=(CodePredeclaredType a, CodePredeclaredType b) => !(a == b);
	}
}
