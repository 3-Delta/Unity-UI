using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class ExtensibleFuzzyOptionTree : FuzzyOptionTree
	{
		protected ExtensibleFuzzyOptionTree()
		{
			extensions = this.Extensions().ToList();
		}

		protected ExtensibleFuzzyOptionTree(GUIContent header) : this()
		{
			this.header = header;
		}

		public IList<IFuzzyOptionTree> extensions { get; }

		public override void Prewarm()
		{
			foreach (var extension in extensions)
			{
				extension.Prewarm();
			}
		}

		public override void Rewarm()
		{
			base.Rewarm();

			foreach (var extension in extensions)
			{
				extension.Rewarm();
			}
		}



		#region Hierarchy

		public override IEnumerable<IFuzzyOption> Root()
		{
			foreach (var extension in extensions)
			{
				foreach (var extensionRootItem in extension.Root())
				{
					yield return extensionRootItem;
				}
			}
		}

		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			foreach (var extension in extensions)
			{
				var children = (extension is FuzzyOptionTree fot) ? fot.Children(parent, ordered) : extension.Children(parent);

				foreach (var extensionChild in children)
				{
					yield return extensionChild;
				}
			}
		}

		#endregion

		#region Search

		public override IEnumerable<IFuzzyOption> OrderedSearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			foreach (var baseSearchResult in base.OrderedSearchResults(query, parent, cancellation))
			{
				yield return baseSearchResult;
			}

			foreach (var extension in extensions)
			{
				if (extension.searchable)
				{
					foreach (var extensionSearchResult in extension.OrderedSearchResults(query, parent, cancellation))
					{
						yield return extensionSearchResult;
					}
				}
			}
		}

		public override IEnumerable<ISearchResult<IFuzzyOption>> SearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			foreach (var baseSearchResult in base.SearchResults(query, parent, cancellation))
			{
				yield return baseSearchResult;
			}

			foreach (var extension in extensions)
			{
				if (extension.searchable)
				{
					foreach (var extensionSearchResult in extension.SearchResults(query, parent, cancellation))
					{
						yield return extensionSearchResult;
					}
				}
			}
		}

		public override string SearchResultLabel(IFuzzyOption item, string query)
		{
			foreach (var extension in extensions)
			{
				try
				{
					var label = extension.SearchResultLabel(item, query);
					if (label != null)
					{
						return label;
					}
				}
				catch (NotSupportedException) {}
			}

			return base.SearchResultLabel(item, query);
		}

		#endregion
	}
}