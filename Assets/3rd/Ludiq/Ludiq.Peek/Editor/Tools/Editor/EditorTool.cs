using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public abstract class EditorTool<T> : Tool, IEditorTool where T : UnityObject
	{
		public T[] targets { get; }

		protected T target
		{
			get
			{
				if (!hasSingleTarget)
				{
					throw new InvalidOperationException("Editor tool is editing multiple targets.");
				}

				return firstTarget;
			}
		}

		protected T firstTarget => targets[0];

		Type IEditorTool.targetType => typeof(T);

		protected virtual bool hasMultipleTargets => targets.Length > 1;

		protected virtual bool hasSingleTarget => targets.Length == 1;

		public override Texture2D preview
		{
			get
			{
				if (hasSingleTarget && PreviewUtility.TryGetPreview(target, out var preview))
				{
					return preview;
				}

				return null;
			}
		}

		public override string label
		{
			get
			{
				if (hasSingleTarget)
				{
					if (target.IsDestroyed())
					{
						return "(Destroyed)";
					}

					return target.name;
				}
				else
				{
					return "(Multiple)";
				}
			}
		}

		public override Texture2D icon
		{
			get
			{
				if (hasSingleTarget)
				{
					return target.Icon()?[IconSize.Small];
				}

				return base.icon;
			}
			protected set => base.icon = value;
		}

		public override string tooltip
		{
			get
			{
				if (hasSingleTarget)
				{
					return target.name;
				}

				return base.tooltip;
			}
			protected set => base.tooltip = value;
		}

		protected EditorTool(T[] targets) : base()
		{
			this.targets = targets;

			if (hasSingleTarget)
			{
				icon = target.Icon()?[IconSize.Small];
				tooltip = target.name;
			}
			else
			{
				icon = typeof(T).Icon()?[IconSize.Small];
				tooltip = "(Multiple)";
			}
		}

		protected EditorWindow window;

		protected ToolControl activatorControl;

		public override bool isActive => PopupWatcher.IsOpenOrJustClosed(window);

		public override void Open(ToolControl control)
		{
			activatorControl = control;
			PopupWatcher.Release(window);
			window = EditorPopup.Open(targets, control.activatorScreenPosition);
			PopupWatcher.Watch(window);
		}

		public override void OpenContextual(ToolControl control)
		{
			activatorControl = control;
			UnityObjectContextMenu.Open(targets, activatorControl.activatorGuiPosition);
		}

		public override void OnMove(ToolControl control)
		{
			if (window is IFollowingPopupWindow popup && control == activatorControl && GUIUtility.hotControl == 0)
			{
				popup.activatorPosition = activatorControl.activatorScreenPosition;
			}
		}

		public override void Close(ToolControl control)
		{
			window?.Close();
		}

		public override bool OnDragEntered(ToolControl control)
		{
			Close(control);
			DragAndDrop.PrepareStartDrag();
			// PrepareStartDrag doesn't seem to actually be clearing the path sometimes?
			DragAndDrop.objectReferences = new UnityObject[0];
			DragAndDrop.paths = new string[0];
			DragAndDrop.objectReferences = targets;
			DragAndDrop.StartDrag(label);
			return true;
		}

		private DateTime dropEnterTime;
		private bool dropActivated;

		public override bool OnDropEntered(ToolControl control)
		{
			if (!PeekPlugin.Configuration.enableStickyDragAndDrop)
			{
				return false;
			}

			DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			dropEnterTime = DateTime.UtcNow;
			dropActivated = false;
			return true;
		}

		public override void OnDropUpdated(ToolControl control)
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Link;

			if (!dropActivated && (DateTime.UtcNow - dropEnterTime).TotalSeconds > PeekPlugin.Configuration.dropActivationDelay)
			{
				DragAndDrop.AcceptDrag();
				
				control.toolbarControl.CloseAllTransientTools();

				// We also need to close all editor popups, not just those from the current toolbar,
				// to prevent multiple popups from opening in tree views

				foreach (var editorPopup in Resources.FindObjectsOfTypeAll<EditorPopup>())
				{
					editorPopup.Close();
				}

				if (!isActive)
				{
					Open(control);
				}

				dropActivated = true;
			}
		}
	}
}