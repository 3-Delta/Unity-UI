using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class NamespaceOptionTree : FuzzyOptionTree
	{
		private NamespaceOptionTree() : base(new GUIContent("Namespace")) { }

		public NamespaceOptionTree(IEnumerable<Namespace> namespaces) : this()
		{
			Ensure.That(nameof(namespaces)).IsNotNull(namespaces);

			this.namespaces = namespaces.ToHashSet();
		}

		private readonly HashSet<Namespace> namespaces;



		#region Hierarchy

		public override IEnumerable<IFuzzyOption> Root()
		{
			foreach (var @namespace in namespaces.Select(ns => ns.Root)
			                                     .Distinct()
			                                     .OrderBy(ns => ns.DisplayName(false)))
			{
				yield return new NamespaceOption(@namespace, FuzzyOptionMode.Branch);
				yield return new NamespaceOption(@namespace, FuzzyOptionMode.Leaf);
			}
		}

		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			if (parent is NamespaceOption namespaceOption)
			{
				var @namespace = namespaceOption.value;

				if (!@namespace.IsGlobal)
				{
					var childNamespaces = namespaces.Where(ns => ns.Parent == @namespace);

					if (ordered)
					{
						childNamespaces = childNamespaces.OrderBy(ns => ns.DisplayName(false));
					}

					foreach (var childNamespace in childNamespaces)
					{
						yield return new NamespaceOption(childNamespace, FuzzyOptionMode.Branch);
						yield return new NamespaceOption(childNamespace, FuzzyOptionMode.Leaf);
					}
				}
			}
		}

		#endregion



		#region Search

		public override bool searchable { get; } = true;
		
		public override IEnumerable<ISearchResult<IFuzzyOption>> SearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			var children = parent != null
				? SearchableChildren(parent)
				: namespaces.Select(x => (IFuzzyOption)new NamespaceOption(x, FuzzyOptionMode.Branch));
			
			return children.OrderableSearchFilter(query, x => x.haystack, cancellation);
		}

		#endregion
	}
}