using System;

namespace Ludiq.PeekCore
{
	public abstract class OperatorException : InvalidCastException
	{
		protected OperatorException() : base() { }
		protected OperatorException(string message) : base(message) { }
		protected OperatorException(string message, Exception innerException) : base(message, innerException) { }
	}
}