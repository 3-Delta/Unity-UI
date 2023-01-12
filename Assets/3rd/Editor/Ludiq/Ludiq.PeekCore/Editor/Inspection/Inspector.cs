using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class Inspector : IDisposable
	{
		static Inspector()
		{
			EditorApplicationUtility.onSelectionChange += blockStack.Clear;
		}

		protected Inspector(Accessor accessor)
		{
			Ensure.That(nameof(accessor)).IsNotNull(accessor);

			this.accessor = accessor;
			accessor.valueChanged += previousValue => SetHeightDirty();
			
			wideAttribute = accessor.GetAttribute<InspectorWideAttribute>();
			spaceAttribute = accessor.GetAttribute<InspectorSpaceAttribute>();
			expandTooltipAttribute = accessor.GetAttribute<InspectorExpandTooltipAttribute>();

			AutoLabel();
		}

		public virtual void Initialize() { } // Allows configuration before initialization

		public virtual void Dispose()
		{
			foreach (var child in children)
			{
				child.Value.Dispose();
			}
		}

		public Accessor accessor { get; }

		public Inspector parent { get; private set; }

		private readonly Dictionary<Accessor, Inspector> children = new Dictionary<Accessor, Inspector>();

		protected void FreeInvalidChildren()
		{
			var toFree = HashSetPool<Inspector>.New();

			foreach (var child in children)
			{
				if (!child.Key.isLinked)
				{
					toFree.Add(child.Value);
				}
			}

			foreach (var invalid in toFree)
			{
				invalid.Dispose();
				children.Remove(invalid.accessor);
			}

			toFree.Free();
		}

		protected virtual Inspector CreateChildInspector(Accessor accessor)
		{
			return accessor.CreateUninitializedInspector();
		}

		public TInspector ChildInspector<TInspector>(Accessor accessor, Action<TInspector> configure = null) where TInspector : Inspector
		{
			if (!children.TryGetValue(accessor, out var inspector))
			{
				var tInspector = CreateChildInspector(accessor).CastTo<TInspector>();
				tInspector.parent = this;
				configure?.Invoke(tInspector);
				tInspector.Initialize();
				children.Add(accessor, tInspector);
				return tInspector;
			}

			return inspector.CastTo<TInspector>();
		}

		public Inspector ChildInspector(Accessor accessor, Action<Inspector> configure = null)
		{
			return ChildInspector<Inspector>(accessor, configure);
		}

		public Inspector ChildInspector(string key, Action<Inspector> configure = null)
		{
			return ChildInspector(accessor[key], configure);
		}

		protected virtual Editor CreateChildEditor(Accessor accessor)
		{
			return accessor.CreateUninitializedEditor();
		}

		public TEditor ChildEditor<TEditor>(Accessor accessor, Action<TEditor> configure = null) where TEditor : Editor
		{
			if (!children.TryGetValue(accessor, out var editor))
			{
				var tEditor = CreateChildEditor(accessor).CastTo<TEditor>();
				tEditor.parent = this;
				configure?.Invoke(tEditor);
				tEditor.Initialize();
				children.Add(accessor, tEditor);
				return tEditor;
			}

			return editor.CastTo<TEditor>();
		}

		public Editor ChildEditor(Accessor accessor, Action<Editor> configure = null)
		{
			return ChildEditor<Editor>(accessor, configure);
		}

		public Editor ChildEditor(string key, Action<Editor> configure = null)
		{
			return ChildEditor(accessor[key], configure);
		}



		#region Label

		public static GUIContent GetLabel(Accessor accessor)
		{
			var attribute = accessor.GetAttribute<InspectorLabelAttribute>();

			if (attribute != null)
			{
				return new GUIContent(attribute.text, attribute.image ?? accessor.label.image, attribute.tooltip ?? accessor.label.tooltip);
			}
			else
			{
				return accessor.label ?? GUIContent.none;
			}
		}

		public void Label(GUIContent label)
		{
			this.label = label ?? GetLabel(accessor);

			SetHeightDirty();
		}

		public void AutoLabel()
		{
			Label(null);
		}

		public void NoLabel()
		{
			Label(GUIContent.none);
		}

		protected virtual GUIStyle LabelStyle(GUIStyle original)
		{
			if (labelStyle == null)
			{
				return EditorStyles.label;
			}
			else
			{
				return original;
			}
		}

		public GUIContent label { get; set; }

		public GUIStyle labelStyle { get; set; }

		#endregion



		#region Error Recovery

		protected internal bool safe => !LudiqCore.Configuration.developerMode;

		protected internal Exception onGuiException;

		protected internal Exception getHeightException;

		#endregion



		#region Attributes
		
		private readonly InspectorWideAttribute wideAttribute;

		private readonly InspectorSpaceAttribute spaceAttribute;

		private readonly InspectorExpandTooltipAttribute expandTooltipAttribute;

		#endregion



		#region Implementation

		protected float y;

		protected virtual float GetControlWidth()
		{
			return 50;
		}
		
		protected virtual float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		protected virtual void OnControlGUI(Rect position)
		{
			throw new InvalidImplementationException();
		}

		protected virtual float GetFieldHeight(float width)
		{
			var controlWidth = WidthWithoutLabel(width);
			var controlHeight = GetControlHeight(controlWidth);
			return HeightWithLabel(width, controlHeight);
		}

		protected virtual void OnFieldGUI(Rect position)
		{
			var controlPosition = PrefixLabel(position);
			controlPosition.height = GetControlHeight(controlPosition.width);
			OnControlGUI(controlPosition);
		}

		#endregion



		#region Dimension Caching

		protected virtual bool cacheControlHeight => true;

		protected virtual bool cacheFieldHeight => true;

		private float cachedControlHeight;

		private int cachedControlHeightHash;

		private float cachedFieldHeight;

		private int cachedFieldHeightHash;

		private bool isControlHeightDirty;

		private bool isFieldHeightDirty;

		public bool isHeightDirty => isControlHeightDirty || isFieldHeightDirty;

		public void SetHeightDirty()
		{
			isControlHeightDirty = true;
			isFieldHeightDirty = true;

			parent?.SetHeightDirty();
		}

		private int GetControlHeightHash(float width)
		{
			unchecked
			{
				var hash = 17;

				hash = (hash * 23) + width.GetHashCode();
				hash = (hash * 23) + wideMode.GetHashCode();
				hash = (hash * 23) + LudiqGUIUtility.currentInspectorWidthWithoutScrollbar.GetHashCode();

				return hash;
			}
		}

		private int GetFieldHeightHash(float width)
		{
			unchecked
			{
				var hash = 17;

				hash = (hash * 23) + width.GetHashCode();
				hash = (hash * 23) + wideMode.GetHashCode();
				hash = (hash * 23) + LudiqGUIUtility.currentInspectorWidthWithoutScrollbar.GetHashCode();
				hash = (hash * 23) + LudiqGUIUtility.labelWidth.value.GetHashCode();

				return hash;
			}
		}

		public float ControlHeight(float width)
		{
			if (!cacheControlHeight)
			{
				SetHeightDirty();
			}

			var heightHash = GetControlHeightHash(width);

			if (isControlHeightDirty || heightHash != cachedControlHeightHash)
			{
				try
				{
					EnsureLabelStyle();
					cachedControlHeight = GetControlHeight(width);
					getHeightException = null;
				}
				catch (ExitGUIException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (safe)
					{
						getHeightException = ex;
						return LudiqGUIUtility.HelpBoxHeight;
					}
					else
					{
						throw;
					}
				}

				cachedControlHeightHash = heightHash;
				isControlHeightDirty = false;
			}

			return cachedControlHeight;
		}

		public float FieldHeight(float width)
		{
			if (!cacheFieldHeight)
			{
				SetHeightDirty();
			}

			var heightHash = GetFieldHeightHash(width);

			if (isFieldHeightDirty || heightHash != cachedFieldHeightHash)
			{
				try
				{
					EnsureLabelStyle();
					cachedFieldHeight = GetFieldHeight(width);
					getHeightException = null;
				}
				catch (ExitGUIException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (safe)
					{
						getHeightException = ex;
						return LudiqGUIUtility.HelpBoxHeight;
					}
					else
					{
						throw;
					}
				}

				if (spaceAttribute != null)
				{
					cachedFieldHeight += spaceAttribute.above + spaceAttribute.below;
				}

				cachedFieldHeightHash = heightHash;
				isFieldHeightDirty = false;
			}

			return cachedFieldHeight;
		}

		public float ControlWidth()
		{
			return GetControlWidth();
		}

		#endregion



		protected internal void EnsureLabelStyle()
		{
			// Doing this late as we can't call EditorStyles before the first GUI call
			if (labelStyle == null)
			{
				labelStyle = LabelStyle(EditorStyles.label);
			}
		}

		protected internal bool ShieldDraw(Rect position)
		{
			if (e.ShouldSkip(position))
			{
				return false;
			}

			if (safe)
			{
				var exception = onGuiException ?? getHeightException;

				if (exception != null)
				{
					EditorGUI.HelpBox(position, $"Error drawing {GetType().Name}.", MessageType.Warning);

					if (GUI.Button(position, GUIContent.none, GUIStyle.none))
					{
						Debug.LogException(onGuiException);
					}

					return false;
				}
			}


			return true;
		}

		protected virtual void OnChange() { }

		public void DrawControl(Rect position)
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
				OnControlGUI(position);

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

		public void DrawField(Rect position, GUIStyle labelStyle = null)
		{
			if (!ShieldDraw(position))
			{
				return;
			}

			if (labelStyle != null)
			{
				this.labelStyle = labelStyle;
			}

			Rect blockPosition;

			if (spaceAttribute != null)
			{
				blockPosition = new Rect
				(
					position.x,
					position.y + spaceAttribute.above,
					position.width,
					position.height - spaceAttribute.above - spaceAttribute.below
				);
			}
			else
			{
				blockPosition = position;
			}

			y = blockPosition.y;

			try
			{
				EnsureLabelStyle();
				BeginBlock(blockPosition);
				OnFieldGUI(blockPosition);

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

		public void DrawControl(Rect parentPosition, ref float y)
		{
			DrawControl(parentPosition.VerticalSection(ref y, ControlHeight(parentPosition.width)));
		}

		public void DrawField(Rect parentPosition, ref float y)
		{
			DrawField(parentPosition.VerticalSection(ref y, FieldHeight(parentPosition.width)));
		}



		#region Helpers

		public static OverrideStack<bool> expandTooltip { get; } = new OverrideStack<bool>(false);

		public static OverrideStack<bool> adaptiveWidth { get; } = new OverrideStack<bool>(false);

		protected static Event e => Event.current;

		protected bool wideMode => LudiqGUIUtility.currentInspectorWidth > wideModeThreshold;

		protected virtual float wideModeThreshold => 332;

		private static readonly GUIContent tempLabelWithoutTooltipContent = new GUIContent();

		private static readonly GUIContent tempLabelTooltipContent = new GUIContent();

		private static GUIContent LabelWithoutTooltip(GUIContent label)
		{
			tempLabelWithoutTooltipContent.text = label.text;
			tempLabelWithoutTooltipContent.image = label.image;
			return tempLabelWithoutTooltipContent;
		}

		private static GUIContent LabelTooltip(GUIContent label)
		{
			tempLabelTooltipContent.text = label.tooltip;
			return tempLabelTooltipContent;
		}

		public float LabelWidth(float width)
		{
			var labelWidth = LudiqGUIUtility.labelWidth.value;

			if (wideAttribute != null)
			{
				labelWidth = width;
			}

			return labelWidth;
		}

		public float WidthWithoutLabel(float width)
		{
			return width - LabelWidth(width);
		}

		public float HeightWithLabel(float width, float height)
		{
			var labelWidth = LabelWidth(width);

			var wide = wideAttribute != null;

			var labelHeight = labelStyle.CalcHeight(label, labelWidth);

			if (expandTooltip.value || expandTooltipAttribute != null)
			{
				var tooltipHeight = StringUtility.IsNullOrWhiteSpace(label.tooltip) ? 0 : LudiqStyles.expandedTooltip.CalcHeight(LabelTooltip(label), labelWidth);

				if (wide)
				{
					height += labelHeight + tooltipHeight;
				}
				else
				{
					height = Mathf.Max(height, labelHeight + tooltipHeight);
				}
			}
			else
			{
				if (wide)
				{
					height += labelHeight;
				}
				else
				{
					height = Mathf.Max(height, labelHeight);
				}
			}

			return height;
		}

		private static Rect PrefixLabel(Rect position, GUIContent label, GUIStyle labelStyle, float width, bool wide, bool expandTooltip)
		{
			if (label == GUIContent.none)
			{
				return position;
			}

			var y = position.y;

			var labelPosition = new Rect
			(
				position.x,
				position.y,
				width,
				labelStyle.CalcHeight(label, width)
			);

			y = labelPosition.yMax + 2;

			if (expandTooltip && !string.IsNullOrEmpty(label.tooltip))
			{
				EditorGUI.LabelField(labelPosition, LabelWithoutTooltip(label), labelStyle);

				var tooltip = new GUIContent(label.tooltip);

				var tooltipPosition = new Rect
				(
					position.x,
					y - 2,
					width,
					LudiqStyles.expandedTooltip.CalcHeight(tooltip, width)
				);

				EditorGUI.LabelField(tooltipPosition, tooltip, LudiqStyles.expandedTooltip);

				if (LudiqCore.Configuration.developerMode && LudiqCore.Configuration.debugInspectorGUI)
				{
					LudiqGUI.DrawEmptyRect(tooltipPosition, Color.green.WithAlpha(0.5f));
				}

				y = tooltipPosition.yMax - 4;
			}
			else
			{
				EditorGUI.LabelField(labelPosition, label, labelStyle);
			}

			if (LudiqCore.Configuration.developerMode && LudiqCore.Configuration.debugInspectorGUI)
			{
				LudiqGUI.DrawEmptyRect(labelPosition, Color.cyan.WithAlpha(0.5f));
			}

			Rect remainingPosition;

			if (wide)
			{
				remainingPosition = new Rect
				(
					position.x,
					y,
					position.width,
					position.height - labelPosition.height
				);
			}
			else
			{
				remainingPosition = new Rect
				(
					labelPosition.xMax,
					position.y,
					position.width - labelPosition.width,
					position.height
				);
			}

			return remainingPosition;
		}

		public static Rect PrefixLabel(Rect position, GUIContent label)
		{
			return PrefixLabel
			(
				position,
				label,
				EditorStyles.label,
				LudiqGUIUtility.labelWidth.value,
				false,
				expandTooltip
			);
		}

		public Rect PrefixLabel(Rect position)
		{
			return PrefixLabel
			(
				position,
				label,
				labelStyle,
				LabelWidth(position.width),
				wideAttribute != null,
				expandTooltip || expandTooltipAttribute != null
			);
		}

		#endregion



		#region Blocks

		internal static Stack<InspectorBlock> blockStack { get; } = new Stack<InspectorBlock>();

		internal static InspectorBlock currentBlock => blockStack.Peek();

		public static void BeginBlock(Accessor accessor, Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var disabled =
				!accessor.isEditable || 
				accessor.HasAttribute<InspectorReadOnlyAttribute>() || 
				(accessor.isPrefabInstance && !accessor.supportsPrefabModifications);

			EditorGUI.BeginDisabledGroup(disabled);

			// Invoking editorHasBoldFont is expensive, so we're avoiding it as much as possible
			var bolded = false;

			if (accessor.hasPrefabModifications)
			{
				LudiqGUIUtility.editorHasBoldFont = true;
				bolded = true;
			}

			blockStack.Push(new InspectorBlock(accessor, position, bolded));
		}

		public void BeginBlock(Rect position)
		{
			BeginBlock(accessor, position);
		}

		public bool EndBlock()
		{
			return EndBlock(accessor);
		}

		public static bool EndBlock(Accessor accessor)
		{
			if (blockStack.Count == 0)
			{
				Debug.LogWarning("Ending unstarted inspector block.");
				return false;
			}

			var block = blockStack.Pop();

			if (block.accessor != accessor)
			{
				Debug.LogWarning($"Inspector block accessor mismatch.\nStarted {block.accessor}, ended {accessor}.");

				if (LudiqCore.Configuration.developerMode && LudiqCore.Configuration.debugInspectorGUI && e.type == EventType.Repaint)
				{
					LudiqGUI.DrawEmptyRect(block.position, Color.red);
				}
			}
			else
			{
				if (e.type == EventType.ContextClick && block.position.Contains(e.mousePosition))
				{
					if (block.accessor.isRevertibleToPrefab)
					{
						var menu = new GenericMenu();

						menu.AddItem(new GUIContent($"Revert {block.accessor.label.text} to Prefab"), false, () => { block.accessor.RevertToPrefab(); });

						menu.ShowAsContext();
					}

					e.Use();
				}

				if (LudiqCore.Configuration.developerMode && LudiqCore.Configuration.debugInspectorGUI && e.type == EventType.Repaint)
				{
					LudiqGUI.DrawEmptyRect(block.position, Color.yellow.WithAlpha(0.5f));
				}
			}
		
			if (block.bolded)
			{
				LudiqGUIUtility.editorHasBoldFont = false;
			}

			EditorGUI.EndDisabledGroup();
			return EditorGUI.EndChangeCheck();
		}

		#endregion



		#region Layout

		private static Rect GetLayoutPosition(Func<float, float> getHeight, float scrollbarTrigger = LudiqGUIUtility.scrollBarWidth)
		{
			var position = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));

			LudiqGUIUtility.currentInspectorHasScrollbar = position.width < LudiqGUIUtility.currentInspectorWidth - scrollbarTrigger;

			position.height = getHeight(position.width);

			GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(0), GUILayout.Height(position.height));

			return position;
		}

		public Rect GetFieldLayoutPosition(float scrollbarTrigger = LudiqGUIUtility.scrollBarWidth)
		{
			return GetLayoutPosition(FieldHeight, scrollbarTrigger);
		}

		public void DrawFieldLayout(float scrollbarTrigger = LudiqGUIUtility.scrollBarWidth)
		{
			DrawField(GetFieldLayoutPosition(scrollbarTrigger));
		}

		public Rect GetControlLayoutPosition(float scrollbarTrigger = LudiqGUIUtility.scrollBarWidth)
		{
			return GetLayoutPosition(ControlHeight, scrollbarTrigger);
		}

		public void DrawControlLayout(float scrollbarTrigger = LudiqGUIUtility.scrollBarWidth)
		{
			DrawControl(GetControlLayoutPosition(scrollbarTrigger));
		}

		#endregion
	}
}