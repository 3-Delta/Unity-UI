using System;
using UnityEditor;
using UnityEngine;
using GUIEvent = UnityEngine.Event;

namespace Ludiq.PeekCore
{
	public abstract class Page
	{
		protected Page(EditorWindow window)
		{
			Ensure.That(nameof(window)).IsNotNull(window);
			this.window = window;
			completeLabel = "Done";
		}

		private Vector2 contentScroll;

		public EditorWindow window { get; }
		public string title { get; set; }
		public string shortTitle { get; set; }
		public string subtitle { get; set; }
		public EditorTexture icon { get; set; }
		public Action onComplete { private get; set; }
		public string completeLabel { get; set; }
		private bool shouldComplete;
		public bool visible { get; private set; }

		protected virtual void OnShow() { }

		public virtual void Update() { }

		public bool CompleteSwitch()
		{
			if (shouldComplete)
			{
				shouldComplete = false;
				onComplete.Invoke();
				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual void OnHeaderGUI()
		{
			LudiqGUI.WindowHeader(title, icon);
		}

		protected abstract void OnContentGUI();

		protected virtual void OnClose() { }

		public void Show()
		{
			if (visible)
			{
				return;
			}

			contentScroll = Vector2.zero;
			OnShow();
			visible = true;
		}

		public void Close()
		{
			if (!visible)
			{
				return;
			}

			OnClose();
			visible = false;
		}

		public void DrawHeader()
		{
			OnHeaderGUI();
		}

		public void DrawContent()
		{
			contentScroll = GUILayout.BeginScrollView(contentScroll, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			OnContentGUI();
			GUILayout.EndScrollView();
		}

		protected virtual void Complete()
		{
			shouldComplete = true;
		}
		
		protected static GUIEvent e => GUIEvent.current;
	}
}