using System.Linq;

namespace Ludiq.PeekCore
{
	public static class XString
	{
		public static string Inject(this string format, params object[] formattingArgs)
		{
			return string.Format(format, formattingArgs);
		}

		public static string Inject(this string format, params string[] formattingArgs)
		{
			return string.Format(format, formattingArgs.Select(a => a as object).ToArray());
		}
	}
}