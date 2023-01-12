using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public sealed class LambdaAccessor : Accessor
	{
		public LambdaAccessor(object content, Type definedType, Accessor parent) : base(content, parent)
		{
			this.content = content;
			this.definedType = definedType;
		}

		public LambdaAccessor(object subpath, object content, Type definedType, Accessor parent) : base(subpath, parent)
		{
			this.content = content;
			this.definedType = definedType;
		}

		public object content { get; private set; }

		protected override object rawValue
		{
			get => content;
			set
			{
				if (subpath == null)
				{
					throw new NotSupportedException("Cannot change the value of a static object.");
				}

				content = value;
			}
		}

		protected override void OnValueTypeChange(Type previousType)
		{
			if (content is UnityObject)
			{
				label = new GUIContent(ObjectNames.NicifyVariableName(((UnityObject)content).name));
			}
			else
			{
				label = new GUIContent(valueType.HumanName());
			}

			label.tooltip = valueType.Summary();

			base.OnValueTypeChange(previousType);
		}

		protected override string Subpath()
		{
			return subpath?.ToString() ?? valueType.Name;
		}

		public override Attribute[] GetCustomAttributes(bool inherit = true)
		{
			return Empty<Attribute>.array;
		}
	}
}