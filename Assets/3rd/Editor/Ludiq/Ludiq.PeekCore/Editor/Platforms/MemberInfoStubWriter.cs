using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class MemberInfoStubWriter<T> : AotStubWriter where T : MemberInfo
	{
		protected MemberInfoStubWriter(T memberInfo) : base(memberInfo)
		{
			stub = memberInfo;
			manipulator = stub.ToMember();
		}

		public new T stub { get; }
		protected Member manipulator { get; }

		public override string stubMethodComment => stub.ReflectedType.CSharpFullName() + "." + stub.Name;

		public override string stubMethodName => stubMethodComment.FilterReplace('_', true, symbols: false, whitespace: false, punctuation: false);

		public override bool skip => !Codebase.IsRuntimeType(stub.ReflectedType) || stub.ReflectedType.ContainsGenericParameters;

		protected abstract bool supportsOptimization { get; }
	}
}