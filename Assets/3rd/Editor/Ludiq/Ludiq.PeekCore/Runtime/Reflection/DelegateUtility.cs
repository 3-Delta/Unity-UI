using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ludiq.PeekCore
{
	public static class DelegateUtility
	{
		// https://stackoverflow.com/questions/5658765/
		public static Delegate BuildDynamicHandle(Type delegateType, Func<object[], object> func)
		{
			var invokeMethod = delegateType.GetMethod("Invoke");
			var parameters = invokeMethod.GetParameters().Select(parm => Expression.Parameter(parm.ParameterType, parm.Name)).ToArray();
			var instance = func.Target == null ? null : Expression.Constant(func.Target);
			var converted = parameters.Select(parm => Expression.Convert(parm, typeof(object)));
			var call = Expression.Call(instance, func.Method, Expression.NewArrayInit(typeof(object), converted));
			var body = invokeMethod.ReturnType == typeof(void) ? (Expression)call : Expression.Convert(call, invokeMethod.ReturnType);
			var expression = Expression.Lambda(delegateType, body, parameters);
			return expression.Compile();
		}
	}
}