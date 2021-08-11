using System;

namespace Ludiq.PeekCore
{
	public class InvalidImplementationException : Exception
	{
		public InvalidImplementationException() : base() { }
		public InvalidImplementationException(string message) : base(message) { }
	}
}