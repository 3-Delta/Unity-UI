using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public interface IAotStubbable
	{
		IEnumerable<object> aotStubs { get; }
	}
}