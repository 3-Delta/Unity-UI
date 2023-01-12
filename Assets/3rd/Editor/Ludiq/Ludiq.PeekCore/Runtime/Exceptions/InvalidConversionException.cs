using System;

namespace Ludiq.PeekCore
{
	public class InvalidConversionException : InvalidCastException
	{
		public InvalidConversionException() : base() { }
		public InvalidConversionException(string message) : base(message) { }
		public InvalidConversionException(string message, Exception innerException) : base(message, innerException) { }
	}
}