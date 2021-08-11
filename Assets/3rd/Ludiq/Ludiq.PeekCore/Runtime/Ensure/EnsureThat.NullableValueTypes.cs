using System;

namespace Ludiq.PeekCore
{
	public partial class EnsureThat
	{
		public void IsNotNull<T>(T? value) where T : struct
		{
			if (!Ensure.IsActive)
			{
				return;
			}

			if (value == null)
			{
				throw new ArgumentNullException(paramName, ExceptionMessages.Common_IsNotNull_Failed);
			}
		}
	}
}