using System;
using System.Reflection;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class TypeTree
	{
		private bool initialized;

		public TypeTree() { }

		public TypeTree(Type type)
		{
			ChangeType(type);
		}

		public TypeTree(Type substitutedType, Type genericParameter)
		{
			ChangeType(substitutedType, genericParameter);
		}

		public Type type { get; private set; }

		public string humanLabel { get; private set; }

		public string programmerLabel { get; private set; }

		public string displayLabel => LudiqCore.Configuration.humanNaming ? humanLabel : programmerLabel;

		public Type genericParameter { get; private set; }

		public Type genericTypeDefinition { get; private set; }

		public TypeTree[] children { get; private set; }

		public Type[] genericConstraints { get; private set; }

		public bool hasReferenceTypeConstraint => (genericParameter?.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0;

		public bool hasNonNullableValueTypeConstraint => (genericParameter?.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;

		public bool hasDefaultConstructorConstraint => (genericParameter?.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) != 0;

		public TypeFilter filter { get; private set; }

		public int recursiveNodeCount
		{
			get
			{
				var result = 1;

				if (children != null)
				{
					foreach (var child in children)
					{
						result += child.recursiveNodeCount;
					}
				}

				return result;
			}
		}

		public int recursiveDepth
		{
			get
			{
				var result = 1;

				if (children != null)
				{
					foreach (var child in children)
					{
						result = Math.Max(result, child.recursiveDepth + 1);
					}
				}

				return result;
			}
		}

		public bool IsClosedForm()
		{
			if (genericTypeDefinition != null)
			{
				foreach (var child in children)
				{
					if (!child.IsClosedForm())
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				return genericParameter == null;
			}
		}

		public void ChangeType(Type type)
		{
			ChangeType(type, type != null && type.IsGenericParameter ? type : genericParameter);
		}

		public void ChangeType(Type type, Type genericParameter)
		{
			if (this.type != type || !initialized)
			{
				this.type = type;

				genericTypeDefinition = null;
				children = null;

				if (type != null)
				{
					if (type.IsGenericType)
					{
						genericTypeDefinition = type.GetGenericTypeDefinition();

						var genericParameters = genericTypeDefinition.GetGenericArguments();
						var typeArguments = type.GetGenericArguments();

						children = new TypeTree[typeArguments.Length];

						for (var i = 0; i < typeArguments.Length; i++)
						{
							children[i] = new TypeTree(typeArguments[i], genericParameters[i]);
						}
					}
				}
			}

			if (this.genericParameter != genericParameter || !initialized)
			{
				this.genericParameter = genericParameter;

				if (genericParameter != null)
				{
					humanLabel = genericParameter.HumanName();
					programmerLabel = genericParameter.Name;
					genericConstraints = genericParameter.GetGenericParameterConstraints();

					filter = new TypeFilter(genericConstraints)
					{
						GenericParameterAttributeFlags = genericParameter.GenericParameterAttributes
					};
				}
				else
				{
					humanLabel = null;
					programmerLabel = null;
					genericConstraints = null;
					filter = TypeFilter.Any;
				}
			}

			initialized = true;
		}

		public Type GetSubstitutedType()
		{
			if (type == null)
			{
				throw new InvalidOperationException("Cannot substitute null type");
			}

			if (genericTypeDefinition != null)
			{
				var genericTypeArguments = new Type[children.Length];

				for (var i = 0; i < children.Length; i++)
				{
					genericTypeArguments[i] = children[i].GetSubstitutedType();
				}

				return genericTypeDefinition.MakeGenericType(genericTypeArguments);
			}
			else
			{
				return type;
			}
		}
	}
}