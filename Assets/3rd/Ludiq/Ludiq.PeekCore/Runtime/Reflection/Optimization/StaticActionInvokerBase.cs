using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class StaticActionInvokerBase : StaticInvokerBase
	{
		protected StaticActionInvokerBase(MethodInfo methodInfo) : base(methodInfo) { }
	}
}