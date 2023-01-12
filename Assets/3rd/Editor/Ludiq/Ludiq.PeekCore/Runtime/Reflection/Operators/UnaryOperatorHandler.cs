using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class UnaryOperatorHandler : OperatorHandler
	{
		private readonly Dictionary<Type, Func<object, object>> manualHandlers = new Dictionary<Type, Func<object, object>>();
		private readonly Dictionary<Type, IOptimizedInvoker> userDefinedOperators = new Dictionary<Type, IOptimizedInvoker>();
		private readonly Dictionary<Type, Type> userDefinedOperandTypes = new Dictionary<Type, Type>();

		protected UnaryOperatorHandler(OperatorCategory category, UnaryOperator @operator, string name, string verb, string symbol, string fancySymbol, string customMethodName)
			: base(category, name, verb, symbol, fancySymbol, customMethodName)
		{
			this.@operator = @operator;
		}

		public UnaryOperator @operator { get; }
		public IEnumerable<Type> manualOperatorQueries => manualHandlers.Keys;

		public abstract string GetDescriptionFormat(Type type);

		private void PopulateUserDefinedOperators(Type type)
		{
			if (!userDefinedOperators.ContainsKey(type))
			{
				var method = type.GetMethod(customMethodName, BindingFlags.Public | BindingFlags.Static);

				if (method != null)
				{
					userDefinedOperandTypes.Add(type, ResolveUserDefinedOperandType(method));
				}

				userDefinedOperators.Add(type, method?.Prewarm());
			}
		}

		public object Operate(object operand)
		{
			Ensure.That(nameof(operand)).IsNotNull(operand);

			var type = operand.GetType();

			if (manualHandlers.ContainsKey(type))
			{
				return manualHandlers[type](operand);
			}

			if (customMethodName != null)
			{
				PopulateUserDefinedOperators(type);

				if (userDefinedOperators[type] != null)
				{
					operand = ConversionUtility.Convert(operand, userDefinedOperandTypes[type]);

					return userDefinedOperators[type].Invoke(null, operand);
				}
			}

			return CustomHandling(operand);
		}

		public virtual bool Supports(Type type)
		{
			if (manualHandlers.ContainsKey(type))
			{
				return true;
			}

			PopulateUserDefinedOperators(type);

			if (userDefinedOperators[type] != null)
			{
				return true;
			}

			return HasCustomHandling(type);
		}

		protected virtual object CustomHandling(object operand)
		{
			throw new InvalidOperatorException(symbol, operand.GetType());
		}

		protected virtual bool HasCustomHandling(Type type)
		{
			return false;
		}

		protected void Handle<T>(Func<T, object> handler)
		{
			manualHandlers.Add(typeof(T), operand => handler((T)operand));
		}

		private static Type ResolveUserDefinedOperandType(MethodInfo userDefinedOperator)
		{
			// See comment in BinaryOperatorHandler
			return userDefinedOperator.GetParameters()[0].ParameterType;
		}
	}
}