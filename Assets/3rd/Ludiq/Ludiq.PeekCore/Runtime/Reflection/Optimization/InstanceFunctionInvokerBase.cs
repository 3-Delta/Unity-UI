using System;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class InstanceFunctionInvokerBase<TTarget, TResult> : InstanceInvokerBase<TTarget>
	{
		protected InstanceFunctionInvokerBase(MethodInfo methodInfo) : base(methodInfo)
		{
			if (OptimizedReflection.safeMode)
			{
				if (methodInfo.ReturnType != typeof(TResult))
				{
					throw new ArgumentException("Return type of method info doesn't match generic type.", nameof(methodInfo));
				}
			}
		}
	}
}