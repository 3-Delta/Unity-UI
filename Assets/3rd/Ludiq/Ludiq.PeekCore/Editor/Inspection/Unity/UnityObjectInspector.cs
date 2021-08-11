using Ludiq.PeekCore.Bolt;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public class UnityObjectInspector : Inspector
	{
		public UnityObjectInspector(Accessor accessor) : base(accessor) { }

		protected UnityObject value => (UnityObject)accessor.value;

		protected virtual bool fuzzy => e.alt != LudiqCore.Configuration.fuzzyObjectPicker;

		protected virtual string typeLabel => accessor.definedType.Name;

		protected virtual EditorTexture typeIcon => accessor.definedType.Icon();

		protected virtual Scene? scene => accessor.serializedObject?.AsGameObject()?.scene;

		protected virtual bool allowAssetObjects => true;

		protected virtual bool hidableFrame => true;

		protected virtual IFuzzyOptionTree GetOptions()
		{
			return new UnityObjectOptionTree
			(
				accessor.definedType,
				typeLabel,
				typeIcon,
				scene,
				allowAssetObjects,
				Filter,
				typeLabel
			);
		}
		
		protected virtual UnityObjectFieldVisualType visualType
		{
			get
			{
				if (adaptiveWidth && accessor.value.IsUnityNull())
				{
					return UnityObjectFieldVisualType.Target;
				}
				else
				{
					return UnityObjectFieldVisualType.NameAndTarget;
				}
			}
		}

		protected override float GetControlWidth()
		{
			if (accessor.value.IsUnityNull())
			{
				return LudiqGUI.GetFuzzyObjectFieldWidth(value, UnityObjectFieldVisualType.Target);
			}
			else
			{
				return LudiqGUI.GetFuzzyObjectFieldWidth(value, UnityObjectFieldVisualType.NameAndTarget);
			}
		}

		private float GetObjectFieldWidth()
		{
			return LudiqGUI.GetFuzzyObjectFieldWidth(value, visualType);
		}

		private float GetObjectFieldHeight(float width)
		{
			return LudiqGUI.GetFuzzyObjectFieldHeight(visualType, width);
		}

		protected override float GetControlHeight(float width)
		{
			var height = EditorGUIUtility.singleLineHeight;

			if (!Filter(value))
			{
				height += EditorGUIUtility.standardVerticalSpacing;
				height += LudiqGUIUtility.GetHelpBoxHeight(InvalidValueMessage(value), MessageType.Error, width);
			}

			return height;
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var oldValue = (UnityObject)accessor.value;
			
			var fieldPosition = new Rect
			(
				position.x,
				position.y,
				position.width,
				GetObjectFieldHeight(position.width)
			);

			UnityObject newValue;

			if (fuzzy)
			{
				newValue = LudiqGUI.ObjectField
				(
					fieldPosition,
					oldValue,
					accessor.definedType,
					scene,
					allowAssetObjects,
					typeLabel,
					typeIcon,
					Filter,
					GetOptions,
					visualType,
					hidableFrame
				);
			}
			else
			{
				newValue = EditorGUI.ObjectField
				(
					fieldPosition,
					oldValue,
					accessor.definedType,
					scene != null
				);
			}

			y += EditorGUIUtility.singleLineHeight;

			var isValid = Filter(newValue);

			if (!isValid)
			{
				y += EditorGUIUtility.standardVerticalSpacing;

				var message = InvalidValueMessage(newValue);

				var invalidValueMessagePosition = position.VerticalSection(ref y, LudiqGUIUtility.GetHelpBoxHeight(message, MessageType.Error, position.width));

				EditorGUI.HelpBox(invalidValueMessagePosition, message, MessageType.Error);

				if (newValue != null && GUI.Button(invalidValueMessagePosition, GUIContent.none, GUIStyle.none))
				{
					EditorGUIUtility.PingObject(newValue);
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				if (isValid)
				{
					accessor.RecordUndo();
					accessor.value = newValue;
				}
				else
				{
					Debug.LogWarning(InvalidValueMessage(newValue));
				}
			}
		}

		protected virtual string InvalidValueMessage(UnityObject invalidValue)
		{
			if (invalidValue == null)
			{
				return "Value should not be null.";
			}

			if (!accessor.definedType.IsInstanceOfTypeNullable(invalidValue))
			{
				return $"Invalid value type: expected {accessor.definedType.DisplayName()}, not {invalidValue.GetType().DisplayName()}.";
			}

			return CustomInvalidValueMessage(invalidValue);
		}

		protected virtual string CustomInvalidValueMessage(UnityObject invalidValue)
		{
			return "Invalid value.";
		}

		protected virtual bool Filter(UnityObject uo)
		{
			if (uo == null)
			{
				return true;
			}

			if (!accessor.definedType.IsInstanceOfTypeNullable(uo))
			{
				return false;
			}

			return true;
		}
	}
}