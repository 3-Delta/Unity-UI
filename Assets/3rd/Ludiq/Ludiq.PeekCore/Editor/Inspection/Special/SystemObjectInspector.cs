using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class SystemObjectInspector : Inspector
	{
		public SystemObjectInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			base.Initialize();
			
			typeFilter = accessor.GetAttribute<TypeFilter>() ?? TypeFilter.Any;

			typeMemberName = accessor.GetAttribute<InspectorObjectTypeAttribute>()?.memberName;
		}

		private Inspector castedInspector => ChildInspector(accessor.Cast(type));

		private TypeFilter _typeFilter;
		
		private Type type => chooseType ? typeTree.type : (Type)typeMemberAccessor.value;
		
		private TypeTree typeTree = new TypeTree(null);

		public bool chooseType => typeMemberName == null;

		public bool showValue => type != null && InspectorProvider.instance.GetDecoratorType(type) != typeof(SystemObjectInspector);
		
		private string typeMemberName;

		private Accessor typeMemberAccessor => chooseType ? null : accessor.parent[typeMemberName];

		public TypeFilter typeFilter
		{
			get
			{
				return _typeFilter;
			}
			private set
			{
				value = value.Clone().Configured();
				value.Abstract = false;
				value.Interfaces = false;
				value.Object = false;
				_typeFilter = value;
			}
		}

		private Func<IFuzzyOptionTree> GetTypeOptions(TypeTree tree)
		{
			return () => new TypeOptionTree(Codebase.types, tree == typeTree ? typeFilter : typeTree.filter);
		}

		private void InferType()
		{
			var newType = accessor.value?.GetType();

			if (newType == typeTree.type || newType == null)
			{
				return;
			}
			
			typeTree.ChangeType(newType);
			EnforceType();
			SetHeightDirty();
		}

		private void EnforceType()
		{
			if (accessor.value?.GetType() == type)
			{
				return;
			}

			accessor.UnlinkChildren();

			if (type == null)
			{
				accessor.value = null;
			}
			else if (ConversionUtility.CanConvert(accessor.value, type, true))
			{
				accessor.value = ConversionUtility.Convert(accessor.value, type);
			}
			else
			{
				accessor.value = type.TryInstantiate();
			}
		}

		protected override void OnControlGUI(Rect position)
		{
			InferType();
			
			var showLabels = !adaptiveWidth && position.width >= 120;

			if (chooseType)
			{
				var x = position.x;
				var remainingWidth = position.width;

				if (showLabels)
				{
					var typeLabel = label == GUIContent.none ? new GUIContent("Type") : new GUIContent(label.text + " Type");

					var typeLabelPosition = new Rect
					(
						x,
						y,
						Styles.labelWidth,
						EditorGUIUtility.singleLineHeight
					);

					GUI.Label(typeLabelPosition, typeLabel, labelStyle);

					x += typeLabelPosition.width;
					remainingWidth -= typeLabelPosition.width;
				}

				var typePosition = new Rect
				(
					x,
					y,
					remainingWidth,
					LudiqGUI.GetTypeTreeFieldHeight(typeTree)
				);

				EditorGUI.BeginChangeCheck();

				LudiqGUI.TypeTreeField(ref typePosition, GUIContent.none, typeTree, true, GetTypeOptions, Contents.nullTypeLabel);

				if (EditorGUI.EndChangeCheck())
				{
					var newType = typeTree.GetSubstitutedType();
					accessor.RecordUndo();
					typeTree.ChangeType(newType);
					EnforceType();
					SetHeightDirty();
				}

				y += typePosition.height;
			}

			if (chooseType && showValue)
			{
				y += Styles.spaceBetweenTypeAndValue;
			}

			if (showValue)
			{
				Rect valuePosition;

				if (chooseType)
				{
					var x = position.x;
					var remainingWidth = position.width;

					if (showLabels)
					{
						var valueLabel = label == GUIContent.none ? new GUIContent("Value") : new GUIContent(label.text + " Value");

						var valueLabelPosition = new Rect
						(
							x,
							y,
							Styles.labelWidth,
							EditorGUIUtility.singleLineHeight
						);

						GUI.Label(valueLabelPosition, valueLabel, labelStyle);

						x += valueLabelPosition.width;
						remainingWidth -= valueLabelPosition.width;
					}

					valuePosition = new Rect
					(
						x,
						y,
						remainingWidth,
						castedInspector.ControlHeight(remainingWidth)
					);

					castedInspector.DrawControl(valuePosition);
				}
				else
				{
					valuePosition = new Rect
					(
						position.x,
						y,
						position.width,
						castedInspector.ControlHeight(position.width)
					);

					castedInspector.DrawControl(valuePosition);
				}

				y += valuePosition.height;
			}
			else
			{
				accessor.value = null;
			}
		}

		protected override float GetControlHeight(float width)
		{
			InferType();

			var height = 0f;

			if (chooseType)
			{
				height += LudiqGUI.GetTypeTreeFieldHeight(typeTree);
			}

			if (chooseType && showValue)
			{
				height += Styles.spaceBetweenTypeAndValue;
			}

			if (showValue)
			{
				height += castedInspector.ControlHeight(width);
			}

			return height;
		}

		protected override float GetControlWidth()
		{
			var width = 0f;

			if (chooseType)
			{
				width = Mathf.Max(width, LudiqGUI.GetTypeFieldAdaptiveWidth(type));
			}

			if (showValue)
			{
				width = Mathf.Max(width, castedInspector.ControlWidth());
			}

			width += Styles.labelWidth;

			return width;
		}

		public static class Styles
		{
			public static readonly float spaceBetweenTypeAndValue = 2;
			public static readonly float labelWidth = 38;
		}

		public static class Contents
		{
			public static readonly GUIContent nullTypeLabel = new GUIContent("(Null)");
		}
	}
}