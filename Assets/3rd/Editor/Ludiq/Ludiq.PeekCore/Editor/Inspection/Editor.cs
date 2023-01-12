using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class Editor : Inspector
	{
		protected Editor(Accessor accessor) : base(accessor) { }

		public EditorLayout layout { get; set; } = EditorLayout.Fields;

		public float InnerWidth() => ControlWidth();

		public float InnerHeight(float width) => GetInnerHeight(width);

		public void DrawInner(Rect position)
		{
			if (!ShieldDraw(position))
			{
				return;
			}

			y = position.y;

			try
			{
				EnsureLabelStyle();
				BeginBlock(position);
				OnInnerGUI(position);

				if (EndBlock())
				{
					OnChange();
					SetHeightDirty();
				}
			}
			catch (ExitGUIException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (safe)
				{
					onGuiException = ex;
				}
				else
				{
					throw;
				}
			}
		}

		protected abstract float GetInnerHeight(float width);

		protected abstract void OnInnerGUI(Rect position);

		protected virtual float GetFooterHeight(float width)
		{
			return 0;
		}

		protected virtual void OnFooterGUI(Rect position) { }

		private bool foldoutExpanded;

		protected virtual bool showMeta => false;

		protected virtual bool showTitle => true;

		protected virtual bool showSummary => true;

		protected virtual bool showIcon => true;

		protected virtual string staticTitle => null;

		protected virtual string staticSummary => null;

		protected virtual EditorTexture staticIcon => null;

		protected virtual string titlePlaceholder => null;

		protected virtual EditorTexture iconPlaceholder => null;

		protected virtual Accessor titleAccessor => null;

		protected virtual Accessor summaryAccessor => null;

		protected virtual Accessor iconAccessor => null;

		private Inspector titleInspector => ChildInspector(titleAccessor, ConfigureTitleInspector);

		private Inspector summaryInspector => ChildInspector(summaryAccessor);

		private Inspector iconInspector => ChildInspector(iconAccessor);

		protected float GetInnerWidth(float width)
		{
			return width - padding.left - padding.right;
		}

		protected float GetFooterWidth(float width)
		{
			return width;
		}

		protected sealed override float GetControlHeight(float width)
		{
			var height = 0f;

			float innerHeight;

			if (showMeta)
			{
				height += GetMetaHeight(width);

				if (layout == EditorLayout.Foldout && !foldoutExpanded)
				{
					return height;
				}

				var innerWidth = GetInnerWidth(width);
				innerHeight = GetInnerHeight(innerWidth);

				if (innerHeight > 0)
				{
					height += padding.top;
				}
			}
			else
			{
				innerHeight = GetInnerHeight(width);
			}

			height += innerHeight;

			var footerWidth = GetFooterWidth(width);
			var footerHeight = GetFooterHeight(footerWidth);

			if (innerHeight > 0 && footerHeight > 0)
			{
				height += padding.bottom;
			}

			height += footerHeight;

			return height;
		}

		private RectOffset padding
		{
			get
			{
				switch (layout)
				{
					case EditorLayout.Header:
					case EditorLayout.Asset:
						return Styles.headerPadding;

					case EditorLayout.Fields:
					case EditorLayout.Component:
						return Styles.noPadding;

					case EditorLayout.Foldout:
						return Styles.foldoutPadding;

					default: throw layout.Unexpected();
				}
			}
		}

		protected sealed override void OnControlGUI(Rect position)
		{
			if (layout == EditorLayout.Asset)
			{
				position = LudiqGUI.ExpandPosition(position);
			}

			float innerWidth, innerHeight;

			if (showMeta)
			{
				OnMetaGUI(position);

				if (layout == EditorLayout.Foldout && !foldoutExpanded)
				{
					return;
				}

				innerWidth = GetInnerWidth(position.width);
				innerHeight = GetInnerHeight(innerWidth);

				if (innerHeight > 0)
				{
					y += padding.top;
				}
			}
			else
			{
				innerWidth = position.width;
				innerHeight = GetInnerHeight(innerWidth);
			}

			var innerPosition = new Rect
			(
				position.x + ((position.width - innerWidth) / 2),
				y,
				innerWidth,
				innerHeight
			);
			
			OnInnerGUI(innerPosition);

			y = innerPosition.yMax;

			var footerWidth = GetFooterWidth(position.width);
			var footerHeight = GetFooterHeight(footerWidth);

			if (innerHeight > 0 && footerHeight > 0)
			{
				y += padding.bottom;
			}

			var footerPosition = new Rect
			(
				position.x + ((position.width - footerWidth) / 2),
				y,
				footerWidth,
				footerHeight
			);
			
			OnFooterGUI(footerPosition);
		}

		protected float GetMetaHeight(float width)
		{
			switch (layout)
			{
				case EditorLayout.Fields: return GetFieldsHeight(width);
				case EditorLayout.Foldout: return GetFoldoutHeight(width);
				case EditorLayout.Header: return GetHeaderHeight(width);

				case EditorLayout.Asset:
				case EditorLayout.Component:
					return 0; // We're using the area from the default Unity header

				default: throw layout.Unexpected();
			}
		}

		protected virtual void OnMetaGUI(Rect position)
		{
			if (layout == EditorLayout.Fields)
			{
				OnFieldsGUI(position);
			}
			else if (layout == EditorLayout.Header)
			{
				OnHeaderGUI(position);
			}
			else if (layout == EditorLayout.Asset)
			{
				LudiqGUI.EraseAssetHeader(position);
				var headerPosition = LudiqGUI.GetAssetHeaderPosition(position);
				y = headerPosition.y;
				OnHeaderGUI(headerPosition);
			}
			else if (layout == EditorLayout.Component)
			{
				OnComponentHeaderGUI(position);
			}
			else if (layout == EditorLayout.Foldout)
			{
				OnFoldoutGUI(position);
			}
		}

		protected virtual void ConfigureTitleInspector(Inspector inspector)
		{
			if (inspector is StringInspector stringInspector)
			{
				stringInspector.placeholder = titlePlaceholder;
			}
		}

		#region Fields

		protected float GetFieldsHeight(float width)
		{
			var height = 0f;

			if (showTitle)
			{
				height += GetSheetTitleHeight(width);
				height += EditorGUIUtility.standardVerticalSpacing;
			}

			if (showSummary)
			{
				height += GetFieldsSummaryHeight(width);
				height += EditorGUIUtility.standardVerticalSpacing;
			}

			if (showIcon)
			{
				height += GetFieldsIconHeight(width);
				height += EditorGUIUtility.standardVerticalSpacing;
			}

			return height;
		}

		protected void OnFieldsGUI(Rect position)
		{
			if (showTitle)
			{
				OnFieldsTitleGUI(position.VerticalSection(ref y, GetSheetTitleHeight(position.width)));
				y += EditorGUIUtility.standardVerticalSpacing;
			}

			if (showSummary)
			{
				OnFieldsSummaryGUI(position.VerticalSection(ref y, GetSheetTitleHeight(position.width)));
				y += EditorGUIUtility.standardVerticalSpacing;
			}

			if (showIcon)
			{
				OnFieldIconGUI(position.VerticalSection(ref y, GetSheetTitleHeight(position.width)));
				y += EditorGUIUtility.standardVerticalSpacing;
			}
		}

		protected virtual float GetSheetTitleHeight(float width)
		{
			if (titleAccessor != null)
			{
				return titleInspector.FieldHeight(width);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}

		protected virtual float GetFieldsSummaryHeight(float width)
		{
			if (summaryAccessor != null)
			{
				return summaryInspector.FieldHeight(width);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}

		protected virtual float GetFieldsIconHeight(float width)
		{
			if (iconAccessor != null)
			{
				return iconInspector.FieldHeight(width);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}

		protected virtual void OnFieldsTitleGUI(Rect position)
		{
			if (titleAccessor != null)
			{
				titleInspector.DrawField(position);
			}
			else
			{
				EditorGUI.LabelField(position, new GUIContent("Title"), new GUIContent(staticTitle));
			}
		}

		protected virtual void OnFieldsSummaryGUI(Rect position)
		{
			if (summaryAccessor != null)
			{
				summaryInspector.DrawField(position);
			}
			else
			{
				EditorGUI.LabelField(position, new GUIContent("Summary"), new GUIContent(staticSummary));
			}
		}

		protected virtual void OnFieldIconGUI(Rect position)
		{
			if (iconAccessor != null)
			{
				iconInspector.DrawField(position);
			}
			else
			{
				EditorGUI.LabelField(position, new GUIContent("Icon"), new GUIContent(staticIcon?[IconSize.Small]));
			}
		}

		#endregion



		#region Header / Asset Header

		protected float GetHeaderHeight(float width)
		{
			return LudiqGUI.GetHeaderHeight(GetHeaderTitleHeight, GetHeaderSummaryHeight, showIcon, width);
		}

		protected void OnHeaderGUI(Rect position)
		{
			LudiqGUI.OnHeaderGUI(GetHeaderTitleHeight, GetHeaderSummaryHeight, OnHeaderTitleGUI, OnHeaderSummaryGUI, OnHeaderIconGUI, position, ref y);
		}

		protected virtual float GetHeaderTitleHeight(float width)
		{
			if (titleAccessor != null)
			{
				return LudiqGUI.GetHeaderTitleHeight(this, titleAccessor, width);
			}
			else
			{
				return LudiqGUI.GetHeaderTitleHeight(staticTitle, width);
			}
		}

		protected virtual float GetHeaderSummaryHeight(float width)
		{
			if (summaryAccessor != null)
			{
				return LudiqGUI.GetHeaderSummaryHeight(this, summaryAccessor, width);
			}
			else
			{
				return LudiqGUI.GetHeaderSummaryHeight(staticSummary, width);
			}
		}

		protected virtual void OnHeaderTitleGUI(Rect position)
		{
			if (titleAccessor != null)
			{
				LudiqGUI.OnHeaderTitleGUI(titleAccessor, position, titlePlaceholder);
			}
			else
			{
				LudiqGUI.OnHeaderTitleGUI(staticTitle, position);
			}
		}

		protected virtual void OnHeaderSummaryGUI(Rect position)
		{
			if (summaryAccessor != null)
			{
				LudiqGUI.OnHeaderSummaryGUI(summaryAccessor, position);
			}
			else
			{
				LudiqGUI.OnHeaderSummaryGUI(staticSummary, position);
			}
		}

		protected virtual void OnHeaderIconGUI(Rect position)
		{
			if (iconAccessor != null)
			{
				LudiqGUI.OnHeaderIconGUI(iconAccessor, position, iconPlaceholder);
			}
			else
			{
				LudiqGUI.OnHeaderIconGUI(staticIcon, position);
			}
		}

		#endregion



		#region Component Header

		protected void OnComponentHeaderGUI(Rect position)
		{
			if (showTitle)
			{
				OnComponentHeaderTitleGUI(position);
			}

			if (showIcon)
			{
				OnComponentHeaderIconGUI(position);
			}
		}

		protected virtual void OnComponentHeaderTitleGUI(Rect position)
		{
			if (titleAccessor != null)
			{
				LudiqGUI.DrawCustomComponentTitleField(position, titleAccessor, titlePlaceholder);
			}
			else
			{
				LudiqGUI.DrawCustomComponentTitle(position, staticTitle);
			}
		}

		protected virtual void OnComponentHeaderIconGUI(Rect position)
		{
			if (iconAccessor != null)
			{
				LudiqGUI.DrawCustomComponentIconField(position, iconAccessor, iconPlaceholder);
			}
			else
			{
				LudiqGUI.DrawCustomComponentIcon(position, staticIcon);
			}
		}

		#endregion



		#region Foldout

		protected virtual bool showFoldoutExtra => false;

		protected float GetFoldoutHeight(float width)
		{
			var height = EditorGUIUtility.singleLineHeight;

			if (showFoldoutExtra)
			{
				var extraWidth = GetFoldoutExtraWidth(width);
				var extraHeight = GetFoldoutExtraHeight(extraWidth);
				height = Mathf.Max(height, extraHeight);
			}

			return height;
		}

		protected void OnFoldoutGUI(Rect position)
		{
			var foldoutPosition = new Rect
			(
				position.x + Styles.foldoutArrowWidth,
				y,
				0,
				GetFoldoutHeight(position.width)
			);

			var hasContent = GetInnerHeight(position.width) > 0;

			foldoutExpanded = hasContent && EditorGUI.Foldout(foldoutPosition, foldoutExpanded, GUIContent.none);

			var iconPosition = new Rect
			(
				foldoutPosition.xMax + Styles.spaceBetweenFoldoutArrowAndIcon,
				foldoutPosition.y,
				IconSize.Small,
				IconSize.Small
			);

			OnFoldoutIconGUI(iconPosition);

			var foldoutContentPosition = new Rect
			(
				iconPosition.xMax + Styles.spaceBetweenIconAndName,
				foldoutPosition.y,
				position.xMax - iconPosition.xMax - Styles.spaceBetweenIconAndName,
				foldoutPosition.height
			);

			if (showFoldoutExtra)
			{
				var titleWidth = GetFoldoutTitleWidth();

				var remainingWidth = foldoutContentPosition.width -
				                     Styles.foldoutArrowWidth -
				                     Styles.spaceBetweenFoldoutArrowAndIcon -
				                     IconSize.Small -
				                     Styles.spaceBetweenIconAndName -
				                     titleWidth -
				                     Styles.spaceBetweenTitleAndFoldoutExtra;

				var foldoutExtraWidth = GetFoldoutExtraWidth(remainingWidth);
				
				var foldoutExtraHeight = GetFoldoutExtraHeight(foldoutExtraWidth);

				var titlePosition = new Rect
				(
					foldoutContentPosition.x,
					foldoutContentPosition.y,
					foldoutContentPosition.width - foldoutExtraWidth - Styles.spaceBetweenTitleAndFoldoutExtra,
					EditorGUIUtility.singleLineHeight
				);

				var foldoutExtraPosition = new Rect
				(
					titlePosition.xMax + Styles.spaceBetweenTitleAndFoldoutExtra,
					foldoutContentPosition.y,
					foldoutExtraWidth,
					foldoutExtraHeight
				);

				OnFoldoutTitleGUI(titlePosition);
				OnFoldoutExtraGUI(foldoutExtraPosition);
			}
			else
			{
				OnFoldoutTitleGUI(foldoutContentPosition);
			}
			
			y = foldoutPosition.yMax;
		}

		protected virtual float GetFoldoutTitleWidth()
		{
			if (titleAccessor != null)
			{
				return titleInspector.ControlWidth();
			}
			else
			{
				return EditorStyles.label.CalcSize(new GUIContent(staticTitle)).x;
			}
		}

		protected virtual float GetFoldoutExtraWidth(float width)
		{
			throw new InvalidImplementationException();
		}

		protected virtual float GetFoldoutExtraHeight(float width)
		{
			throw new InvalidImplementationException();
		}

		protected virtual void OnFoldoutIconGUI(Rect position)
		{
			if (iconAccessor != null)
			{
				LudiqGUI.ObjectField(position, iconAccessor, UnityObjectFieldVisualType.Thumbnail, true, iconPlaceholder?[IconSize.Small]);
			}
			else if (staticIcon != null)
			{
				GUI.DrawTexture(position, staticIcon[IconSize.Small]);
			}
		}

		protected virtual void OnFoldoutTitleGUI(Rect position)
		{
			if (titleAccessor != null)
			{
				titleInspector.DrawControl(position);
			}
			else
			{
				GUI.Label(position, staticTitle);
			}
		}

		protected virtual void OnFoldoutExtraGUI(Rect position)
		{
			throw new InvalidImplementationException();
		}

		#endregion



		private static class Styles
		{
			static Styles()
			{
				headerPadding = new RectOffset(10, 10, 10, 10);
				noPadding = new RectOffset(0, 0, 0, 0);
				foldoutPadding = new RectOffset(0, 0, 4, 4);
			}

			public static readonly RectOffset headerPadding;

			public static readonly RectOffset noPadding;

			public static readonly RectOffset foldoutPadding;

			public static readonly float labelWidth = 68;
			
			public static readonly float foldoutDefaultValueWidthThreshold = 120f;

			public static readonly float foldoutArrowWidth = 12;

			public static readonly float spaceBetweenFoldoutArrowAndIcon = 4;

			public static readonly float spaceBetweenIconAndName = 4;

			public static readonly float spaceBetweenTitleAndFoldoutExtra = 4;
		}
	}
}