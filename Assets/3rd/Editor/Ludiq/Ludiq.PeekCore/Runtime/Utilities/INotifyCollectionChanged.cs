using System;

namespace Ludiq.PeekCore
{
	public interface INotifyCollectionChanged
	{
		event Action itemChanged;
		event Action collectionChanged;
	}
}