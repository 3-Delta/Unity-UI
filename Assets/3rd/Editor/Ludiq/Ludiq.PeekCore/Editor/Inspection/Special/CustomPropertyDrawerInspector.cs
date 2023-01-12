using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class CustomPropertyDrawerInspector : Inspector
	{
		public CustomPropertyDrawerInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			base.Initialize();

			property = SerializedPropertyUtility.CreateTemporaryProperty(accessor.definedType);
			propertyType = property.GetUnderlyingType();
			
			var adaptiveWidthAttribute = propertyType.GetAttribute<InspectorFieldWidthAttribute>();
			_adaptiveWidth = adaptiveWidthAttribute?.width ?? 200;
		}

		private float _adaptiveWidth;

		private SerializedProperty property;

		private Type propertyType;

		public override void Dispose()
		{
			SerializedPropertyUtility.DestroyTemporaryProperty(property);
			base.Dispose();
		}

		protected override bool cacheControlHeight => false;

		protected override float GetFieldHeight(float width)
		{
			return EditorGUI.GetPropertyHeight(property, label);
		}

		protected override float GetControlWidth()
		{
			return _adaptiveWidth;
		}

		protected override void OnFieldGUI(Rect position)
		{
			BeginBlock(position);

			if (!propertyType.IsAssignableFrom(accessor.valueType))
			{
				if (propertyType.IsValueType)
				{
					accessor.value = Activator.CreateInstance(propertyType);
				}
				else
				{
					accessor.value = null;
				}
			}

			property.SetUnderlyingValue(accessor.value);

			property.serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(position, property, label);

			property.serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = property.GetUnderlyingValue();
			}

			EndBlock();
		}
	}
}