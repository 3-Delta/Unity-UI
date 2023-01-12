using System;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public static class ExceptionUtility
	{
		public static Exception Relevant(this Exception ex)
		{
			if (ex is TargetInvocationException)
			{
				return ex.InnerException;
			}
			else
			{
				return ex;
			}
		}
	}
}