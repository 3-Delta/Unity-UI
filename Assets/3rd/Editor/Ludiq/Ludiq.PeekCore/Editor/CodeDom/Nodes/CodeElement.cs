using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
	public abstract class CodeElement
	{
		public virtual IEnumerable<CodeElement> Children => Enumerable.Empty<CodeElement>();
	}
}
