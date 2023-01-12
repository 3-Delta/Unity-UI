using System;
using JetBrains.Annotations;

namespace Ludiq.PeekCore
{
	public partial class EnsureThat
	{
		public void IsNull<T>([NoEnumeration] T value)
		{
			if (!Ensure.IsActive)
			{
				return;
			}

			if (value != null)
			{
				throw new ArgumentNullException(paramName, ExceptionMessages.Common_IsNull_Failed);
			}
		}

		public void IsNotNull<T>([NoEnumeration] T value)
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