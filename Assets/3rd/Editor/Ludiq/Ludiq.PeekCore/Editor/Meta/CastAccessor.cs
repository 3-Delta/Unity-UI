using System;

namespace Ludiq.PeekCore
{
	public class CastAccessor : ProxyAccessor
	{
		public CastAccessor(Type newType, Accessor parent) : base(newType, parent, parent)
		{
			this.newType = newType;

			definedType = newType;
		}

		public Type newType { get; private set; }
		
		protected override string Subpath()
		{
			return "(" + newType.CSharpName(false) + ")";
		}

		protected override string OdinPath(string parentPath)
		{
			return parentPath;
		}

		public override Attribute[] GetCustomAttributes(bool inherit = true)
		{
			return parent.GetCustomAttributes(inherit);
		}
	}
}