using System;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public class LooseAssemblyName
	{
		[SerializeAs(nameof(name))] 
		private string _name;

		[DoNotSerialize]
		public string name => _name;

		[Obsolete(Serialization.ConstructorWarning)]
		public LooseAssemblyName()
		{
			_name = null;
		}

		public LooseAssemblyName(string name)
		{
			Ensure.That(nameof(name)).IsNotNull(name);

			_name = name;
		}

		public override bool Equals(object obj)
		{
			if (obj is LooseAssemblyName other)
			{
				return other.name == name;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return HashUtility.GetHashCode(name);
		}

		public static bool operator ==(LooseAssemblyName a, LooseAssemblyName b)
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

		public static bool operator !=(LooseAssemblyName a, LooseAssemblyName b)
		{
			return !(a == b);
		}

		public static implicit operator LooseAssemblyName(string name)
		{
			return new LooseAssemblyName(name);
		}

		public static implicit operator string(LooseAssemblyName name)
		{
			return name.name;
		}

		public static explicit operator LooseAssemblyName(AssemblyName strongAssemblyName)
		{
			return new LooseAssemblyName(strongAssemblyName.Name);
		}

		public override string ToString()
		{
			return name;
		}
	}
}