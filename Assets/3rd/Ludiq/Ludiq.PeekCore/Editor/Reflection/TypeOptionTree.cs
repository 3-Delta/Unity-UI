using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class TypeOptionTree : FuzzyOptionTree
	{
		public static TypeOptionTree All { get; } = new TypeOptionTree(Codebase.types, TypeFilter.Any);

		public enum RootMode
		{
			Types,

			Namespaces
		}

		private readonly IEnumerable<Type> typeSet;

		private readonly IEnumerable<TypeOption> typeOptions;

		private readonly TypeFilter filter;

		private HashSet<Type> types;

		private HashSet<Type> enumTypes;

		private HashSet<Namespace> namespaces;

		private HashSet<Namespace> rootNamespaces;

		private Dictionary<Namespace, HashSet<Namespace>> namespacesInNamespace;

		private Dictionary<Namespace, HashSet<Type>> typesInNamespace;

		private TypeOptionTree() : base(new GUIContent("Type")) { }

		public TypeOptionTree(IEnumerable<Type> types) : this()
		{
			Ensure.That(nameof(types)).IsNotNull(types);
			this.types = types.NotNull().ToHashSet();
		}

		public TypeOptionTree(IEnumerable<Type> typeSet, TypeFilter filter) : this()
		{
			Ensure.That(nameof(typeSet)).IsNotNull(typeSet);
			Ensure.That(nameof(filter)).IsNotNull(filter);
			this.typeSet = typeSet;
			this.filter = filter;
		}

		public override void Prewarm()
		{
			base.Prewarm();

			if (types == null)
			{
				types = typeSet.Where(filter.Configured().ValidateType).NotNull().ToHashSet();
			}

			enumTypes = types.Where(t => t.IsEnum).ToHashSet();

			groupEnums = enumTypes.Count != types.Count;

			namespaces = new HashSet<Namespace>();
			typesInNamespace = new Dictionary<Namespace, HashSet<Type>>();

			foreach (var t in types)
			{
				if (!groupEnums || !t.IsEnum)
				{
					var ns = t.Namespace();

					if (!typesInNamespace.TryGetValue(ns, out var children))
					{
						children = new HashSet<Type>();
						typesInNamespace.Add(ns, children);
					}

					children.Add(t);

					namespaces.AddRange(ns.AndAncestors());
				}
			}

			rootNamespaces = namespaces.Select(ns => ns.Root).ToHashSet();
			namespacesInNamespace = Namespace.ChildrenByNamespaces(namespaces);
		}


		#region Configuration

		public RootMode rootMode { get; set; } = RootMode.Namespaces;

		public bool groupEnums { get; set; } = true;

		public bool surfaceCommonTypes { get; set; } = true;

		#endregion



		#region Hierarchy

		private readonly FuzzyGroup enumsGroup = new FuzzyGroup("(Enums)", typeof(Enum).Icon());

		public override IEnumerable<IFuzzyOption> Root()
		{
			if (rootMode == RootMode.Namespaces)
			{
				BeginSeparatorCheck();

				foreach (var earlyOption in EarlyRoot())
				{
					yield return earlyOption;
				}

				foreach (var lateOption in LateRoot(EndSeparatorCheck()))
				{
					yield return lateOption;
				}
			}
			else if (rootMode == RootMode.Types)
			{
				foreach (var type in types)
				{
					yield return new TypeOption(type, FuzzyOptionMode.Leaf);
				}
			}
			else
			{
				throw new UnexpectedEnumValueException<RootMode>(rootMode);
			}
		}

		public IEnumerable<IFuzzyOption> EarlyRoot()
		{
			foreach (var commonType in SeparatorGroup("Common Types", CommonTypes()))
			{
				yield return commonType;
			}
		}

		public IEnumerable<IFuzzyOption> LateRoot(bool separated)
		{
			if (separated)
			{
				yield return Separator("Namespaces");
			}

			foreach (var @namespace in rootNamespaces.OrderBy(ns => ns.DisplayName(false)))
			{
				yield return new NamespaceOption(@namespace, FuzzyOptionMode.Branch);
			}

			if (groupEnums && enumTypes.Any())
			{
				yield return new FuzzyGroupOption(enumsGroup);
			}
		}

		private IEnumerable<IFuzzyOption> CommonTypes()
		{
			foreach (var type in EditorTypeUtility.commonTypes)
			{
				if (types.Contains(type))
				{
					yield return new TypeOption(type, FuzzyOptionMode.Leaf);
				}
			}
		}
		
		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			if (parent is NamespaceOption namespaceOption)
			{
				var @namespace = namespaceOption.value;

				if (!@namespace.IsGlobal)
				{
					var childNamespaces = Enumerable.Empty<Namespace>();
					if (namespacesInNamespace.TryGetValue(@namespace, out var foundNamespaces))
					{
						childNamespaces = foundNamespaces;
					}

					if (ordered)
					{
						childNamespaces = childNamespaces.OrderBy(ns => ns.DisplayName(false));
					}

					foreach (var childNamespace in childNamespaces)
					{
						yield return new NamespaceOption(childNamespace, FuzzyOptionMode.Branch);
					}
				}

				var childTypes = Enumerable.Empty<Type>();
				if (typesInNamespace.TryGetValue(@namespace, out var foundTypes))
				{
					childTypes = foundTypes;
				}

				if (ordered)
				{
					childTypes = childTypes.OrderBy(t => t.DisplayName());
				}

				foreach (var type in childTypes)
				{
					yield return new TypeOption(type, FuzzyOptionMode.Leaf);
				}
			}
			else if (parent.value == enumsGroup)
			{
				var childTypes = enumTypes.AsEnumerable();

				if (ordered)
				{
					childTypes = childTypes.OrderBy(t => t.DisplayName());
				}

				foreach (var type in childTypes)
				{
					yield return new TypeOption(type, FuzzyOptionMode.Leaf);
				}
			}
		}

		#endregion



		#region Search

		public override bool searchable { get; } = true;

		public override IEnumerable<IFuzzyOption> SearchableRoot()
		{
			foreach (var type in types)
			{
				yield return new TypeOption(type, FuzzyOptionMode.Leaf);
			}

			/*
			foreach (var @namespace in namespaces)
			{
				yield return new NamespaceOption(@namespace, FuzzyOptionMode.Leaf);
			} 
			*/
		}

		#endregion
	}
}