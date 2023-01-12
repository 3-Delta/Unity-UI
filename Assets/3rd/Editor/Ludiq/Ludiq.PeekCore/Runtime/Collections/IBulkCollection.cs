using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public interface IBulkCollection<T> : ICollection<T>
	{
		void Add(IEnumerable<T> items);
	}
}