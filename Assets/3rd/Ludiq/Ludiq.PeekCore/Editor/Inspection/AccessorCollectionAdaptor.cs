using System;
using Ludiq.PeekCore.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class AccessorCollectionAdaptor : IReorderableListAdaptor
	{
		protected AccessorCollectionAdaptor(Accessor accessor, Inspector parentInspector)
		{
			Ensure.That(nameof(accessor)).IsNotNull(accessor);
			Ensure.That(nameof(parentInspector)).IsNotNull(parentInspector);

			this.accessor = accessor;
			this.parentInspector = parentInspector;

			var wideAttribute = accessor.GetAttribute<InspectorWideAttribute>();

			if (wideAttribute != null)
			{
				widthMode = WidthMode.Wide;
			}
			else
			{
				widthMode = WidthMode.Thin;
			}

			var listControlFlags = default(ReorderableListFlags);

			if (!showAddButton)
			{
				listControlFlags |= ReorderableListFlags.HideAddButton;
			}

			listControl = new ReorderableListControl(listControlFlags);
			listControl.ContainerStyle = new GUIStyle(ReorderableListStyles.Container);
			listControl.FooterButtonStyle = new GUIStyle(ReorderableListStyles.FooterButton);
			listControl.ItemButtonStyle = new GUIStyle(ReorderableListStyles.ItemButton);

			listControl.ItemRemoving += OnItemRemoving;

			if (showAddMenu)
			{
				listControl.AddMenuClicked += OnAddMenuClicked;
			}
		}

		private float itemWidth;

		private WidthMode widthMode;

		private Accessor accessor;

		private ReorderableListControl listControl;

		protected Inspector parentInspector { get; private set; }
		
		private enum WidthMode
		{
			Thin,

			Wide,
		}



		#region Drawing

		public virtual void BeginGUI() { }

		public virtual void EndGUI() { }

		public virtual void DrawItemBackground(Rect position, int index) { }

		protected virtual float GetTitleHeight(float width, GUIContent title)
		{
			if (title == GUIContent.none)
			{
				return 0;
			}

			return 20;
		}

		protected virtual void OnTitleGUI(Rect position, GUIContent title)
		{
			if (title == GUIContent.none)
			{
				return;
			}

			ReorderableListGUI.Title(position, title);
		}

		public float GetItemHeight(int index)
		{
			return GetItemHeight(itemWidth, index);
		}

		public abstract float GetItemHeight(float width, int index);

		public abstract void DrawItem(Rect position, int index);

		public abstract float GetItemWidth(int index);

		private const float HandlesWidth = 56;
		private const float DefaultBottomPadding = 10;

		public float GetControlWidth()
		{
			var width = 0f;

			for (int i = 0; i < Count; i++)
			{
				width = Mathf.Max(width, GetItemWidth(i));
			}

			width += HandlesWidth;

			return width;
		}

		public float GetControlHeight(float width)
		{
			itemWidth = width - HandlesWidth;

			var height = 0f;

			height += listControl.CalculateListHeight(this);
			height -= DefaultBottomPadding;

			return height;
		}

		public void DrawControl(Rect position)
		{
			var y = position.y;
			
			itemWidth = position.width - HandlesWidth;

			var height = listControl.CalculateListHeight(this);

			var listPosition = new Rect
			(
				position.x,
				y,
				position.width,
				height
			);

			listControl.Draw(listPosition, this);
		}

		public float GetFieldHeight(float width)
		{
			if (widthMode == WidthMode.Thin)
			{
				width = parentInspector.WidthWithoutLabel(width);
			}

			itemWidth = width - HandlesWidth;

			var height = 0f;

			var label = Inspector.GetLabel(accessor);

			if (widthMode != WidthMode.Thin && label != GUIContent.none)
			{
				height += GetTitleHeight(width, label);
			}

			height += listControl.CalculateListHeight(this);
			height -= DefaultBottomPadding;

			if (widthMode == WidthMode.Thin)
			{
				height = parentInspector.HeightWithLabel(width, height);
			}

			return height;
		}

		public void DrawField(Rect parentPosition, ref float y)
		{
			DrawField(parentPosition.VerticalSection(ref y, GetFieldHeight(parentPosition.width)));
		}

		public void DrawField(Rect position)
		{
			var y = position.y;

			if (widthMode == WidthMode.Thin)
			{
				position = parentInspector.PrefixLabel(position);
			}

			position.height += DefaultBottomPadding;
			itemWidth = position.width - HandlesWidth;

			var height = position.height;
			
			var label = Inspector.GetLabel(accessor);

			if (widthMode != WidthMode.Thin && label != GUIContent.none)
			{
				var titlePosition = new Rect
				(
					position.x,
					y,
					position.width,
					GetTitleHeight(position.width, label)
				);

				OnTitleGUI(titlePosition, label);

				y += titlePosition.height - 1;
				height -= titlePosition.height;
			}
			else
			{
				height = listControl.CalculateListHeight(this);
			}

			var listPosition = new Rect
			(
				position.x,
				y,
				position.width,
				height
			);

			listControl.Draw(listPosition, this);
		}

		#endregion



		#region Manipulation

		public virtual bool showAddButton => true;

		public virtual bool showAddMenu => false;

		public abstract int Count { get; }

		public abstract bool CanDrag(int index);

		public abstract bool CanRemove(int index);

		public abstract void Add();

		public abstract void Insert(int index);

		public abstract void Duplicate(int index);

		public abstract void Remove(int index);

		public abstract void Move(int sourceIndex, int destIndex);

		public abstract void Clear();

		protected virtual void OnAddMenuClicked(object sender, AddMenuClickedEventArgs args)
		{

		}

		protected virtual void OnItemRemoving(object sender, ItemRemovingEventArgs args)
		{

		}

		#endregion
	}
}
