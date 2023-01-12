using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ludiq.PeekCore.CodeDom;
using Ludiq.PeekCore;

[assembly: RegisterAotStubWriter(typeof(MethodInfo), typeof(MethodInfoStubWriter))]

namespace Ludiq.PeekCore
{
	public class MethodInfoStubWriter : MethodBaseStubWriter<MethodInfo>
	{
		public MethodInfoStubWriter(MethodInfo methodInfo) : base(methodInfo) { }

		protected override bool supportsOptimization => stub.SupportsOptimization();

		public override IEnumerable<CodeStatement> GetStubStatements()
		{
			/* 
			 * Required output:
			 * 1. Create a target expression
			 * 2. Call its method with the correct number of args
			 * 3. Call its optimized method with the correct number of args
			 * 4. Call its optimized method with an args array
			*/

			var targetType = Code.TypeRef(manipulator.targetType, true);
			var declaringType = Code.TypeRef(stub.DeclaringType, true);

			CodeExpression targetValue;
			CodeExpression targetReference;

			if (manipulator.requiresTarget && !manipulator.isExtension)
			{
				// default(Material)
				targetValue = targetType.DefaultValue();

				// 1. Material target = default(Material);
				yield return Code.VarDecl(targetType, "target", targetValue);

				targetReference = Code.VarRef("target");
			}
			else
			{
				targetValue = Code.Primitive(null);
				targetReference = targetType.Expression();

				if (manipulator.isExtension)
				{
					// 1. ShortcutExtensions
					targetReference = new CodeTypeReferenceExpression(declaringType); 
				}
				else
				{
					// 1. Material
					targetReference = new CodeTypeReferenceExpression(targetType);
				}
			}

			// target.SetColor
			var methodReference = targetReference.Method(manipulator.name);

			var arguments = new List<CodeExpression>();

			var includesOutOrRef = false;

			foreach (var parameterInfo in stub.GetParameters())
			{
				var parameterType = Code.TypeRef(parameterInfo.UnderlyingParameterType(), true);
				var argumentName = $"arg{arguments.Count}";

				// arg0 = default(string)
				// arg1 = default(Color)
				yield return Code.VarDecl(parameterType, argumentName, parameterType.DefaultValue());

				CodeParameterDirection direction;

				if (parameterInfo.IsOut)
				{
					direction = CodeParameterDirection.Out;
					includesOutOrRef = true;
				}
				else if (parameterInfo.ParameterType.IsByRef)
				{
					direction = CodeParameterDirection.Ref;
					includesOutOrRef = true;
				}
				else
				{
					direction = CodeParameterDirection.Default;
				}

				var argument = Code.ArgumentDirection(direction, Code.VarRef(argumentName));

				arguments.Add(argument);
			}

			if (operatorTypes.ContainsKey(manipulator.name))
			{
				// arg0 * arg1
				var operation = arguments[0].BinaryOp(operatorTypes[manipulator.name], arguments[1]);

				// 2. var operator = arg0 * arg1;
				yield return Code.VarDecl(Code.TypeRef(manipulator.type), "operator", operation);
			}
			else if (manipulator.isConversion)
			{
				// (Vector3)arg0
				var cast = arguments[0].Cast(Code.TypeRef(manipulator.type));

				// 2. var conversion = (Vector3)arg0;
				yield return Code.VarDecl(Code.TypeRef(manipulator.type), "conversion", cast);
			}
			else if (manipulator.isPubliclyInvocable && !manipulator.isConversion)
			{
				// 2. target.SetColor(arg0, arg1);
				yield return methodReference.Invoke(arguments.ToArray()).Statement();
			}

			if (supportsOptimization)
			{
				var optimizedInvokerType = Code.TypeRef(stub.Prewarm().GetType(), true);

				// var invoker = new InstanceActionInvoker<Material, string, Color>(default(MethodInfo));
				yield return Code.VarDecl(optimizedInvokerType, "optimized", optimizedInvokerType.ObjectCreate(Code.TypeRef(typeof(MethodInfo), true).DefaultValue()));

				// [default(Material), arg0, arg1]
				var argumentsWithTarget = targetValue.Yield().Concat(arguments).ToArray();

				// Ref and out parameters are not supported in the numbered argument signatures
				if (!includesOutOrRef)
				{
					// 3. invoker.Invoke(default(Material), arg0, arg1);
					yield return Code.VarRef("optimized").Method(nameof(IOptimizedInvoker.Invoke)).Invoke(argumentsWithTarget).Statement();
				}

				// 4. invoker.Invoke(default(Material), default(object[]));
				yield return Code.VarRef("optimized").Method(nameof(IOptimizedInvoker.Invoke)).Invoke(Code.TypeRef(typeof(object[])).DefaultValue()).Statement();
			}
		}

		public static readonly Dictionary<string, CodeBinaryOperatorType> operatorTypes = new Dictionary<string, CodeBinaryOperatorType>
		{
			{ "op_Addition", CodeBinaryOperatorType.Add },
			{ "op_Subtraction", CodeBinaryOperatorType.Subtract },
			{ "op_Multiply", CodeBinaryOperatorType.Multiply },
			{ "op_Division", CodeBinaryOperatorType.Divide },
			{ "op_Modulus", CodeBinaryOperatorType.Modulo },
			{ "op_BitwiseAnd", CodeBinaryOperatorType.BitwiseAnd },
			{ "op_BitwiseOr", CodeBinaryOperatorType.BitwiseOr },
			{ "op_LogicalAnd", CodeBinaryOperatorType.LogicalAnd },
			{ "op_LogicalOr", CodeBinaryOperatorType.LogicalOr },
			{ "op_Equality", CodeBinaryOperatorType.Equality },
			{ "op_GreaterThan", CodeBinaryOperatorType.GreaterThan },
			{ "op_LessThan", CodeBinaryOperatorType.LessThan },
			{ "op_Inequality", CodeBinaryOperatorType.Inequality },
			{ "op_GreaterThanOrEqual", CodeBinaryOperatorType.GreaterThanOrEqual },
			{ "op_LessThanOrEqual", CodeBinaryOperatorType.LessThanOrEqual }
		};
	}
}