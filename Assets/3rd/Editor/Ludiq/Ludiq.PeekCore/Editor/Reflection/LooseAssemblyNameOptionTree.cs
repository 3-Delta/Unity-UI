using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class LooseAssemblyNameOptionTree : FuzzyOptionTree
	{
		public LooseAssemblyNameOptionTree() : base(new GUIContent("Assembly"))
		{
			looseAssemblyNames = Codebase.assemblies.Select(a => new LooseAssemblyName(a.GetName().Name)).ToList();
		}

		private readonly List<LooseAssemblyName> looseAssemblyNames;

		public override bool searchable { get; } = true;

		public override IEnumerable<IFuzzyOption> Root()
		{
			return looseAssemblyNames.OrderBy(x => x.name).Select(x => new LooseAssemblyNameOption(x));
		}

		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			return Enumerable.Empty<IFuzzyOption>();
		}
		
		public override IEnumerable<ISearchResult<IFuzzyOption>> SearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			return looseAssemblyNames.Select(x => new LooseAssemblyNameOption(x)).OrderableSearchFilter(query, x => x.haystack, cancellation);
		}
	}
}