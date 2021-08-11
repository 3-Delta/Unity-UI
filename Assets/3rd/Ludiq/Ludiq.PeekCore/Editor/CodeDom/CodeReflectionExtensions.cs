using System.Reflection;

namespace Ludiq.PeekCore.CodeDom
{
	public static class CodeReflectionExtensions
	{
		public static CodeParameterDirection CodeDirection(this ParameterInfo parameterInfo)
		{
			if (parameterInfo.IsOut)
			{
				return CodeParameterDirection.Out;
			}
			else if (parameterInfo.ParameterType.IsByRef)
			{
				return CodeParameterDirection.Ref;
			}
			else
			{
				return CodeParameterDirection.Default;
			}
		}
	}
}
