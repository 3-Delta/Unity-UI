namespace Ludiq.PeekCore
{
	public sealed class MemberData
	{
		public Member.Source source;
		public TypeData reflectedType;
		public TypeData targetType;
		public string name;
		public bool isOpenConstructed;
		public bool isExtension;
		public string[] parameterTypeNames;
		public string[] parameterOpenTypeNames;
		public string[] genericMethodTypeArgumentNames;

		public Member ToMember()
		{
			return new Member(this);
		}
	}
}
