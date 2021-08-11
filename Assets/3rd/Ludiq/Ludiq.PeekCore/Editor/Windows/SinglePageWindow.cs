using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class SinglePageWindow<TPage> : LudiqEditorWindow where TPage : Page
	{
		protected SinglePageWindow() { }

		private TPage _page;

		public TPage page
		{
			get
			{
				if (_page == null)
				{
					_page = CreatePage();

					if (_page == null)
					{
						throw new InvalidImplementationException();
					}

					_page.onComplete = Close;
				}

				return _page;
			}
		}

		protected abstract TPage CreatePage();
		
		protected override void Update()
		{
			if (!page.visible)
			{
				titleContent = new GUIContent(page.title, page.icon?[IconSize.Small]);
				page.Show();
			}

			if (page.CompleteSwitch())
			{
				return;
			}

			page.Update();
		}

		protected override void OnDisable()
		{
			page.Close();
		}

		protected override void OnGUI()
		{
			LudiqGUI.BeginVertical();
			page.DrawHeader();
			// GUILayout.Box(GUIContent.none, LudiqStyles.horizontalSeparator);
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
			page.DrawContent();
			LudiqGUI.EndHorizontal();
			LudiqGUI.EndVertical();
		}
	}
}