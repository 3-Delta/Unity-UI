using System;

namespace Ludiq.PeekCore
{
	public class UnexpectedEnumValueException<T> : Exception
	{
		public UnexpectedEnumValueException(T value) : base("Value " + value + " of enum " + typeof(T).Name + " is unexpected.")
		{
			Value = value;
		}

		public T Value { get; private set; }
	}
}