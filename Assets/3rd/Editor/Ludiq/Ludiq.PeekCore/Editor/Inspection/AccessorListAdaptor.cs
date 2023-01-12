using System;
using System.Collections;
using Ludiq.PeekCore.ReorderableList;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public class AccessorListAdaptor : AccessorCollectionAdaptor, IReorderableListDropTarget
	{
		public AccessorListAdaptor(Accessor accessor, Inspector parentInspector) : base((Accessor)accessor, parentInspector)
		{
			if (accessor == null)
			{
				throw new ArgumentNullException(nameof(accessor));
			}

			this.accessor = accessor;

			reorderable = accessor.GetAttribute<InspectorReorderableAttribute>()?.reorderable ?? true;

			accessor.valueChanged += (previousValue) =>
			{
				if (!accessor.isList)
				{
					throw new InvalidOperationException("Accessor for list adaptor is not a list: " + accessor);
				}

				if (accessor.value == null)
				{
					accessor.value = ConstructList();
				}
			};
		}

		private bool reorderable = true;

		public event Action<object> itemAdded;

		public Accessor accessor { get; private set; }

		protected virtual IList ConstructList()
		{
			if (accessor.listType.IsArray)
			{
				return Array.CreateInstance(accessor.listElementType, 0);
			}
			else
			{
				try
				{
					return (IList)accessor.listType.Instantiate(false);
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Could not create list instance of type '{accessor.listType}'.", ex);
				}
			}
		}

		protected virtual object ConstructItem()
		{
			return accessor.listElementType.TryInstantiate(false);
		}

		#region Manipulation

		public object this[int index]
		{
			get
			{
				return ((IList)accessor)[index];
			}
			set
			{
				accessor.RecordUndo();
				((IList)accessor)[index] = value;
			}
		}

		public override int Count => accessor.Count;

		public override void Add()
		{
			if (!CanAdd())
			{
				return;
			}

			var newItem = ConstructItem();

			accessor.RecordUndo();
			accessor.Add(newItem);

			itemAdded?.Invoke(newItem);

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
			if (!CanAdd())
			{
				return;
			}

			var newItem = ConstructItem();

			accessor.RecordUndo();
			accessor.Insert(index, newItem);

			itemAdded?.Invoke(newItem);

			parentInspector.SetHeightDirty();
		}

		public override void Remove(int index)
		{
			accessor.RecordUndo();
			accessor.RemoveAt(index);

			parentInspector.SetHeightDirty();
		}

		public override void Move(int sourceIndex, int destinationIndex)
		{
			accessor.RecordUndo();
			accessor.Move(sourceIndex, destinationIndex);
		}

		public override void Duplicate(int index)
		{
			accessor.RecordUndo();
			accessor.Duplicate(index);

			itemAdded?.Invoke(this[index + 1]);

			parentInspector.SetHeightDirty();
		}

		protected virtual bool CanAdd()
		{
			return true;
		}

		public override bool CanDrag(int index)
		{
			return reorderable;
		}

		public override bool CanRemove(int index)
		{
			return true;
		}

		#endregion

		#region Drag & Drop

		private const float MouseDragThreshold = 0.6f;
		private static AccessorListAdaptor selectedList;
		private static object selectedItem;
		private static Vector2 mouseDragStartPosition;

		public bool CanDropInsert(int insertionIndex)
		{
			if (!ReorderableListControl.CurrentListPosition.Contains(Event.current.mousePosition))
			{
				return false;
			}

			var data = DragAndDrop.GetGenericData(DraggedListItem.TypeName);

			return data is DraggedListItem && accessor.listElementType.IsInstanceOfType(((DraggedListItem)data).item);
		}

		protected virtual bool CanDrop(object item)
		{
			return true;
		}

		public void ProcessDropInsertion(int insertionIndex)
		{
			if (Event.current.type == EventType.DragPerform)
			{
				var draggedItem = (DraggedListItem)DragAndDrop.GetGenericData(DraggedListItem.TypeName);

				if (draggedItem.sourceListAdaptor == this)
				{
					Move(draggedItem.index, insertionIndex);
				}
				else
				{
					if (CanDrop(draggedItem.item))
					{
						accessor.Insert(insertionIndex, draggedItem.item);

						itemAdded?.Invoke(draggedItem.item);

						draggedItem.sourceListAdaptor.Remove(draggedItem.index);
						selectedList = this;

						draggedItem.sourceListAdaptor.parentInspector.SetHeightDirty();
						parentInspector.SetHeightDirty();
					}
				}

				GUI.changed = true;
				Event.current.Use();
			}
		}

		#endregion

		#region Drawing

		protected virtual Inspector GetItemInspector(int index)
		{
			return parentInspector.ChildInspector(accessor[index]);
		}

		public override float GetItemWidth(int index)
		{
			return GetItemInspector(index).ControlWidth();
		}

		public override float GetItemHeight(float width, int index)
		{
			return GetItemInspector(index).ControlHeight(width);
		}

		public bool alwaysDragAndDrop { get; set; } = false;
		
		public override void DrawItem(Rect position, int index)
		{
			GetItemInspector(index).DrawControl(position);

			var item = this[index];

			var controlID = GUIUtility.GetControlID(FocusType.Passive);

			switch (Event.current.GetTypeForControl(controlID))
			{
				case EventType.MouseDown:
					// Exclude delete button from draggable position
					var draggablePosition = ReorderableListGUI.CurrentItemTotalPosition;
					draggablePosition.xMax = position.xMax + 2;

					if (Event.current.button == (int)MouseButton.Left && draggablePosition.Contains(Event.current.mousePosition))
					{
						selectedList = this;
						selectedItem = item;

						if (alwaysDragAndDrop || Event.current.alt)
						{
							GUIUtility.hotControl = controlID;
							mouseDragStartPosition = Event.current.mousePosition;
							Event.current.Use();
						}
					}

					break;

				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						GUIUtility.hotControl = 0;

						if (Vector2.Distance(mouseDragStartPosition, Event.current.mousePosition) >= MouseDragThreshold)
						{
							DragAndDrop.PrepareStartDrag();
							DragAndDrop.objectReferences = new UnityObject[0];
							DragAndDrop.paths = new string[0];
							DragAndDrop.SetGenericData(DraggedListItem.TypeName, new DraggedListItem(this, index, item));
							DragAndDrop.StartDrag(accessor.path);
						}

						Event.current.Use();
					}

					break;
			}
		}

		public override void DrawItemBackground(Rect position, int index)
		{
			base.DrawItemBackground(position, index);

			if (this == selectedList && this[index] == selectedItem)
			{
				//GUI.DrawTexture(new RectOffset(1, 1, 1, 1).Add(position), ReorderableListStyles.SelectionBackgroundColor.GetPixel());
			}
		}

		#endregion
	}
}