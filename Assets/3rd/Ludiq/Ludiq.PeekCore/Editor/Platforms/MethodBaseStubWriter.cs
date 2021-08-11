using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class MethodBaseStubWriter<TMethodBase> : MemberInfoStubWriter<TMethodBase> where TMethodBase : MethodBase
	{
		protected MethodBaseStubWriter(TMethodBase methodBase) : base(methodBase) { }
	}
}