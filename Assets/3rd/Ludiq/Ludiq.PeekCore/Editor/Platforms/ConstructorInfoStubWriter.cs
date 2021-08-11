using System.Collections.Generic;
using System.Reflection;
using Ludiq.PeekCore.CodeDom;
using Ludiq.PeekCore;

[assembly: RegisterAotStubWriter(typeof(ConstructorInfo), typeof(ConstructorInfoStubWriter))]

namespace Ludiq.PeekCore
{
	public class ConstructorInfoStubWriter : MethodBaseStubWriter<ConstructorInfo>
	{
		public ConstructorInfoStubWriter(ConstructorInfo constructorInfo) : base(constructorInfo) { }
		
		protected override bool supportsOptimization => false;

		public override IEnumerable<CodeStatement> GetStubStatements()
		{
			/* 
			 * Required output:
			 * 1. Call the constructor with the correct number of args
			 * (No optimization available for constructors)
			*/

			var arguments = new List<CodeExpression>();

			foreach (var parameterInfo in stub.GetParameters())
			{
				var parameterType = Code.TypeRef(parameterInfo.UnderlyingParameterType(), true);
				var argumentName = $"arg{arguments.Count}";

				yield return Code.VarDecl(parameterType, argumentName, parameterType.DefaultValue());

				CodeParameterDirection direction;

				if (parameterInfo.IsOut)
				{
					direction = CodeParameterDirection.Out;
				}
				else if (parameterInfo.ParameterType.IsByRef)
				{
					direction = CodeParameterDirection.Ref;
				}
				else
				{
					direction = CodeParameterDirection.Default;
				}

				var argument = Code.ArgumentDirection(direction, Code.VarRef(argumentName));

				arguments.Add(argument);
			}

			if (manipulator.isPubliclyInvocable)
			{
				yield return Code.TypeRef(stub.DeclaringType).ObjectCreate(arguments).Statement();
			}
		}
	}
}