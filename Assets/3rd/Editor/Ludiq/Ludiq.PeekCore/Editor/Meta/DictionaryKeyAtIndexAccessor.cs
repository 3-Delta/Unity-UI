using System;
using System.Collections;
using System.Linq;

namespace Ludiq.PeekCore
{
	public sealed class DictionaryKeyAtIndexAccessor : DictionaryIndexAccessor
	{
		public DictionaryKeyAtIndexAccessor(int index, Accessor parent) : base(SubpathPrefix, index, parent) { }

		protected override object rawValue
		{
			get
			{
				return ((IDictionary)parent.value).Keys.Cast<object>().ElementAt(index);
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		protected override Type GetDefinedType(Type dictionaryType)
		{
			return TypeUtility.GetDictionaryKeyType(dictionaryType, true);
		}

		public const string SubpathPrefix = "__keyAt.";
	}
}