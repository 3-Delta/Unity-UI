using System;

namespace Ludiq.PeekCore
{
	public interface INotifyChanged
	{
		event Action changedInternally;

		event Action changedExternally;
	}
}