using System;

namespace Ludiq.PeekCore
{
	public class ProxyAccessor : Accessor
	{
		public ProxyAccessor(object subpath, Accessor binding, Accessor parent) : base(subpath, parent)
		{
			this.binding = binding;

			if (binding != null)
			{
				definedType = binding.definedType;
				label = binding.label;
			}
		}

		public Accessor binding { get; private set; }

		protected override object rawValue
		{
			get
			{
				if (binding == null)
				{
					return null;
				}

				return binding.value;
			}
			set
			{
				if (binding == null)
				{
					return;
				}

				binding.value = value;
			}
		}
		
		public override Attribute[] GetCustomAttributes(bool inherit = true)
		{
			return binding.GetCustomAttributes(inherit);
		}
	}
}