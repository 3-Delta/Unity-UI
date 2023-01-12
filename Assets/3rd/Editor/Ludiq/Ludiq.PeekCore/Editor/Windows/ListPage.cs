using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class ListPage : Page
	{
		public ListPage(EditorWindow window) : base(window)
		{
			pages = new List<Page>();
			pageOptions = new List<ListOption>();
		}

		private Vector2 listScroll;
		private Page _currentPage;

		public List<Page> pages { get; }

		protected virtual bool flexible => false;

		public Page currentPage
		{
			get
			{
				return _currentPage;
			}
			set
			{
				currentPage?.Close();

				_currentPage = value;

				currentPage?.Show();
			}
		}

		private List<ListOption> pageOptions { get; }

		public void UpdateOptions()
		{
			pageOptions.Clear();
			pageOptions.AddRange(pages.Select(page => new ListOption(page, new GUIContent(page.shortTitle, null, page.subtitle))));
			currentPage = pages.FirstOrDefault();
		}

		protected override void OnShow()
		{
			base.OnShow();

			listScroll = Vector2.zero;

			UpdateOptions();
		}

		public override void Update()
		{
			if (currentPage != null)
			{
				if (currentPage.CompleteSwitch())
				{
					return;
				}

				currentPage.Update();
			}
		}

		protected virtual void OnEmptyGUI()
		{
			GUILayout.BeginVertical(Styles.emptyBackground);
			LudiqGUI.FlexibleSpace();
			GUILayout.Label("No item found.", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndVertical();
		}

		protected override void OnContentGUI()
		{
			if (pages.Count == 0)
			{
				OnEmptyGUI();
			}
			else
			{
				LudiqGUI.BeginHorizontal();
				OnSidebarGUI();
				GUILayout.Box(GUIContent.none, LudiqStyles.verticalSeparator);
				currentPage?.DrawContent();
				LudiqGUI.EndHorizontal();
			}
		}

		protected virtual void OnSidebarGUI()
		{
			listScroll = LudiqGUI.List(listScroll, flexible, pageOptions, currentPage, newPage => currentPage = (Page)newPage);
		}

		public static class Styles
		{
			static Styles()
			{
				emptyBackground = ColorPalette.unityBackgroundMid.CreateBackground();
				emptyBackground.padding = new RectOffset(10, 10, 10, 10);
			}

			public static readonly GUIStyle emptyBackground;
		}
	}
}