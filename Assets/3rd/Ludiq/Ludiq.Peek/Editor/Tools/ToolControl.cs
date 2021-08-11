using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class ToolControl
	{
		private static Event e => Event.current;

		public ITool tool { get; }

		private Rect _screenPosition;

		public Rect screenPosition
		{
			get => _screenPosition;
			set => _screenPosition = value;
		}

		public Rect guiPosition
		{
			get => GUIUtility.ScreenToGUIRect(screenPosition);
			set => screenPosition = LudiqGUIUtility.GUIToScreenRect(value);
		}

		public EventModifiers shortcutModifiers = EventModifiers.None;

		public int? shortcutIndex { get; set; } 

		public string shortcutLabel { get; set; }
		
		public ToolbarControl toolbarControl { get; }

		public Rect activatorScreenPosition
		{
			get
			{
				if (toolbarControl.isActivator)
				{
					return toolbarControl.screenPosition;
				}
				else
				{
					return screenPosition;
				}
			}
		}

		public Rect activatorGuiPosition
		{
			get
			{
				if (toolbarControl.isActivator)
				{
					return toolbarControl.guiPosition;
				}
				else
				{
					return guiPosition;
				}
			}
		}
		
		private Rect previousScreenPosition;

		private bool isPressed;

		private bool isDragging;

		private bool isDropping;
		
		public ToolControl(ToolbarControl toolbarControl, ITool tool)
		{
			Ensure.That(nameof(tool)).IsNotNull(toolbarControl);
			Ensure.That(nameof(tool)).IsNotNull(tool);

			this.toolbarControl = toolbarControl;
			this.tool = tool;
		}

		private void HandleDragAndDrop()
		{
			var isHovered = guiPosition.Contains(e.mousePosition);
			
			// Handle Drag
			if (e.button == (int)MouseButton.Left && e.modifiers == EventModifiers.None)
			{
				if (e.type == EventType.MouseDown)
				{
					if (isHovered && !isPressed)
					{
						isPressed = true;
						GUIUtility.hotControl = 0;
					}
				}
				else if (e.type == EventType.MouseDrag)
				{
					if (isPressed && !isHovered && !isDragging)
					{
						if (tool.OnDragEntered(this))
						{
							isDragging = true;
							e.Use();
							GUIUtility.hotControl = 0;
						}
					}
				}
			}

			// Handle Drop
			if (e.rawType == EventType.DragUpdated)
			{
				if (isHovered && !isDropping)
				{
					if (tool.OnDropEntered(this))
					{
						isDropping = true;
					}

					e.Use();
				}
				else if (isHovered && isDropping)
				{
					tool.OnDropUpdated(this);
					e.Use();
				}
				else if (!isHovered && isDropping)
				{
					tool.OnDropExited(this);
					isDropping = false;
					e.Use();
				}
			}
			
			// Exit
			if (e.rawType == EventType.DragExited)
			{
				if (isDragging)
				{
					tool.OnDragExited(this);
					isDragging = false;
				}

				if (isDropping)
				{
					tool.OnDropExited(this);
					isDropping = false;
				}

				isPressed = false;
			}

			if (e.rawType == EventType.MouseUp)
			{
				isPressed = false;
			}
		}
		
		public void DrawInTreeView(bool isVisible, Rect visibleRect, bool fixReadability)
		{
			using (LudiqGUIUtility.iconSize.Override(IconSize.Small))
			{
				var isActive = tool.isActive;
				bool wantsActive = false;
				var showPreview = PeekPlugin.Configuration.enablePreviewIcons && tool.preview != null;

				var icon = showPreview ? tool.preview : tool.icon;
				var toolContent = isVisible ? LudiqGUIUtility.TempContent(tool.label) : GUIContent.none;
				var toolStyle = isVisible ? tool.treeViewStyle : GUIStyle.none;
				
				HandleDragAndDrop();

				var iconPosition = new Rect
				(
					guiPosition.x + ((guiPosition.width - tool.iconSize.x) / 2),
					guiPosition.y + ((guiPosition.height - tool.iconSize.y) / 2),
					tool.iconSize.x,
					tool.iconSize.y
				);
				
				using (LudiqGUI.color.Override(LudiqGUI.color.value.WithAlphaMultiplied(tool.isDimmed ? 0.5f : 1)))
				{
					wantsActive = LudiqGUI.DropdownToggle(guiPosition, isActive, toolContent, toolStyle);

					if (isVisible)
					{
						if (fixReadability && LudiqGUIUtility.isFlatSkin && !EditorGUIUtility.isProSkin && !showPreview)
						{
							LudiqGUI.DrawTextureColored(iconPosition, icon, Color.white.WithAlpha(0.9f));
						}
						else
						{
							GUI.DrawTexture(iconPosition, icon);
						}
					}
				}

				if (isVisible && tool.overlay != null)
				{
					var overlayPosition = new Rect(guiPosition.position, tool.iconSize);
					
					GUI.DrawTexture(overlayPosition, tool.overlay);
				}

				tool.OnGUI(this);

				if (e.type != EventType.Layout && screenPosition != previousScreenPosition)
				{
					tool.OnMove(this);
				}

				if (guiPosition.Contains(e.mousePosition))
				{
					var tooltipContent = LudiqGUIUtility.TempContent(tool.tooltip);
					var tooltipStyle = PeekStyles.treeViewTooltip;
					var tooltipSize = tooltipStyle.CalcSize(tooltipContent);

					var tooltipPosition = new Rect
					(
						guiPosition.center.x - (tooltipSize.x / 2),
						guiPosition.yMin - tooltipSize.y - tooltipStyle.margin.bottom,
						tooltipSize.x,
						tooltipSize.y
					);

					tooltipPosition.x = Mathf.Clamp
					(
						tooltipPosition.x,
						visibleRect.xMin,
						visibleRect.xMax - tooltipPosition.width
					);

					GUI.Label(tooltipPosition, tooltipContent, tooltipStyle);
				}

				if (wantsActive != isActive)
				{
					if (wantsActive)
					{
						if (tool.isTransient)
						{
							toolbarControl.CloseAllTransientTools();
						}
						
						if (e.IsContextMouseButton())
						{
							tool.OpenContextual(this);
						}
						else
						{
							tool.Open(this);
						}
					}
					else
					{
						tool.Close(this);

						if (e.IsContextMouseButton())
						{
							tool.OpenContextual(this);
						}
					}
				}
			}

			if (e.type != EventType.Layout)
			{
				previousScreenPosition = screenPosition;
			}
		}

		public Vector2 GetSceneViewSize(bool isFirst, bool isLast)
		{
			var style = tool.SceneViewStyle(isFirst, isLast);
			var content = LudiqGUIUtility.TempContent(tool.showText ? tool.label : string.Empty, tool.icon);

			using (LudiqGUIUtility.realIconSize.Override(tool.iconSize))
			{
				return style.CalcSize(content);
			}
		}
		 
		public DelayedTooltip? DrawInSceneView(bool isFirst, bool isLast)
		{
			var delayedTooltip = (DelayedTooltip?)null;

			var isActive = tool.isActive;
			var isDimmed = tool.isDimmed;
			var showText = tool.showText;
			var showPreview = PeekPlugin.Configuration.enablePreviewIcons && tool != toolbarControl.toolbar.mainTool && tool.preview != null;

			LudiqGUIUtility.realIconSize.BeginOverride(tool.iconSize);
			var icon = showPreview ? tool.preview : tool.icon;
			var content = new GUIContent(showText ? tool.label : string.Empty, ColorUtility.GetPixel(ColorPalette.transparent));
			var style = tool.SceneViewStyle(isFirst, isLast);

			if (e.type == EventType.Repaint && screenPosition != previousScreenPosition)
			{
				tool.OnMove(this);
			}

			var isHovered = guiPosition.Contains(e.mousePosition);

			if (isDimmed && e.type == EventType.Repaint)
			{
				style.Draw(guiPosition, false, false, false, false);
			}
			
			var hasShortcut = shortcutIndex.HasValue && e.modifiers == shortcutModifiers && InternalEditorUtility.isApplicationActive;

			HandleDragAndDrop();

			LudiqGUI.color.BeginOverride(LudiqGUI.color.value.WithAlphaMultiplied(isDimmed ? 0.5f : 1));

			var wantsActive = LudiqGUI.DropdownToggle(guiPosition, isActive, content, style);

			if (icon != null)
			{
				var iconPosition = new Rect
				(
					guiPosition.x + style.padding.left,
					guiPosition.y + style.padding.top + (guiPosition.height / 2 - tool.iconSize.y / 2),
					tool.iconSize.x,
					tool.iconSize.y
				);

				if (isActive && LudiqGUIUtility.isFlatSkin && !EditorGUIUtility.isProSkin && !showPreview)
				{
					LudiqGUI.DrawTextureColored(iconPosition, icon, Color.white);
				}
				else
				{
					GUI.DrawTexture(iconPosition, icon);
				}
			}

			LudiqGUI.color.EndOverride();
			LudiqGUIUtility.realIconSize.EndOverride();

			if (tool.overlay != null)
			{
				var overlayPosition = new Rect(guiPosition.position + new Vector2(6, 2), tool.iconSize);

				GUI.DrawTexture(overlayPosition, tool.overlay);
			}

			if (wantsActive != isActive)
			{
				if (wantsActive)
				{
					if (tool.isTransient)
					{
						toolbarControl.CloseAllTransientTools();
					}

					if (e.IsContextMouseButton())
					{
						tool.OpenContextual(this);
					}
					else
					{
						tool.Open(this);
					}
				}
				else
				{
					tool.Close(this);

					if (e.IsContextMouseButton())
					{
						tool.OpenContextual(this);
					}
				}

				e.Use();
			}

			tool.OnGUI(this);

			var showTooltip = isHovered || hasShortcut;

			if (showTooltip)
			{
				var tooltipContent = new GUIContent(hasShortcut ? shortcutLabel : tool.tooltip);
				var tooltipStyle = PeekStyles.sceneViewTooltip;
				var tooltipSize = tooltipStyle.CalcSize(tooltipContent);

				var tooltipPosition = new Rect
				(
					guiPosition.center.x - (tooltipSize.x / 2),
					guiPosition.yMin - tooltipSize.y - tooltipStyle.margin.bottom,
					tooltipSize.x,
					tooltipSize.y
				);

				delayedTooltip = new DelayedTooltip()
				{
					screenPosition = LudiqGUIUtility.GUIToScreenRect(tooltipPosition),
					content = tooltipContent,
					style = tooltipStyle
				};
			}

			if (e.type == EventType.Repaint)
			{
				previousScreenPosition = screenPosition;
			}

			// EditorGUI.DrawRect(guiPosition, Color.red.WithAlpha(0.5f));

			return delayedTooltip;
		}

		public void Open()
		{
			tool.Open(this);
		}

		public void Close()
		{
			tool.Close(this);
		}

		public void Toggle()
		{
			if (tool.isActive)
			{
				Open();
			}
			else
			{
				Close();
			}
		}
	}
}