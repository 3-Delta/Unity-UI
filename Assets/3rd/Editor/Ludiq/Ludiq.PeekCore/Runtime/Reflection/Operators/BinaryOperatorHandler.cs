using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public abstract class BinaryOperatorHandler : OperatorHandler
	{
		private readonly Dictionary<OperatorQuery, Func<object, object, object>> manualHandlers = new Dictionary<OperatorQuery, Func<object, object, object>>();
		private readonly Dictionary<OperatorQuery, IOptimizedInvoker> userDefinedOperators = new Dictionary<OperatorQuery, IOptimizedInvoker>();
		private readonly Dictionary<OperatorQuery, OperatorQuery> userDefinedOperandTypes = new Dictionary<OperatorQuery, OperatorQuery>();
		private readonly Dictionary<OperatorQuery, Type> resultTypes = new Dictionary<OperatorQuery, Type>();

		protected BinaryOperatorHandler(OperatorCategory category, BinaryOperator @operator, string name, string verb, string symbol, string fancySymbol, string customMethodName)
			: base(category, name, verb, symbol, fancySymbol, customMethodName)
		{
			this.@operator = @operator;
		}

		public BinaryOperator @operator { get; }
		public IEnumerable<OperatorQuery> manualOperatorQueries => manualHandlers.Keys;

		public abstract string GetDescriptionFormat(Type leftType, Type rightType);

		private void PopulateUserDefinedOperators(OperatorQuery query)
		{
			lock (userDefinedOperators)
			{
				if (!userDefinedOperators.ContainsKey(query))
				{
					var leftMethod = query.leftType.GetMethod(customMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] {query.leftType, query.rightType}, null);

					if (query.leftType != query.rightType)
					{
						var rightMethod = query.rightType.GetMethod(customMethodName, BindingFlags.Public | BindingFlags.Static, null, new[] {query.leftType, query.rightType}, null);

						if (leftMethod != null && rightMethod != null)
						{
							throw new AmbiguousOperatorException(symbol, query.leftType, query.rightType);
						}

						var method = (leftMethod ?? rightMethod);

						if (method != null)
						{
							userDefinedOperandTypes.Add(query, ResolveUserDefinedOperandTypes(method));
							resultTypes.Add(query, method.ReturnType);
						}

						userDefinedOperators.Add(query, method?.Prewarm());
					}
					else
					{
						if (leftMethod != null)
						{
							userDefinedOperandTypes.Add(query, ResolveUserDefinedOperandTypes(leftMethod));
							resultTypes.Add(query, leftMethod.ReturnType);
						}

						userDefinedOperators.Add(query, leftMethod?.Prewarm());
					}
				}
			}
		}

		public virtual object Operate(object leftOperand, object rightOperand)
		{
			var leftType = leftOperand?.GetType();
			var rightType = rightOperand?.GetType();

			if (!TryGetOperatorQuery(leftType, rightType, out var query))
			{
				if (leftType == null && rightType == null)
				{
					return BothNullHandling();
				}
				else
				{
					return SingleNullHandling();
				}
			}

			if (manualHandlers.ContainsKey(query))
			{
				return manualHandlers[query](leftOperand, rightOperand);
			}

			if (customMethodName != null)
			{
				PopulateUserDefinedOperators(query);

				if (userDefinedOperators[query] != null)
				{
					leftOperand = ConversionUtility.Convert(leftOperand, userDefinedOperandTypes[query].leftType);
					rightOperand = ConversionUtility.Convert(rightOperand, userDefinedOperandTypes[query].rightType);

					return userDefinedOperators[query].Invoke(null, leftOperand, rightOperand);
				}
			}

			return CustomHandling(leftOperand, rightOperand);
		}

		public bool Supports(Type leftType, Type rightType)
		{
			if (!TryGetOperatorQuery(leftType, rightType, out var query))
			{
				return false;
			}

			if (manualHandlers.ContainsKey(query))
			{
				return true;
			}

			PopulateUserDefinedOperators(query);

			if (userDefinedOperators[query] != null)
			{
				return true;
			}

			return GetCustomHandlingType(leftType, rightType) != null;
		}

		public Type GetResultType(Type leftType, Type rightType)
		{
			if (TryGetOperatorQuery(leftType, rightType, out var query))
			{
				if (resultTypes.TryGetValue(query, out var resultType))
				{
					return resultType;
				}
			}

			var customHandlingType = GetCustomHandlingType(leftType, rightType);

			if (customHandlingType != null)
			{
				return customHandlingType;
			}

			throw new InvalidOperatorException(symbol, leftType, rightType);
		}

		public bool TryGetResultType(Type leftType, Type rightType, out Type type)
		{
			if (Supports(leftType, rightType))
			{
				type = GetResultType(leftType, rightType);
				return true;
			}
			else
			{
				type = null;
				return false;
			}
		}

		private bool TryGetOperatorQuery(Type leftType, Type rightType, out OperatorQuery query)
		{
			if (leftType != null && rightType != null)
			{
				query = new OperatorQuery(leftType, rightType);
				return true;
			}
			else if (leftType != null && leftType.IsNullable())
			{
				query = new OperatorQuery(leftType, leftType);
				return true;
			}
			else if (rightType != null && rightType.IsNullable())
			{
				query = new OperatorQuery(rightType, rightType);
				return true;
			}
			else
			{
				query = new OperatorQuery();
				return false;
			}
		}

		protected virtual object CustomHandling(object leftOperand, object rightOperand)
		{
			throw new InvalidOperatorException(symbol, leftOperand?.GetType(), rightOperand?.GetType());
		}

		protected virtual Type GetCustomHandlingType(Type leftType, Type rightType)
		{
			return null;
		}

		protected virtual object BothNullHandling()
		{
			throw new InvalidOperatorException(symbol, null, null);
		}

		protected virtual object SingleNullHandling()
		{
			throw new InvalidOperatorException(symbol, null, null);
		}

		protected void Handle<TLeft, TRight>(Func<TLeft, TRight, object> handler, Type resultType)
		{
			var query = new OperatorQuery(typeof(TLeft), typeof(TRight));

			if (manualHandlers.ContainsKey(query))
			{
				throw new ArgumentException($"A handler is already registered for '{typeof(TLeft)} {symbol} {typeof(TRight)}'.");
			}

			manualHandlers.Add(query, (left, right) => handler((TLeft)left, (TRight)right));
			resultTypes.Add(query, resultType);
		}

		private static OperatorQuery ResolveUserDefinedOperandTypes(MethodInfo userDefinedOperator)
		{
			// We will need to convert the operands to the argument types,
			// because .NET is actually permissive of implicit conversion 
			// in its GetMethod calls. For example, an operator requiring
			// a float operand will accept an int. However, our optimized
			// reflection code is more strict, and will only accept the
			// exact type, hence why we need to manually store the expected
			// parameter types here to convert them later.
			var parameters = userDefinedOperator.GetParameters();
			return new OperatorQuery(parameters[0].ParameterType, parameters[1].ParameterType);
		}

		public struct OperatorQuery : IEquatable<OperatorQuery>
		{
			public readonly Type leftType;
			public readonly Type rightType;

			public OperatorQuery(Type leftType, Type rightType)
			{
				this.leftType = leftType;
				this.rightType = rightType;
			}

			public bool Equals(OperatorQuery other)
			{
				return
					leftType == other.leftType &&
					rightType == other.rightType;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is OperatorQuery))
				{
					return false;
				}

				return Equals((OperatorQuery)obj);
			}

			public override int GetHashCode()
			{
				return HashUtility.GetHashCode(leftType, rightType);
			}
		}
	}
}