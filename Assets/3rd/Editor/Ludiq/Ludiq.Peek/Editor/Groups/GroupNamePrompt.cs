using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class GroupNamePrompt : LudiqEditorWindow
	{
		public static bool Prompt(out string name, string defaultName = "Group")
		{
			var prompt = CreateInstance<GroupNamePrompt>();
			prompt.name = defaultName;
			prompt.minSize = prompt.maxSize = new Vector2(300, 70);
			prompt.ShowModal();
			name = prompt.name;
			return prompt.confirmed;
		}
		
		public new string name { get; private set; }

		public bool confirmed { get; private set; }

		private bool focused;

		protected override void OnEnable()
		{
			base.OnEnable();
			titleContent.text = "Group";
			this.Center();
		}

		protected override void OnGUI()
		{
			base.OnGUI();

			if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
			{
				Confirm();
			}
			else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
			{
				Cancel();
			}

			GUILayout.BeginVertical(Styles.fieldsArea);

			GUI.SetNextControlName("GroupNameField");
			name = EditorGUILayout.TextField("Group Name", name);

			GUILayout.EndVertical();

			if (!focused)
			{
				EditorGUI.FocusTextInControl("GroupNameField");
				focused = true;
			}
			
			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal(Styles.buttonsArea);

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Cancel"))
			{
				Cancel();
			}

			GUILayout.Space(2);

			if (GUILayout.Button("OK"))
			{
				Confirm();
			}

			GUILayout.EndHorizontal();
		}

		private void Confirm()
		{
			confirmed = true;
			Close();
			GUIUtility.ExitGUI();
		}

		private void Cancel()
		{
			confirmed = false;
			Close();
			GUIUtility.ExitGUI();
		}

		private static class Styles
		{
			static Styles()
			{
				fieldsArea = ColorPalette.unityBackgroundMid.CreateBackground();
				fieldsArea.padding = new RectOffset(8, 8, 8, 8);

				buttonsArea = ColorPalette.unityBackgroundDark.CreateBackground();
				buttonsArea.padding = new RectOffset(8, 8, 8, 8);
			}

			public static readonly GUIStyle fieldsArea;
			public static readonly GUIStyle buttonsArea;
		}
	}
}
