namespace Ludiq.PeekCore
{
	public static class EditorFilteringUtility
	{
		public static void Configure(this TypeFilter typeFilter)
		{
			typeFilter.Obsolete |= LudiqCore.Configuration.obsoleteOptions;
		}

		public static void Configure(this MemberFilter memberFilter)
		{
			memberFilter.Obsolete |= LudiqCore.Configuration.obsoleteOptions;
		}

		public static TypeFilter Configured(this TypeFilter typeFilter)
		{
			typeFilter = typeFilter.Clone();
			return typeFilter;
		}

		public static MemberFilter Configured(this MemberFilter memberFilter)
		{
			memberFilter = memberFilter.Clone();
			memberFilter.Configure();
			return memberFilter;
		}
	}
}