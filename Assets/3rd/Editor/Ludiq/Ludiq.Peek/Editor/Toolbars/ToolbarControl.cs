using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.ShortcutManagement;
#endif
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class ToolbarControl : IEnumerable<ToolControl>
	{
		private static Event e => Event.current;

		private IToolbar _toolbar;

		public IToolbar toolbar
		{
			get => _toolbar;
			set
			{
				Ensure.That(nameof(toolbar)).IsNotNull(value);
				_toolbar = value;
				Refresh();
			}
		}

		public ToolbarWindow window { get; private set; }

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

		public bool isDraggable { get; set; }

		public bool isActivator { get; set; }

		public bool isDragging { get; private set; }

		private readonly Dictionary<ITool, ToolControl> _toolControls = new Dictionary<ITool, ToolControl>();

		public IEnumerator<ToolControl> GetEnumerator()
		{
			foreach (var tool in toolbar)
			{
				yield return GetToolControl(tool);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Refresh()
		{
			_toolControls.Clear();
		}

		public ToolbarControl(IToolbar toolbar, ToolbarWindow window)
		{
			Ensure.That(nameof(toolbar)).IsNotNull(toolbar);

			this.toolbar = toolbar;
			this.window = window;
		}

		public ToolControl GetToolControl(ITool tool)
		{
			if (!_toolControls.TryGetValue(tool, out var toolControl))
			{
				toolControl = new ToolControl(this, tool);
				_toolControls.Add(tool, toolControl);
			}

			return toolControl;
		}

		public void CloseAllTools()
		{
			foreach (var toolControl in _toolControls.Values)
			{
				var tool = toolControl.tool;

				if (tool.isActive)
				{
					tool.Close(toolControl);
				}
			}
		}

		public void CloseAllTransientTools()
		{
			foreach (var toolControl in _toolControls.Values)
			{
				var tool = toolControl.tool;

				if (tool.isTransient && tool.isActive)
				{
					tool.Close(toolControl);
				}
			}
		}

		public void TriggerShortcut(int index)
		{
			foreach (var toolControl in this)
			{
				if (toolControl.shortcutIndex == index)
				{
					var tool = toolControl.tool;

					if (tool.isActive)
					{
						tool.Close(toolControl);
					}
					else
					{
						CloseAllTransientTools();
						tool.Open(toolControl);
					}

					e?.TryUse();
					return;
				}
			}
		}

		public void DrawMainToolInTreeView(Rect position, Rect visibleRect)
		{
			var tool = toolbar.mainTool;

			if (tool == null || !tool.IsVisible(this))
			{
				return;
			}

			var toolControl = GetToolControl(tool);

			toolControl.guiPosition = position;

			toolControl.DrawInTreeView(false, visibleRect, false);
		}

		public float GetTreeViewWidth()
		{
			var width = 0f;

			foreach (var tool in toolbar)
			{
				if (tool == toolbar.mainTool)
				{
					continue;
				}

				if (!tool.IsVisible(this))
				{
					continue;
				}

				width += IconSize.Small;
			}

			return width;
		}

		public void DrawInTreeView(Rect visibleRect, bool fixReadability)
		{
			var i = 0;

			var visibleToolCount = 0;

			foreach (var tool in toolbar)
			{
				if (tool.IsVisible(this))
				{
					visibleToolCount++;
				}
			}

			foreach (var tool in toolbar)
			{
				if (tool == toolbar.mainTool)
				{
					continue;
				}

				if (!tool.IsVisible(this))
				{
					continue;
				}

				var toolControl = GetToolControl(tool);

				toolControl.guiPosition = new Rect
				(
					guiPosition.x + (IconSize.Small * i),
					guiPosition.yMin,
					IconSize.Small,
					IconSize.Small
				);

				if (toolControl.guiPosition.xMax + IconSize.Small > guiPosition.xMax && i != visibleToolCount - 2)
				{
					break;
				}

				i++;

				toolControl.DrawInTreeView(true, visibleRect, fixReadability);
			}

			if (i < visibleToolCount - 1)
			{
				var morePosition = new Rect
				(
					guiPosition.x + (IconSize.Small * i),
					guiPosition.yMin,
					IconSize.Small,
					IconSize.Small
				);

				if (morePosition.Contains(e.mousePosition))
				{
					// TODO: DRY
					var tooltipContent = LudiqGUIUtility.TempContent("More...");
					var tooltipStyle = PeekStyles.treeViewTooltip;
					var tooltipSize = tooltipStyle.CalcSize(tooltipContent);

					var tooltipPosition = new Rect
					(
						morePosition.center.x - (tooltipSize.x / 2),
						morePosition.yMin - tooltipSize.y - tooltipStyle.margin.bottom,
						tooltipSize.x,
						tooltipSize.y
					);

					tooltipPosition.x = Mathf.Clamp
					(
						tooltipPosition.x,
						guiPosition.xMin,
						guiPosition.xMax - tooltipPosition.width
					);

					GUI.Label(tooltipPosition, tooltipContent, tooltipStyle);
				}

				var moreIcon = PeekPlugin.Icons.more?[IconSize.Small];

				LudiqGUIUtility.realIconSize.BeginOverride(moreIcon?.PointSize() ?? new Vector2(IconSize.Small, IconSize.Small));

				if (GUI.Button(morePosition, moreIcon, PeekStyles.treeViewMoreButton))
				{
					var menu = new GenericMenu();
					menu.allowDuplicateNames = true;

					for (var j = i; j < toolbar.Count; j++)
					{
						var tool = toolbar[j];

						if (!tool.IsVisible(this))
						{
							continue;
						}

						if (tool is MergedTool mergedTool)
						{
							foreach (var subtool in mergedTool.tools)
							{
								var subtoolLabel = subtool.tooltip;

								if (!mergedTool.expandable)
								{
									subtoolLabel = tool.tooltip + "/" + subtoolLabel;
								}

								var subtoolContent = new GUIContent(subtoolLabel, subtool.icon);
								var subtoolControl = GetToolControl(subtool);
								subtoolControl.guiPosition = morePosition;
								menu.AddItem(subtoolContent, subtool.isActive, () => { subtool.Open(subtoolControl); });
							}
						}
						else
						{
							var toolContent = new GUIContent(tool.tooltip, tool.icon);
							var toolControl = GetToolControl(tool);
							toolControl.guiPosition = morePosition;
							menu.AddItem(toolContent, tool.isActive, () => { tool.Open(toolControl); });
						}
					}

					menu.DropDown(morePosition);
				}

				LudiqGUIUtility.realIconSize.EndOverride();
			}
		}

		public Vector2 GetSceneViewSize()
		{
			var width = 0f;
			var height = 0f;

			if (isDraggable)
			{
				var handleStyle = PeekStyles.SceneViewTool(true, toolbar.Count == 0);
				var handleContent = LudiqGUIUtility.TempContent(PeekPlugin.Icons.toolbarDragHandle?[IconSize.Small]);

				width += handleStyle.CalcSize(handleContent).x;
			}

			for (var i = 0; i < toolbar.Count; i++)
			{
				var tool = toolbar[i];

				if (!tool.IsVisible(this))
				{
					continue;
				}

				var toolControl = GetToolControl(tool);

				var isFirst = i == 0;
				var isLast = i == toolbar.Count - 1;

				var toolSize = toolControl.GetSceneViewSize(isFirst, isLast);
				width += toolSize.x;
				height = Mathf.Max(height, toolSize.y);
			}

			return new Vector2(width, height);
		}

#if UNITY_2019_1_OR_NEWER
		private static KeyCombination? GetShortcutBinding(int index, bool primary)
		{
			var shortcutID = $"Peek/{(primary ? "Primary" : "Secondary")} Toolbar {index}";

			var shortcutExists = ShortcutManager.instance.GetAvailableShortcutIds().Contains(shortcutID);

			if (!shortcutExists)
			{
				return null;
			}

			var mapping = ShortcutManager.instance.GetShortcutBinding(shortcutID);

			if (!mapping.keyCombinationSequence.Any())
			{
				return null;
			}

			var combination = mapping.keyCombinationSequence.FirstOrDefault();

			if (combination.keyCode == KeyCode.None)
			{
				return null;
			}

			return combination;
		}
#endif

		private static bool HasShortcut(int index, bool primary)
		{
#if UNITY_2019_1_OR_NEWER
			return GetShortcutBinding(index, primary) != null;
#else
			return index <= 10;
#endif
		}

		private static string GetShortcutLabel(int index, bool primary)
		{
#if UNITY_2019_1_OR_NEWER
			return GetShortcutBinding(index, primary).Value.keyCode.ToNiceString();
#else
			return index.ToString();
#endif
		}

		public static EventModifiers GetShortcutModifiers(int index, bool primary)
		{
#if UNITY_2019_1_OR_NEWER
			var mapping = GetShortcutBinding(index, primary).Value;

			var modifiers = EventModifiers.None;

			if (mapping.action)
			{
				modifiers |= Application.platform == RuntimePlatform.OSXEditor ? EventModifiers.Command : EventModifiers.Control;
			}

			if (mapping.shift)
			{
				modifiers |= EventModifiers.Shift;
			}

			if (mapping.alt)
			{
				modifiers |= EventModifiers.Alt;
			}

			return modifiers;
#else
			if (primary)
			{
				return EventModifiers.Alt;
			}
			else
			{
				return EventModifiers.Alt | EventModifiers.Shift;
			}
#endif
		}

		public void DrawInSceneView()
		{
			var x = guiPosition.x;

			if (isDraggable)
			{
				var handleStyle = PeekStyles.SceneViewTool(true, toolbar.Count == 0);
				var handleContent = LudiqGUIUtility.TempContent(PeekPlugin.Icons.toolbarDragHandle?[IconSize.Small]);
				var handleControlId = GUIUtility.GetControlID(FocusType.Passive);
				var handleSize = handleStyle.CalcSize(handleContent);

				var handlePosition = new Rect
				(
					x,
					guiPosition.y,
					handleSize.x,
					handleSize.y
				);

				x = handlePosition.xMax;

				EditorGUIUtility.AddCursorRect(handlePosition, MouseCursor.MoveArrow, handleControlId);

				if (!isDragging &&
				    e.type == EventType.MouseDown &&
				    e.button == (int)MouseButton.Left &&
				    e.modifiers == EventModifiers.None &&
				    handlePosition.Contains(e.mousePosition))
				{
					GUIUtility.hotControl = handleControlId;
					isDragging = true;
					e.Use();
				}

				if (isDragging)
				{
					if (e.rawType == EventType.MouseUp)
					{
						isDragging = false;
						e.Use();
						GUIUtility.hotControl = 0;
					}
					else if (e.type == EventType.MouseDrag)
					{
						var origin = guiPosition.position;
						origin += e.delta;
						guiPosition = new Rect(origin, guiPosition.size);
						e.Use();
					}
				}

				GUI.Toggle(handlePosition, isDragging, handleContent, handleStyle);
			}

			var delayedTooltips = ListPool<DelayedTooltip>.New();

			var shortcutIndex = 1;

			for (var i = 0; i < toolbar.Count; i++)
			{
				var tool = toolbar[i];

				if (!tool.IsVisible(this))
				{
					continue;
				}

				var toolControl = GetToolControl(tool);

				var hasPrimaryShortcuts = ShortcutsIntegration.primaryToolbar == this;
				var hasSecondaryShortcuts = ShortcutsIntegration.secondaryToolbar == this;
				var hasShortcuts = hasPrimaryShortcuts || hasSecondaryShortcuts;

				if (hasShortcuts && tool.isShortcuttable)
				{
					if (HasShortcut(shortcutIndex, hasPrimaryShortcuts))
					{
						toolControl.shortcutIndex = shortcutIndex % 10;
						toolControl.shortcutLabel = GetShortcutLabel(shortcutIndex, hasPrimaryShortcuts);
						toolControl.shortcutModifiers = GetShortcutModifiers(shortcutIndex, hasPrimaryShortcuts);
					}

					shortcutIndex++;
				}
				else
				{
					toolControl.shortcutIndex = null;
					toolControl.shortcutModifiers = EventModifiers.None;
					toolControl.shortcutLabel = null;
				}

				var isFirst = i == 0;
				var isLast = i == toolbar.Count - 1;
				var size = toolControl.GetSceneViewSize(isFirst, isLast);

				if (e.type == EventType.Repaint)
				{
					toolControl.guiPosition = new Rect
					(
						x,
						guiPosition.y,
						size.x,
						size.y
					);
				}

				x = toolControl.guiPosition.xMax;

				var delayedTooltip = toolControl.DrawInSceneView(isFirst && !isDraggable, isLast);

				if (delayedTooltip.HasValue)
				{
					delayedTooltips.Add(delayedTooltip.Value);
				}
			}

			foreach (var delayedTooltip in delayedTooltips)
			{
				var position = GUIUtility.ScreenToGUIRect(delayedTooltip.screenPosition);
				GUI.Label(position, delayedTooltip.content, delayedTooltip.style);
			}

			delayedTooltips.Free();
		}
	}
}