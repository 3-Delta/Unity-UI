using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class AccessorDictionaryAdaptor : AccessorCollectionAdaptor
	{
		public AccessorDictionaryAdaptor(Accessor accessor, Inspector parentInspector) : base(accessor, parentInspector)
		{
			this.accessor = accessor;

			accessor.valueChanged += (previousValue) =>
			{
				if (!accessor.isDictionary)
				{
					throw new InvalidOperationException("Accessor for dictionary adaptor is not a dictionary: " + accessor);
				}

				if (accessor.value == null)
				{
					accessor.value = ConstructDictionary();
				}

				newKeyAccessor?.Unlink();

				newValueAccessor?.Unlink();

				newKeyAccessor = accessor.Lambda("newKey", ConstructKey(), accessor.dictionaryKeyType);
				newValueAccessor = accessor.Lambda("newValue", ConstructValue(), accessor.dictionaryValueType);
			};
		}

		public event Action<object, object> itemAdded;

		private Accessor newKeyAccessor;
		private Accessor newValueAccessor;

		protected Accessor accessor { get; private set; }

		protected virtual IDictionary ConstructDictionary()
		{
			try
			{
				return (IDictionary)Activator.CreateInstance(accessor.dictionaryType);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Could not create dictionary instance of type '{accessor.dictionaryType}'.", ex);
			}
		}

		protected virtual object ConstructKey()
		{
			if (accessor.dictionaryKeyType.IsValueType)
			{
				return Activator.CreateInstance(accessor.dictionaryKeyType);
			}
			else
			{
				return null;
			}
		}

		protected virtual object ConstructValue()
		{
			if (accessor.dictionaryValueType.IsValueType)
			{
				return Activator.CreateInstance(accessor.dictionaryValueType);
			}
			else
			{
				return null;
			}
		}

		private const float spaceBetweenKeyAndValue = 5;
		private const float itemPadding = 2;

		#region Manipulation

		public object this[object key]
		{
			get
			{
				return ((IDictionary)accessor)[key];
			}
			set
			{
				accessor.RecordUndo();
				((IDictionary)accessor)[key] = value;
			}
		}

		public override int Count => accessor.Count + 1;

		public override void Add()
		{
			if (!CanAdd())
			{
				return;
			}

			var newKey = newKeyAccessor.value;
			var newValue = newValueAccessor.value;

			accessor.RecordUndo();
			accessor.Add(newKey, newValue);

			newKeyAccessor.value = ConstructKey();
			newValueAccessor.value = ConstructValue();

			itemAdded?.Invoke(newKey, newValue);

			parentInspector.SetHeightDirty();
		}

		public override void Clear()
		{
			accessor.RecordUndo();
			accessor.Clear();

			parentInspector.SetHeightDirty();
		}

		public override void Insert(int index)
		{
			return;
		}

		public override void Remove(int index)
		{
			accessor.RecordUndo();
			accessor.Remove(accessor.KeyAccessor(index).value);
			parentInspector.SetHeightDirty();
		}

		public override void Move(int sourceIndex, int destinationIndex)
		{
			return;
		}

		public override void Duplicate(int index)
		{
			return;
		}

		protected bool CanAdd()
		{
			var newKey = newKeyAccessor.value;

			if (newKey == null)
			{
				EditorUtility.DisplayDialog("New Dictionary Item", "A dictionary key cannot be null.", "OK");
				return false;
			}

			if (accessor.Contains(newKeyAccessor.value))
			{
				EditorUtility.DisplayDialog("New Dictionary Item", "An item with the same key already exists.", "OK");
				return false;
			}

			return true;
		}

		public override bool CanDrag(int index)
		{
			return accessor.isOrderedDictionary && (index != Count - 1);
		}

		public override bool CanRemove(int index)
		{
			return index != Count - 1;
		}

		#endregion

		#region Drawing

		public override float GetItemWidth(int index)
		{
			// TODO
			return 100;
		}

		public override float GetItemHeight(float width, int index)
		{
			if (index == Count - 1)
			{
				return GetNewItemHeight(width);
			}
			else
			{
				return GetItemHeight(accessor.KeyAccessor(index), accessor.ValueAccessor(index), width);
			}
		}

		public override void DrawItem(Rect position, int index)
		{
			if (index == Count - 1)
			{
				DrawNewItem(position);
			}
			else
			{
				OnItemGUI(accessor.KeyAccessor(index), accessor.ValueAccessor(index), position, false);
			}
		}

		private float GetNewItemHeight(float width)
		{
			var height = 0f;
			height += EditorGUIUtility.singleLineHeight;
			height += GetItemHeight(newKeyAccessor, newValueAccessor, width);
			return height;
		}

		private void DrawNewItem(Rect position)
		{
			var newLabelPosition = new Rect
			(
				position.x,
				position.y,
				position.width,
				EditorGUIUtility.singleLineHeight
			);

			var newItemPosition = new Rect
			(
				position.x,
				newLabelPosition.yMax,
				position.width,
				GetItemHeight(newKeyAccessor, newValueAccessor, position.width)
			);

			GUI.Label(newLabelPosition, "New Item: ");
			OnItemGUI(newKeyAccessor, newValueAccessor, newItemPosition, true);
		}

		private float GetKeyHeight(Accessor keyAccessor, float keyWidth)
		{
			return parentInspector.ChildInspector(keyAccessor).ControlHeight(keyWidth);
		}

		private float GetValueHeight(Accessor valueAccessor, float valueWidth)
		{
			return parentInspector.ChildInspector(valueAccessor).ControlHeight(valueWidth);
		}

		private float GetKeyWidth(float width)
		{
			return (width - spaceBetweenKeyAndValue) / 2;
		}

		private float GetValueWidth(float width)
		{
			return (width - spaceBetweenKeyAndValue) / 2;
		}

		private float GetItemHeight(Accessor keyAccessor, Accessor valueAccessor, float width)
		{
			return Mathf.Max(GetKeyHeight(keyAccessor, GetKeyWidth(width)), GetValueHeight(valueAccessor, GetValueWidth(width))) + (itemPadding * 2);
		}

		private void OnKeyGUI(Accessor keyAccessor, Rect keyPosition)
		{
			parentInspector.ChildInspector(keyAccessor).DrawControl(keyPosition);
		}

		private void OnValueGUI(Accessor valueAccessor, Rect valuePosition)
		{
			parentInspector.ChildInspector(valueAccessor).DrawControl(valuePosition);
		}

		private void OnItemGUI(Accessor keyAccessor, Accessor valueAccessor, Rect position, bool editableKey)
		{
			var keyPosition = new Rect
			(
				position.x + itemPadding,
				position.y + itemPadding,
				GetKeyWidth(position.width),
				GetKeyHeight(keyAccessor, GetKeyWidth(position.width))
			);

			var valuePosition = new Rect
			(
				keyPosition.xMax + spaceBetweenKeyAndValue,
				position.y + itemPadding,
				GetValueWidth(position.width),
				GetValueHeight(valueAccessor, GetValueWidth(position.width))
			);

			EditorGUI.BeginDisabledGroup(!editableKey);
			OnKeyGUI(keyAccessor, keyPosition);
			EditorGUI.EndDisabledGroup();

			OnValueGUI(valueAccessor, valuePosition);
		}

		#endregion
	}
}