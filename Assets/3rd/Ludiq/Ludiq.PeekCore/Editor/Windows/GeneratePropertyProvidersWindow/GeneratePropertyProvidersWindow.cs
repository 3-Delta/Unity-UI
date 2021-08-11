using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class GeneratePropertyProvidersWindow : SinglePageWindow<GeneratePropertyProvidersPage>
	{
		protected override GeneratePropertyProvidersPage CreatePage()
		{
			return new GeneratePropertyProvidersPage(this);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			minSize = maxSize = new Vector2(400, 330);
		}
		
		public static GeneratePropertyProvidersWindow instance { get; private set; }

		// [MenuItem("Tools/Peek/Ludiq/Generate Custom Inspectors...", priority = LudiqProduct.ToolsMenuPriority + 302)]
		public new static void Show()
		{
			if (instance != null)
			{
				instance.Focus();
			}
			else
			{
				instance = CreateInstance<GeneratePropertyProvidersWindow>();
				instance.ShowUtility();
				instance.Center();
			}
		}
	}
}