using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public class MemberOptionTree : FuzzyOptionTree
	{
		public enum RootMode
		{
			Members,
			Types,
			Namespaces
		}

		public MemberOptionTree(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter, MemberAction action) : base(new GUIContent("Member"))
		{
			favorites = new Favorites(this);
			codebase = Codebase.Subset(types, memberFilter.Configured(), memberTypeFilter?.Configured());
			this.action = action;
			this.types = types;
			this.memberFilter = memberFilter;
			this.memberTypeFilter = memberTypeFilter;
			expectingBoolean = memberTypeFilter?.ExpectsBoolean ?? false;
		}

		public MemberOptionTree(UnityObject target, MemberFilter memberFilter, TypeFilter memberTypeFilter, MemberAction action)
			: this(EditorUnityObjectUtility.GetUnityTypes(target), memberFilter, memberTypeFilter, action)
		{
			rootMode = RootMode.Types;
		}

		private readonly IEnumerable<Type> types;
		private readonly MemberFilter memberFilter;
		private readonly TypeFilter memberTypeFilter;
		private CodebaseSubset codebase;
		private readonly MemberAction action;
		private readonly bool expectingBoolean;
		private readonly RootMode rootMode = RootMode.Namespaces;

		public override void Prewarm()
		{
			base.Prewarm();

			codebase = Codebase.Subset(types, memberFilter.Configured(), memberTypeFilter?.Configured());
		}

		#region Hierarchy

		public override IEnumerable<IFuzzyOption> Root()
		{
			if (rootMode == RootMode.Types)
			{
				foreach (var type in codebase.members
											 .Select(m => m.targetType)
											 .Distinct()
											 .OrderBy(t => t.DisplayName()))
				{
					yield return new TypeOption(type, FuzzyOptionMode.Branch);
				}
			}
			else if (rootMode == RootMode.Namespaces)
			{
				foreach (var @namespace in codebase.members.Select(m => m.targetType)
												   .Distinct()
												   .Where(t => !t.IsEnum)
												   .Select(t => t.Namespace().Root)
												   .Distinct()
												   .OrderBy(ns => ns.DisplayName(false)))
				{
					yield return new NamespaceOption(@namespace, FuzzyOptionMode.Branch);
				}
			}
			else
			{
				throw new UnexpectedEnumValueException<RootMode>(rootMode);
			}
		}

		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			if (parent is NamespaceOption namespaceOption)
			{
				var @namespace = namespaceOption.value;

				if (!@namespace.IsGlobal)
				{
					var childNamespaces = codebase.members
													.Select(m => m.targetType)
													.Distinct()
													.Where(t => !t.IsEnum)
													.Select(t => t.Namespace())
													.Distinct()
													.Where(ns => ns.Parent == @namespace);

					if (ordered)
					{
						childNamespaces = childNamespaces.OrderBy(ns => ns.DisplayName(false));
					}

					foreach (var childNamespace in childNamespaces)
					{
						yield return new NamespaceOption(childNamespace, FuzzyOptionMode.Branch);
					}
				}

				var childTypes = codebase.members
											 .Select(m => m.targetType)
											 .Where(t => !t.IsEnum)
											 .Distinct()
											 .Where(t => t.Namespace() == @namespace);

				if (ordered)
				{
					childTypes = childTypes.OrderBy(t => t.DisplayName());
				}

				foreach (var type in childTypes)
				{
					yield return new TypeOption(type, FuzzyOptionMode.Branch);
				}
			}
			else if (parent is TypeOption typeOption)
			{
				var childMembers = codebase.members.Where(m => m.targetType == typeOption.value);

				if (ordered)
				{
					childMembers = childMembers
										.OrderBy(m => LudiqCore.Configuration.groupInheritedMembers && m.isPseudoInherited)
										.ThenBy(m => m.info.DisplayName());
				}

				foreach (var member in childMembers)
				{
					yield return new MemberOption(member);
				}
			}
			else if (parent is MemberOption)
			{
				yield break;
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		#region Search

		public override bool searchable { get; } = true;

		public override IEnumerable<IFuzzyOption> OrderedSearchResults(string query, IFuzzyOption parent, CancellationToken cancellation)
		{
			// Exclude duplicate inherited members, like the high amount of "Destroy()" or "enabled",
			// if their declaring type is also available for search.

			foreach (var memberOption in codebase.members
			                               .Cancellable(cancellation)
										   .Select(m => new MemberOption(m))
										   .UnorderedSearchFilter(query, m => m.Haystack(action, expectingBoolean))
										   .Where(m => !m.value.isPseudoInherited || !codebase.types.Contains(m.value.info.DeclaringOrExtendedType()))
										   .OrderBy(m => LudiqCore.Configuration.groupInheritedMembers && m.value.isPseudoInherited)
										   .ThenByDescending(m => SearchUtility.Relevance(query, m.Haystack(action, expectingBoolean))))
			{
				yield return memberOption;
			}

			foreach (var typeOption in codebase.types
										 .Select(t => new TypeOption(t, FuzzyOptionMode.Branch))
			                             .Where(t => !t.value.IsEnum)
			                             .OrderedSearchFilter(query, t => t.haystack, cancellation))
			{
				yield return typeOption;
			}
		}

		public override string SearchResultLabel(IFuzzyOption item, string query)
		{
			if (item is MemberOption memberOption)
			{
				return memberOption.SearchResultLabel(query, action, expectingBoolean);
			}

			return base.SearchResultLabel(item, query);
		}

		#endregion

		#region Favorites

		public override ICollection<IFuzzyOption> favorites { get; }

		public override string ExplicitLabel(IFuzzyOption item)
		{
			if (item is NamespaceOption namespaceOption)
			{
				return namespaceOption.value.DisplayName();
			}
			else if (item is TypeOption typeOption)
			{
				return typeOption.value.DisplayName();
			}
			else if (item is MemberOption memberOption)
			{
				var member = memberOption.value;

				if (member.isInvocable)
				{
					return $"{member.info.DisplayName(action, expectingBoolean)} ({member.methodBase.DisplayParameterString()})";
				}
				else
				{
					return member.info.DisplayName(action, expectingBoolean);
				}
			}

			throw new NotSupportedException();
		}

		public override bool CanFavorite(IFuzzyOption item)
		{
			return item is MemberOption;
		}

		public override void OnFavoritesChange()
		{
			LudiqCore.Configuration.Save();
		}

		private class Favorites : ICollection<IFuzzyOption>
		{
			public Favorites(MemberOptionTree tree)
			{
				this.tree = tree;
			}

			private readonly MemberOptionTree tree;

			private IEnumerable<Member> validFavorites => LudiqCore.Configuration.favoriteMembers.Where(tree.codebase.ContainsMember);

			public int Count => validFavorites.Count();

			public bool IsReadOnly => false;

			public IEnumerator<IFuzzyOption> GetEnumerator()
			{
				foreach (var favorite in validFavorites)
				{
					yield return new MemberOption(favorite);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public bool Contains(IFuzzyOption item)
			{
				return validFavorites.Contains((Member)item.value);
			}

			public void Add(IFuzzyOption item)
			{
				favorites.Add((Member)item.value);
			}

			public bool Remove(IFuzzyOption item)
			{
				return favorites.Remove((Member)item.value);
			}

			public void Clear()
			{
				favorites.Clear();
			}

			public void CopyTo(IFuzzyOption[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex));
				}

				if (array.Length - arrayIndex < Count)
				{
					throw new ArgumentException();
				}

				var i = 0;

				foreach (var item in this)
				{
					array[i + arrayIndex] = item;
					i++;
				}
			}

			private static readonly HashSet<Member> favorites = new HashSet<Member>(LudiqCore.Configuration.favoriteMembers);
		}

		#endregion
	}
}