using System;

namespace Ludiq.PeekCore
{
	public interface IAttributeProvider
	{
		Attribute[] GetCustomAttributes(bool inherit);
	}
}