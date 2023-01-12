using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public static class OperatorUtility
	{
		static OperatorUtility()
		{
            UnaryHandler(bitwiseNegationHandler.@operator, bitwiseNegationHandler);
            UnaryHandler(numericNegationHandler.@operator, numericNegationHandler);
            UnaryHandler(incrementHandler.@operator, incrementHandler);
            UnaryHandler(decrementHandler.@operator, decrementHandler);
            UnaryHandler(plusHandler.@operator, plusHandler);

			BinaryHandler(additionHandler.@operator, additionHandler);
            BinaryHandler(subtractionHandler.@operator, subtractionHandler);
            BinaryHandler(multiplicationHandler.@operator, multiplicationHandler);
            BinaryHandler(divisionHandler.@operator, divisionHandler);
            BinaryHandler(moduloHandler.@operator, moduloHandler);
            BinaryHandler(bitwiseAndHandler.@operator, bitwiseAndHandler);
            BinaryHandler(bitwiseOrHandler.@operator, bitwiseOrHandler);
            BinaryHandler(bitwiseExclusiveOrHandler.@operator, bitwiseExclusiveOrHandler);
            BinaryHandler(equalityHandler.@operator, equalityHandler);
            BinaryHandler(inequalityHandler.@operator, inequalityHandler);
            BinaryHandler(greaterThanHandler.@operator, greaterThanHandler);
            BinaryHandler(lessThanHandler.@operator, lessThanHandler);
            BinaryHandler(greaterThanOrEqualHandler.@operator, greaterThanOrEqualHandler);
            BinaryHandler(lessThanOrEqualHandler.@operator, lessThanOrEqualHandler);
            BinaryHandler(leftShiftHandler.@operator, leftShiftHandler);
            BinaryHandler(rightShiftHandler.@operator, rightShiftHandler);
		}

		private static void UnaryHandler(UnaryOperator @operator, UnaryOperatorHandler handler)
		{
			unaryOperatorHandlers.Add(@operator, handler);
			operatorHandlers.Add(handler);
		}

		private static void BinaryHandler(BinaryOperator @operator, BinaryOperatorHandler handler)
		{
			binaryOperatorHandlers.Add(@operator, handler);
			operatorHandlers.Add(handler);
		}

		// https://msdn.microsoft.com/en-us/library/2sk3x8a7(v=vs.71).aspx
		public static readonly Dictionary<string, string> operatorSymbols = new Dictionary<string, string>
		{
			{ "op_Addition", "+" },
			{ "op_Subtraction", "-" },
			{ "op_Multiply", "*" },
			{ "op_Division", "/" },
			{ "op_Modulus", "%" },
			{ "op_ExclusiveOr", "^" },
			{ "op_BitwiseAnd", "&" },
			{ "op_BitwiseOr", "|" },
			{ "op_LogicalAnd", "&&" },
			{ "op_LogicalOr", "||" },
			{ "op_LeftShift", "<<" },
			{ "op_RightShift", ">>" },
			{ "op_Equality", "==" },
			{ "op_GreaterThan", ">" },
			{ "op_LessThan", "<" },
			{ "op_Inequality", "!=" },
			{ "op_GreaterThanOrEqual", ">=" },
			{ "op_LessThanOrEqual", "<=" },
			{ "op_Decrement", "--" },
			{ "op_Increment", "++" },
			{ "op_UnaryNegation", "-" },
			//{ "op_UnaryPlus", "+" },
			{ "op_OnesComplement", "~" },
		};

		public static readonly Dictionary<string, string> operatorAlternativeNames = new Dictionary<string, string>
		{
			{ "op_Addition", "Add" },
			{ "op_Subtraction", "Subtract" },
			{ "op_Multiply", "Multiply" },
			{ "op_Division", "Divide" },
			{ "op_Modulus", "Modulo" },
			{ "op_ExclusiveOr", "ExclusiveOr" },
			{ "op_BitwiseAnd", "BitwiseAnd" },
			{ "op_BitwiseOr", "BitwiseOr" },
			{ "op_LogicalAnd", "LogicalAnd" },
			{ "op_LogicalOr", "LogicalOr" },
			{ "op_LeftShift", "LeftShift" },
			{ "op_RightShift", "RightShift" },
			{ "op_Equality", "Equals" },
			{ "op_GreaterThan", "GreaterThan" },
			{ "op_LessThan", "LessThan" },
			{ "op_Inequality", "NotEquals" },
			{ "op_GreaterThanOrEqual", "GreaterThanOrEquals" },
			{ "op_LessThanOrEqual", "LessThanOrEquals" },
			{ "op_Decrement", "Decrement" },
			{ "op_Increment", "Increment" },
			{ "op_UnaryNegation", "Negate" },
			{ "op_UnaryPlus", "Positive" },
			{ "op_OnesComplement", "OnesComplement" },
		};

		public static readonly Dictionary<string, string> operatorHumanNames = new Dictionary<string, string>
		{
			{ "op_Addition", "Add" },
			{ "op_Subtraction", "Subtract" },
			{ "op_Multiply", "Multiply" },
			{ "op_Division", "Divide" },
			{ "op_Modulus", "Modulo" },
			{ "op_ExclusiveOr", "Exclusive Or" },
			{ "op_BitwiseAnd", "Bitwise And" },
			{ "op_BitwiseOr", "Bitwise Or" },
			{ "op_LogicalAnd", "Logical And" },
			{ "op_LogicalOr", "Logical Or" },
			{ "op_LeftShift", "Left Shift" },
			{ "op_RightShift", "Right Shift" },
			{ "op_Equality", "Equals" },
			{ "op_GreaterThan", "Greater Than" },
			{ "op_LessThan", "Less Than" },
			{ "op_Inequality", "Not Equals" },
			{ "op_GreaterThanOrEqual", "Greater Than Or Equals" },
			{ "op_LessThanOrEqual", "Less Than Or Equals" },
			{ "op_Decrement", "Decrement" },
			{ "op_Increment", "Increment" },
			{ "op_UnaryNegation", "Negate" },
			{ "op_UnaryPlus", "Positive" },
			{ "op_OnesComplement", "One's Complement" },
		};

		public static readonly Dictionary<string, int> operatorRanks = new Dictionary<string, int>
		{
			{ "op_Addition", 2 },
			{ "op_Subtraction", 2 },
			{ "op_Multiply", 2 },
			{ "op_Division", 2 },
			{ "op_Modulus", 2 },
			{ "op_ExclusiveOr", 2 },
			{ "op_BitwiseAnd", 2 },
			{ "op_BitwiseOr", 2 },
			{ "op_LogicalAnd", 2 },
			{ "op_LogicalOr", 2 },
			{ "op_Assign", 2 },
			{ "op_LeftShift", 2 },
			{ "op_RightShift", 2 },
			{ "op_Equality", 2 },
			{ "op_GreaterThan", 2 },
			{ "op_LessThan", 2 },
			{ "op_Inequality", 2 },
			{ "op_GreaterThanOrEqual", 2 },
			{ "op_LessThanOrEqual", 2 },
			{ "op_Decrement", 1 },
			{ "op_Increment", 1 },
			{ "op_UnaryNegation", 1 },
			{ "op_UnaryPlus", 1 },
			{ "op_OnesComplement", 1 },
		};

		public static readonly Dictionary<string, BinaryOperator> methodNamesToBinaryOperators = new Dictionary<string, BinaryOperator>
		{
			{ "op_Addition", BinaryOperator.Addition },
			{ "op_Subtraction", BinaryOperator.Subtraction },
			{ "op_Multiply", BinaryOperator.Multiplication },
			{ "op_Division", BinaryOperator.Division },
			{ "op_Modulus", BinaryOperator.Modulo },
			{ "op_ExclusiveOr", BinaryOperator.ExclusiveOr },
			{ "op_BitwiseAnd", BinaryOperator.And },
			{ "op_BitwiseOr", BinaryOperator.Or },
			{ "op_LeftShift", BinaryOperator.LeftShift },
			{ "op_RightShift", BinaryOperator.RightShift },
			{ "op_Equality", BinaryOperator.Equality },
			{ "op_GreaterThan", BinaryOperator.GreaterThan },
			{ "op_LessThan", BinaryOperator.LessThan },
			{ "op_Inequality", BinaryOperator.Inequality },
			{ "op_GreaterThanOrEqual", BinaryOperator.GreaterThanOrEqual },
			{ "op_LessThanOrEqual", BinaryOperator.LessThanOrEqual },
		};

		public static readonly Dictionary<string, UnaryOperator> methodNamesToUnaryOperators = new Dictionary<string, UnaryOperator>
		{
			{ "op_Decrement", UnaryOperator.Decrement},
			{ "op_Increment", UnaryOperator.Increment },
			{ "op_UnaryNegation", UnaryOperator.NumericNegation },
			{ "op_UnaryPlus", UnaryOperator.Plus },
			{ "op_OnesComplement", UnaryOperator.BitwiseNegation },
		};

		private static readonly HashSet<OperatorHandler> operatorHandlers = new HashSet<OperatorHandler>();
		private static readonly Dictionary<UnaryOperator, UnaryOperatorHandler> unaryOperatorHandlers = new Dictionary<UnaryOperator, UnaryOperatorHandler>();
		private static readonly Dictionary<BinaryOperator, BinaryOperatorHandler> binaryOperatorHandlers = new Dictionary<BinaryOperator, BinaryOperatorHandler>();

		private static readonly BitwiseNegationHandler bitwiseNegationHandler = new BitwiseNegationHandler();
		private static readonly NumericNegationHandler numericNegationHandler = new NumericNegationHandler();
		private static readonly IncrementHandler incrementHandler = new IncrementHandler();
		private static readonly DecrementHandler decrementHandler = new DecrementHandler();
		private static readonly PlusHandler plusHandler = new PlusHandler();

		private static readonly AdditionHandler additionHandler = new AdditionHandler();
		private static readonly SubtractionHandler subtractionHandler = new SubtractionHandler();
		private static readonly MultiplicationHandler multiplicationHandler = new MultiplicationHandler();
		private static readonly DivisionHandler divisionHandler = new DivisionHandler();
		private static readonly ModuloHandler moduloHandler = new ModuloHandler();
		private static readonly BitwiseAndHandler bitwiseAndHandler = new BitwiseAndHandler();
		private static readonly BitwiseOrHandler bitwiseOrHandler = new BitwiseOrHandler();
		private static readonly BitwiseExclusiveOrHandler bitwiseExclusiveOrHandler = new BitwiseExclusiveOrHandler();
		private static readonly EqualityHandler equalityHandler = new EqualityHandler();
		private static readonly InequalityHandler inequalityHandler = new InequalityHandler();
		private static readonly GreaterThanHandler greaterThanHandler = new GreaterThanHandler();
		private static readonly LessThanHandler lessThanHandler = new LessThanHandler();
		private static readonly GreaterThanOrEqualHandler greaterThanOrEqualHandler = new GreaterThanOrEqualHandler();
		private static readonly LessThanOrEqualHandler lessThanOrEqualHandler = new LessThanOrEqualHandler();
		private static readonly LeftShiftHandler leftShiftHandler = new LeftShiftHandler();
		private static readonly RightShiftHandler rightShiftHandler = new RightShiftHandler();

		public static UnaryOperatorHandler Handler(this UnaryOperator @operator)
		{
			if (!unaryOperatorHandlers.TryGetValue(@operator, out var handler))
			{
				throw new UnexpectedEnumValueException<UnaryOperator>(@operator);
			}

			return handler;
		}

		public static BinaryOperatorHandler Handler(this BinaryOperator @operator)
		{
			if (!binaryOperatorHandlers.TryGetValue(@operator, out var handler))
			{
				throw new UnexpectedEnumValueException<BinaryOperator>(@operator);
			}

			return handler;
		}

		public static IEnumerable<UnaryOperatorHandler> GetUnaryOperatorHandlers()
		{
			return unaryOperatorHandlers.Values;
		}

		public static IEnumerable<BinaryOperatorHandler> GetBinaryOperatorHandlers()
		{
			return binaryOperatorHandlers.Values;
		}

		public static IEnumerable<OperatorHandler> GetHandlers()
		{
			return operatorHandlers;
		}

		public static string Symbol(this UnaryOperator @operator)
		{
			return @operator.Handler().symbol;
		}

		public static string Symbol(this BinaryOperator @operator)
		{
			return @operator.Handler().symbol;
		}

		public static string Name(this UnaryOperator @operator)
		{
			return @operator.Handler().name;
		}

		public static string Name(this BinaryOperator @operator)
		{
			return @operator.Handler().name;
		}

		public static string Verb(this UnaryOperator @operator)
		{
			return @operator.Handler().verb;
		}

		public static string Verb(this BinaryOperator @operator)
		{
			return @operator.Handler().verb;
		}

		public static object Operate(UnaryOperator @operator, object x)
		{
			if (!unaryOperatorHandlers.ContainsKey(@operator))
			{
				throw new UnexpectedEnumValueException<UnaryOperator>(@operator);
			}

			return unaryOperatorHandlers[@operator].Operate(x);
		}

		public static object Operate(BinaryOperator @operator, object a, object b)
		{
			if (!binaryOperatorHandlers.ContainsKey(@operator))
			{
				throw new UnexpectedEnumValueException<BinaryOperator>(@operator);
			}

			return binaryOperatorHandlers[@operator].Operate(a, b);
		}

		public static object Negate(object x)
		{
			return numericNegationHandler.Operate(x);
		}

		public static object BitwiseNot(object x)
		{
			return bitwiseNegationHandler.Operate(x);
		}

		public static object UnaryPlus(object x)
		{
			return plusHandler.Operate(x);
		}

		public static object Increment(object x)
		{
			return incrementHandler.Operate(x);
		}

		public static object Decrement(object x)
		{
			return decrementHandler.Operate(x);
		}

		public static object BitwiseAnd(object a, object b)
		{
			return bitwiseAndHandler.Operate(a, b);
		}

		public static object BitwiseOr(object a, object b)
		{
			return bitwiseOrHandler.Operate(a, b);
		}

		public static object BitwiseExclusiveOr(object a, object b)
		{
			return bitwiseExclusiveOrHandler.Operate(a, b);
		}

		public static object Add(object a, object b)
		{
			return additionHandler.Operate(a, b);
		}

		public static object Subtract(object a, object b)
		{
			return subtractionHandler.Operate(a, b);
		}

		public static object Multiply(object a, object b)
		{
			return multiplicationHandler.Operate(a, b);
		}

		public static object Divide(object a, object b)
		{
			return divisionHandler.Operate(a, b);
		}

		public static object Modulo(object a, object b)
		{
			return moduloHandler.Operate(a, b);
		}

		public static bool Equal(object a, object b)
		{
			return (bool)equalityHandler.Operate(a, b);
		}

		public static bool NotEqual(object a, object b)
		{
			return (bool)inequalityHandler.Operate(a, b);
		}

		public static bool GreaterThan(object a, object b)
		{
			return (bool)greaterThanHandler.Operate(a, b);
		}

		public static bool LessThan(object a, object b)
		{
			return (bool)lessThanHandler.Operate(a, b);
		}

		public static bool GreaterThanOrEqual(object a, object b)
		{
			return (bool)greaterThanOrEqualHandler.Operate(a, b);
		}

		public static bool LessThanOrEqual(object a, object b)
		{
			return (bool)lessThanOrEqualHandler.Operate(a, b);
		}

		public static object LeftShift(object a, object b)
		{
			return leftShiftHandler.Operate(a, b);
		}

		public static object RightShift(object a, object b)
		{
			return rightShiftHandler.Operate(a, b);
		}

		public static OperatorCategory GetOperatorCategory(this UnaryOperator @operator)
		{
			if (!unaryOperatorHandlers.ContainsKey(@operator))
			{
				throw new UnexpectedEnumValueException<UnaryOperator>(@operator);
			}

			return unaryOperatorHandlers[@operator].category;
		}

		public static OperatorCategory GetOperatorCategory(this BinaryOperator @operator)
		{
			if (!binaryOperatorHandlers.ContainsKey(@operator))
			{
				throw new UnexpectedEnumValueException<BinaryOperator>(@operator);
			}

			return binaryOperatorHandlers[@operator].category;
		}

		public static bool IsExtractable(this UnaryOperator @operator)
		{
			switch (@operator)
			{
				case UnaryOperator.Increment:
				case UnaryOperator.Decrement:
				case UnaryOperator.Plus:
					return false;
				default:
					return true;
			}
		}

		public static bool IsExtractable(this BinaryOperator @operator) => true;

		public static bool TryGetUnaryByMethodName(string name, out UnaryOperator result)
		{
			return methodNamesToUnaryOperators.TryGetValue(name, out result);
		}

		public static bool TryGetBinaryByMethodName(string name, out BinaryOperator result)
		{
			return methodNamesToBinaryOperators.TryGetValue(name, out result);
		}
	}
}