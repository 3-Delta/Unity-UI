using System;

namespace Ludiq.PeekCore
{
	public static class GuidUtility
	{
		public static string ToShortString(this Guid guid)
		{
			return guid.ToString().PartBefore('-');
		}
	}
}
