using System.Collections.Generic;
using System.Reflection;
using Ludiq.PeekCore.CodeDom;

namespace Ludiq.PeekCore
{
	public abstract class AccessorInfoStubWriter<TAccessor> : MemberInfoStubWriter<TAccessor> where TAccessor : MemberInfo
	{
		protected AccessorInfoStubWriter(TAccessor accessorInfo) : base(accessorInfo) { }

		protected abstract IOptimizedAccessor GetOptimizedAccessor(TAccessor accessorInfo);

		public override IEnumerable<CodeStatement> GetStubStatements()
		{
			/* 
			 * Required output:
			 * 1. Create a target variable
			 * 2. Call its getter to prevent stripping
			 * 3. Call its setter to prevent stripping
			 * 4. Create its optimized accessor to explicitly compile generic type
			 * 5. Call its optimized getter to explicitly compile generic method
			 * 6. Call its optimized setter to explicitly compile generic method
			*/

			var targetType = Code.TypeRef(manipulator.targetType, true);
			var accessorType = Code.TypeRef(manipulator.type, true);

			CodeExpression property;

			if (manipulator.requiresTarget)
			{
				// 1. Material target = default(Material);
				yield return Code.VarDecl(targetType, "target", targetType.DefaultValue());

				property = Code.VarRef("target");
			}
			else
			{
				property = targetType.Expression();
			}

			// target.color
			var propertyReference = property.Field(manipulator.name);

			if (manipulator.isPubliclyGettable)
			{
				// 2. Color accessor = target.color;
				yield return Code.VarDecl(accessorType, "accessor", propertyReference);
			}

			if (manipulator.isPubliclySettable)
			{
				// 3. target.color = default(Color);
				yield return propertyReference.Assign(accessorType.DefaultValue()).Statement();
			}

			if (supportsOptimization)
			{
				var optimizedAccessorType = Code.TypeRef(GetOptimizedAccessor(stub).GetType(), true);

				// 4. var accessor = new PropertyAccessor<Material, Color>(default(PropertyInfo));
				yield return Code.VarDecl(optimizedAccessorType, "optimized", optimizedAccessorType.ObjectCreate(Code.TypeRef(typeof(TAccessor)).DefaultValue()));

				CodeExpression target;

				if (manipulator.requiresTarget)
				{
					// default(Material)
					target = targetType.DefaultValue();
				}
				else
				{
					// null for static types
					target = Code.Primitive(null);
				}

				if (manipulator.isGettable)
				{
					// 5. accessor.GetValue(default(Material));
					yield return Code.VarRef("optimized").Method(nameof(IOptimizedAccessor.GetValue)).Invoke(target).Statement();
				}

				if (manipulator.isSettable)
				{
					// 6. accessor.SetValue(default(Material), default(Color));
					yield return Code.VarRef("optimized").Method(nameof(IOptimizedAccessor.SetValue)).Invoke(target, accessorType.DefaultValue()).Statement();
				}
			}
		}
	}
}