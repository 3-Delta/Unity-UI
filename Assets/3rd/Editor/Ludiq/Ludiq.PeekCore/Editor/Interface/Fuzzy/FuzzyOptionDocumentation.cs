using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore
{
	public abstract class FuzzyOptionDocumentation : IFuzzyOptionDocumentation
	{
		public virtual string summary => null;

		public virtual string returns => null;

		public virtual string remarks => null;

		public virtual string returnType => null;

		public virtual EditorTexture returnIcon => null;

		public virtual IEnumerable<string> parameters => Enumerable.Empty<string>();

		public virtual string GetParameterName(string parameter) => parameter;

		public virtual string GetParameterSummary(string parameter) => null;

		public virtual string GetParameterType(string parameter) => null;

		public virtual EditorTexture GetParameterIcon(string parameter) => null;
	}
}