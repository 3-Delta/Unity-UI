using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ludiq.PeekCore
{
	public sealed class Namespace
	{
		public const char Separator = '.';

		private Namespace(string fullName)
		{
			FullName = fullName;

			if (fullName != null)
			{
				Parts = FullName.Split(Separator);

				Name = Parts[Parts.Length - 1];

				if (Parts.Length > 1)
				{
					Root = Parts[0];
					Parent = FullName.Substring(0, FullName.LastIndexOf(Separator));
				}
				else
				{
					Root = this;
					IsRoot = true;
					Parent = Global;
				}
			}
			else
			{
				Root = this;
				IsRoot = true;
				IsGlobal = true;
			}
		}
		
		public string[] Parts { get; }
		public Namespace Root { get; }
		public Namespace Parent { get; }
		public string FullName { get; }
		public string Name { get; }
		public bool IsRoot { get; }
		public bool IsGlobal { get; }

		public IEnumerable<Namespace> Ancestors
		{
			get
			{
				var ancestor = Parent;

				while (ancestor != null)
				{
					yield return ancestor;
					ancestor = ancestor.Parent;
				}
			}
		}

		public IEnumerable<Namespace> AndAncestors()
		{
			yield return this;

			foreach (var ancestor in Ancestors)
			{
				yield return ancestor;
			}
		}

		public bool IsAncestorOf(Namespace other)
		{
			Ensure.That(nameof(other)).IsNotNull(other);

			if (other.IsGlobal)
			{
				return false;
			}

			return other.FullName.StartsWith(FullName + ".");
		}

		public bool IsDescendantOf(Namespace other)
		{
			Ensure.That(nameof(other)).IsNotNull(other);

			if (other.IsGlobal)
			{
				return false;
			}

			return FullName.StartsWith(other.FullName + ".");
		}

		public override int GetHashCode()
		{
			if (FullName == null)
			{
				return 0;
			}

			return FullName.GetHashCode();
		}

		public override string ToString()
		{
			return FullName;
		}

		static Namespace()
		{
			interns = new Interns();
		}

		private static readonly Interns interns;

		public static Namespace Global { get; } = new Namespace(null);

		public static Namespace FromFullName(string fullName)
		{
			if (fullName == null)
			{
				return Global;
			}

			lock (interns)
			{
				if (!interns.TryGetValue(fullName, out var @namespace))
				{
					@namespace = new Namespace(fullName);
					interns.Add(@namespace);
				}

				return @namespace;
			}
		}

		public static implicit operator Namespace(string fullName)
		{
			return FromFullName(fullName);
		}

		public static implicit operator string(Namespace @namespace)
		{
			return @namespace.FullName;
		}

		public static Dictionary<Namespace, HashSet<Namespace>> ChildrenByNamespaces(IEnumerable<Namespace> namespaces)
		{
			var result = new Dictionary<Namespace, HashSet<Namespace>>();

			foreach (var ns in namespaces)
			{
				if (ns.Parent != null)
				{
					if (!result.TryGetValue(ns.Parent, out var children))
					{
						children = new HashSet<Namespace>();
						result.Add(ns.Parent, children);
					}

					children.Add(ns);
				} 
			}

			return result;
		}

		public static bool operator ==(Namespace a, Namespace b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(Namespace a, Namespace b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Namespace;

			if (other == null)
			{
				return false;
			}

			return FullName == other.FullName;
		}

		private class Interns : KeyedCollection<string, Namespace>
		{
			protected override string GetKeyForItem(Namespace item)
			{
				return item.FullName;
			}
		}
	}
}