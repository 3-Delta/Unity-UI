using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class InstanceActionInvokerBase<TTarget> : InstanceInvokerBase<TTarget>
	{
		protected InstanceActionInvokerBase(MethodInfo methodInfo) : base(methodInfo) { }
	}
}