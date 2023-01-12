using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UEditor = UnityEditor.Editor;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;
	using System.Collections.Generic;

	public sealed class HierarchyPopup : LudiqEditorWindow, IFollowingPopupWindow
	{
		public Rect activatorPosition { get; set; }

		private TreeViewState treeViewState;

		private HierarchyTreeView treeView;
		
		private static UEditor previewEditor;

		private static readonly List<GameObject> previewEditorTargets = new List<GameObject>(); 

		private bool focusSearch = true;

		private const string searchFieldName = "HierarchyPopupSearchField";
		
		public static HierarchyPopup Show(Rect activator)
		{
			return Show(null, activator);
		}

		public static HierarchyPopup Show(GameObject[] targets, Rect activator)
		{
			var popup = CreateInstance<HierarchyPopup>();
			popup.Initialize(targets);
			
			popup.activatorPosition = activator;
			popup.position = Rect.zero;
			popup.ShowAsDropDown(popup.activatorPosition, new Vector2(activator.width, 1));
			popup.minSize = new Vector2(200, 16);
			popup.maxSize = new Vector2(640, 640);

			return popup;
		}

		private void Initialize(GameObject[] targets)
		{
			treeViewState = new TreeViewState();
			treeView = new HierarchyTreeView(this, targets, treeViewState);
			treeView.Reload();
			treeView.AutoExpand();
		} 

		protected override void Update()
		{ 
			// Reloaded from serialization
			if (treeView == null)
			{
				Close();
				return;
			}

			base.Update();
			
			var height = LudiqStyles.searchFieldOuterHeight + treeView.totalHeight + 2;

			if (previewEditorTargets.Count > 0 &&
			    previewEditor != null &&
			    (previewEditor.HasPreviewGUI() || previewEditorTargets.Any(PreviewUtility.HasPreview)))
			{
				height += Styles.previewHeight;
			}

			height = Mathf.Min(height, maxSize.y);

			// FIX: Not calling GDPCropped to not crop on OSX, so that the scrollbar displays
			position = this.GetDropdownPosition(activatorPosition, new Vector2(position.width, height));
		}

		private void OnHierarchyChange()
		{
			treeView.Reload();
			Repaint();
		}

		private void OnProjectChange()
		{
			treeView.Reload();
			Repaint();
		}

		protected override void OnGUI()
		{
			base.OnGUI();
			
			// Reloaded from serialization
			if (treeView == null)
			{
				Close();
				GUIUtility.ExitGUI();
			}

			// Close on Escape
			if (e.type == EventType.KeyDown && e.modifiers == EventModifiers.None && e.keyCode == KeyCode.Escape)
			{
				Close();
				GUIUtility.ExitGUI();
			}

			var innerPosition = new Rect(0, 0, position.width, position.height);

			// Draw Background
			EditorGUI.DrawRect(innerPosition, ColorPalette.unityBackgroundMid);

			GUILayout.BeginVertical();

			// Draw Search
			GUILayout.BeginHorizontal(LudiqStyles.searchFieldBackground, GUILayout.Height(LudiqStyles.searchFieldOuterHeight), GUILayout.ExpandWidth(true));

			EditorGUI.BeginChangeCheck();

			GUI.SetNextControlName(searchFieldName);

			// Special keyboard controls  while search field is selected
			if (GUI.GetNameOfFocusedControl() == searchFieldName && e.type == EventType.KeyDown)
			{
				// Pass arrow events to tree view
				if (e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.UpArrow || !treeView.hasSearch && (e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow))
				{
					var selection = treeView.GetSelection();
					treeView.SetFocus();
					treeView.SetSelection(selection);
				}
				// Confirm search with enter
				else if (e.keyCode == KeyCode.Return)
				{
					if (treeView.SelectActive())
					{
						Close();
						GUIUtility.ExitGUI();
					}
					else
					{
						e.Use();
					}
				}
				// Close if pressing space again without search
				else if (e.keyCode == KeyCode.Space && !treeView.hasSearch)
				{
					Close();
					GUIUtility.ExitGUI();
				}
			}

			treeView.searchString = EditorGUILayout.TextField(treeView.searchString, LudiqStyles.searchField);

			// Focus on Search
			if (focusSearch && e.type == EventType.Repaint)
			{
				GUI.FocusControl(searchFieldName);
				focusSearch = false;
			}

			// Reload Tree View on Search
			if (EditorGUI.EndChangeCheck())
			{
				treeView.Reload();
			}

			// Search Cancel Button
			if (GUILayout.Button(GUIContent.none, treeView.hasSearch ? LudiqStyles.searchFieldCancelButton : LudiqStyles.searchFieldCancelButtonEmpty) && treeView.hasSearch)
			{
				treeView.searchString = string.Empty;
				treeView.Reload();
				GUIUtility.keyboardControl = 0;
			}

			GUILayout.EndHorizontal();

			// Horizontal Separator
			GUILayout.Box(GUIContent.none, LudiqStyles.horizontalSeparator);

			// Handle special keyboard strokes in tree view
			if (treeView.HasFocus() && e.type == EventType.KeyDown)
			{
				// Select current item
				if (e.keyCode == KeyCode.Space)
				{
					if (treeView.SelectActive())
					{
						Close();
						GUIUtility.ExitGUI();
					}
					else
					{
						e.Use();
					}
				}
				// Move back up to search field
				else if (e.keyCode == KeyCode.UpArrow)
				{
					if (treeView.GetSelection().FirstOrDefault() == treeView.GetRows().FirstOrDefault()?.id)
					{
						focusSearch = true;
						e.Use();
					}
				}
				// Delete character from search
				else if (e.keyCode == KeyCode.Backspace)
				{
					treeView.searchString = treeView.searchString.Substring(0, Mathf.Max(0, treeView.searchString.Length - 1));
					
					e.Use();
				}
				// Append characters to search
				else if (e.modifiers == EventModifiers.None && !char.IsWhiteSpace(e.character) && !char.IsControl(e.character))
				{
					treeView.searchString += e.character;

					e.Use();
				}
				// Focus search
				else if (e.keyCode == KeyCode.F && e.CtrlOrCmd())
				{
					focusSearch = true;
					e.Use();
				}
			}

			// Draw Tree View  
			var treeViewPosition = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			treeView.OnGUI(treeViewPosition);

			// Draw Preview
			previewEditorTargets.Clear();
			previewEditorTargets.AddRange(treeView.GetActiveGameObjects());

			if ((previewEditor == null && previewEditorTargets.Count > 0) || 
			    (previewEditor != null && !previewEditorTargets.SequenceEqual(previewEditor.targets)))
			{
				if (previewEditor != null)
				{
					DestroyImmediate(previewEditor);
					previewEditor = null;
				}

				previewEditor = UEditor.CreateEditor(previewEditorTargets.ToArray(), null);
			}

			if (previewEditor != null && (previewEditor.HasPreviewGUI() || previewEditorTargets.Any(PreviewUtility.HasPreview)))
			{
				GUILayout.Box(GUIContent.none, LudiqStyles.horizontalSeparator);
				var previewPosition = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(Styles.previewHeight), GUILayout.ExpandWidth(true));
				previewEditor.DrawPreview(previewPosition);
			}

			GUILayout.EndVertical();

			// Draw Border
			if (e.type == EventType.Repaint)
			{
				LudiqGUI.DrawEmptyRect(new Rect(Vector2.zero, position.size), ColorPalette.unityBackgroundVeryDark);
			}

			// Repaint on hover
			if (innerPosition.Contains(e.mousePosition))
			{
				Repaint();
			}
		}

		protected override void OnDisable()
		{
			if (previewEditor != null)
			{
				DestroyImmediate(previewEditor);
				previewEditor = null;
			}

			base.OnDisable();
		}

		private static class Styles
		{
			public static readonly float previewHeight = 128;
		}
	}
}