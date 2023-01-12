using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class FuzzyOptionTree : IFuzzyOptionTree
	{
		protected FuzzyOptionTree() { }

		protected FuzzyOptionTree(GUIContent header) : this()
		{
			this.header = header;
		}

		public GUIContent header { get; set; }

		public ICollection<object> selected { get; } = new HashSet<object>();

		public bool prewarmed { get; set; }

		public virtual void Prewarm() { }

		public virtual void Rewarm() { }

		

		private readonly List<bool> separatorCheckStack = new List<bool>();

		public void BeginSeparatorCheck()
		{
			separatorCheckStack.Add(false);
		}

		public bool EndSeparatorCheck()
		{
			if (separatorCheckStack.Count == 0)
			{
				Debug.LogWarning("Invalid separator check stacking.");
				return false;
			}

			var result = separatorCheckStack[separatorCheckStack.Count - 1];
			separatorCheckStack.RemoveAt(separatorCheckStack.Count - 1);
			return result;
		}

		protected IEnumerable<IFuzzyOption> SeparatorGroup(string label, IEnumerable<IFuzzyOption> options)
		{
			var _options = options.ToListPooled();

			if (_options.Count > 0)
			{
				yield return Separator(label);

				foreach (var option in _options)
				{
					yield return option;
				}
			}

			_options.Free();
		}

		protected IFuzzyOption Separator(string label)
		{
			for (int i = 0; i < separatorCheckStack.Count; i++)
			{
				separatorCheckStack[i] = true;
			}

			return new FuzzySeparator(label);
		}

		#region Multithreading

		public virtual bool multithreaded => true;

		#endregion

		#region Hierarchy

		public abstract IEnumerable<IFuzzyOption> Root();

		public virtual IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered) => Enumerable.Empty<IFuzzyOption>();
		
		public IEnumerable<IFuzzyOption> Children(IFuzzyOption parent)
		{
			return Children(parent, true);
		}

		public virtual IEnumerable<IFuzzyOption> SearchableRoot() => Enumerable.Empty<IFuzzyOption>();

		public virtual IEnumerable<IFuzzyOption> SearchableChildren(IFuzzyOption parent)
		{
			if (parent is FuzzyWindow.FavoritesRoot)
			{
				foreach (var favorite in favorites.Where(CanFavorite))
				{
					yield return favorite;
				}
			}
			else
			{
				foreach (var child in Children(parent, false))
				{
					yield return child;

					foreach (var grandchild in SearchableChildren(child))
					{
						yield return grandchild;
					}
				}
			}
		}

		#endregion

		#region Search

		public virtual bool searchable { get; } = false;

		public virtual IEnumerable<IFuzzyOption> OrderedSearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			return SearchResults(query, parent, cancellation).OrderByRelevance();
		}

		public virtual IEnumerable<ISearchResult<IFuzzyOption>> SearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			var options = parent == null ? SearchableRoot() : SearchableChildren(parent);

			return options.OrderableSearchFilter(query, x => x.haystack, cancellation);
		}

		public virtual string SearchResultLabel(IFuzzyOption item, string query)
		{
			return item.SearchResultLabel(query);
		}

		#endregion

		#region Favorites

		public virtual ICollection<IFuzzyOption> favorites => null;

		public virtual bool UseExplicitLabel(IFuzzyOption parent, IFuzzyOption item)
		{
			return false;	
		}

		public virtual string ExplicitLabel(IFuzzyOption item)
		{
			throw new NotSupportedException();
		}

		public virtual bool CanFavorite(IFuzzyOption item)
		{
			throw new NotSupportedException();
		}

		public virtual void OnFavoritesChange() { }

		#endregion
	}
}