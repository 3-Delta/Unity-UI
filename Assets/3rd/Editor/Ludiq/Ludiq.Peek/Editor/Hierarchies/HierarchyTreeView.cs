using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class HierarchyTreeView : TreeView
	{
		private Event e => Event.current;

		private static readonly ToolbarControlProvider toolbarControlProvider = new ToolbarControlProvider(ToolbarWindow.HierarchyPopup);

		public Item hoveredItem { get; private set; }
		
		public class Item : TreeViewItem
		{
			public GUIStyle style { get; set; }
		}

		public class GameObjectItem : Item
		{
			public GameObject gameObject { get; }

			public Texture2D overlay { get; }

			public bool canOpenPrefab { get; }

			public GameObjectItem(GameObject gameObject)
			{
				Ensure.That(nameof(gameObject)).IsNotNull(gameObject);
				this.gameObject = gameObject;
				id = gameObject.GetInstanceID();
				displayName = gameObject.name;
				icon = gameObject.gameObject.Icon()?[IconSize.Small];
				style = HierarchyStyles.Label(PrefabUtility.IsPartOfPrefabInstance(gameObject), PrefabUtility.IsPrefabAssetMissing(gameObject), !gameObject.activeInHierarchy);
				
				if (PrefabUtility.IsAddedGameObjectOverride(gameObject))
				{
					overlay = PeekPlugin.Icons.prefabOverlayAdded?[IconSize.Small];
				}

				canOpenPrefab = GameObjectOperations.CanOpenPrefab(gameObject);
			}
		}

		public class SceneItem : Item
		{
			public Scene scene { get; }

			public SceneItem(Scene scene)
			{
				Ensure.That(nameof(scene)).IsNotNull(scene);
				this.scene = scene;
				id = scene.handle;
				displayName = scene.name;
				icon = typeof(SceneAsset).Icon()?[IconSize.Small];
				style = HierarchyStyles.normalLabel;
			}
		}

		public class PrefabStageItem : Item
		{
			public PrefabStage prefabStage { get; }

			public PrefabStageItem(PrefabStage prefabStage)
			{
				Ensure.That(nameof(prefabStage)).IsNotNull(prefabStage);
				this.prefabStage = prefabStage;
				id = prefabStage.scene.handle;
				displayName = prefabStage.scene.name;
				icon = PeekPlugin.Icons.prefab?[IconSize.Small];
			}
		}

		public GameObjectItem FindGameObjectItem(int id)
		{
			return (GameObjectItem)FindItem(id, rootItem);
		}

		public GameObjectItem FindGameObjectItem(Transform transform)
		{
			return FindGameObjectItem(transform.GetInstanceID());
		}

		public SceneItem FindSceneItem(int id)
		{
			return (SceneItem)FindItem(id, rootItem);
		}

		public SceneItem FindSceneItem(Scene scene)
		{
			return FindSceneItem(scene.handle);
		}

		public GameObject[] targets { get; }

		public EditorWindow parentWindow { get; }

		public HierarchyTreeView(EditorWindow parentWindow, GameObject[] targets, TreeViewState state) : base(state)
		{
			this.targets = targets;
			this.parentWindow = parentWindow;
			this.useScrollView = true;
		}

		protected override TreeViewItem BuildRoot()
		{
			var root = new TreeViewItem(-1, -1, "Root");
			root.children = new List<TreeViewItem>();

			if (targets != null && targets.Length == 1)
			{
				var target = targets[0];

				if (target == null)
				{
					parentWindow.Close();

					return root;
				}

				// Build the minimal transform hierarchy
				var parents = new List<GameObject>();

				var currentParent = target;

				while (currentParent != null)
				{
					parents.Add(currentParent);
					currentParent = currentParent.transform.parent.AsUnityNull()?.gameObject;
				}

				parents.Reverse();

				var rootTransform = parents[0];
				var scene = rootTransform.gameObject.scene;
				var sceneItem = new SceneItem(scene);
				root.AddChild(sceneItem);
				Item currentParentItem = sceneItem;

				foreach (var parent in parents)
				{
					var parentItem = new GameObjectItem(parent);
					currentParentItem.AddChild(parentItem);
					currentParentItem = parentItem;
				}

				AddChildrenRecursive((GameObjectItem)currentParentItem);

				// AddSiblings((GameObjectItem)currentParentItem);
			}
			else
			{
				var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

				if (prefabStage != null)
				{
					// Build the prefab stage hierarchy

					var prefabRoot = prefabStage.prefabContentsRoot;
					var prefabRootItem = new GameObjectItem(prefabRoot);

					var stageItem = new PrefabStageItem(prefabStage);
					AddChildrenRecursive(prefabRootItem);
					stageItem.AddChild(prefabRootItem);
					root.AddChild(stageItem);
				}
				else
				{
					// Build the full scene hierarchy

					for (var i = 0; i < SceneManager.sceneCount; i++)
					{
						var scene = SceneManager.GetSceneAt(i);

						if (!scene.IsValid())
						{
							continue;
						}

						var sceneItem = new SceneItem(scene);

						if (scene.isLoaded)
						{
							foreach (var sceneRootGameObject in scene.GetRootGameObjects())
							{
								var sceneRootGameObjectItem = new GameObjectItem(sceneRootGameObject);

								AddChildrenRecursive(sceneRootGameObjectItem);

								sceneItem.AddChild(sceneRootGameObjectItem);
							}
						}

						root.AddChild(sceneItem);
					}
				}
			}

			// Override with search if needed
			if (hasSearch)
			{
				var searchRoot = new TreeViewItem(-1, -1, "Results");
				var searchResults = new List<TreeViewItem>();

				var treeViewItemStack = new Stack<TreeViewItem>();
				treeViewItemStack.Push(root);

				while (treeViewItemStack.Count > 0)
				{
					var treeViewItem = treeViewItemStack.Pop();

					if (treeViewItem.children != null)
					{
						foreach (var child in treeViewItem.children)
						{
							if (child != null)
							{
								if (DoesItemMatchSearch(child, searchString))
								{
									searchResults.Add(child);
								}

								treeViewItemStack.Push(child);
							}
						}
					}
				}

				foreach (var searchResult in searchResults)
				{
					searchResult.children = null;
				}

				searchResults.Sort((x, y) => SearchUtility.Relevance(searchString, x.displayName).CompareTo(SearchUtility.Relevance(searchString, y.displayName)));
				searchRoot.children = searchResults;
				root = searchRoot;
			}

			SetupDepthsFromParentsAndChildren(root);

			hoveredItem = null;

			return root;
		}
		
		public override void OnGUI(Rect rect)
		{
			base.OnGUI(rect);

			if (e.type == EventType.MouseLeaveWindow)
			{
				hoveredItem = null;
			}
		}

		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			if (item is SceneItem)
			{
				return 20;
			}

			return base.GetCustomRowHeight(row, item);
		}

		public void AutoExpand()
		{
			if (targets != null)
			{
				var targetIds = targets.Select(t => t.GetInstanceID()).ToArray();

				SetSelection(targetIds);

				foreach (var targetId in targetIds)
				{
					FrameItem(targetId);
				}

				if (targetIds.Length == 1)
				{
					SetExpanded(targetIds[0], true);
				}
			}
			else
			{
				foreach (var sceneItem in GetRows().OfType<SceneItem>())
				{
					SetExpanded(sceneItem.id, true);
				}

				foreach (var prefabStageItem in GetRows().OfType<PrefabStageItem>())
				{
					SetExpandedRecursive(prefabStageItem.id, true);
				}
			}
		}

		protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
		{
			return SearchUtility.Matches(search, item.displayName);
		}

		private void AddChildrenRecursive(GameObjectItem parentItem)
		{
			for (var i = 0; i < parentItem.gameObject.transform.childCount; i++)
			{
				var child = parentItem.gameObject.transform.GetChild(i);
				var childItem = new GameObjectItem(child.gameObject);
				parentItem.AddChild(childItem);
				AddChildrenRecursive(childItem);
			}
		}

		private void AddSiblings(GameObjectItem item)
		{
			var parent = item.gameObject.transform.parent;

			if (parent == null)
			{
				var rootObjects = item.gameObject.scene.GetRootGameObjects();

				foreach (var sceneRootObject in rootObjects)
				{
					var sibling = sceneRootObject;

					if (sibling != item.gameObject)
					{
						var siblingItem = new GameObjectItem(sibling);
						// siblingItem.dim = true;
						item.parent.AddChild(siblingItem);
						AddChildrenRecursive(siblingItem);
					}
				}
			}
			else
			{
				for (var i = 0; i < parent.childCount; i++)
				{
					var sibling = parent.GetChild(i).gameObject;

					if (sibling != item.gameObject)
					{
						var siblingItem = new GameObjectItem(sibling);
						// siblingItem.dim = true;
						item.parent.AddChild(siblingItem);
						AddChildrenRecursive(siblingItem);
					}
				}
			}
		}

		protected override void DoubleClickedItem(int id)
		{
			SelectItem(FindItem(id, rootItem));
		}

		private void SelectItem(TreeViewItem item)
		{
			if (item is GameObjectItem gameObjectItem)
			{
				if (Selection.activeTransform?.gameObject != gameObjectItem.gameObject)
				{
					Selection.activeTransform = gameObjectItem.gameObject.transform;
					parentWindow?.Close();
				}

				switch (PeekPlugin.Configuration.hierarchyFraming)
				{
					case HierarchyFramingOption.Always: SceneView.lastActiveSceneView?.FrameSelected(); break;
					case HierarchyFramingOption.WhenOutOfView: SceneView.lastActiveSceneView?.FrameSelectedIfOutOfView(); break;
				}

				GUIUtility.ExitGUI();
			}
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = (Item)args.item;
			
			if (args.selected && e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Space))
			{
				SelectItem(item);
				return;
			}
			
			var rowPosition = args.rowRect;

			var indent = ((args.item.depth + 1) * 14) + 1;

			var contentPosition = new Rect
			(
				indent,
				rowPosition.yMin,
				rowPosition.width - indent,
				rowPosition.height
			);

			var isHovered = e.type != EventType.Layout && rowPosition.Contains(e.mousePosition);
			var isSelected = args.selected;

			if (isHovered)
			{
				hoveredItem = item;
			}

			if (isHovered && !isSelected)
			{
				EditorGUI.DrawRect(rowPosition, ColorPalette.unityBackgroundDark);
			}

			if (item is GameObjectItem gameObjectItem)
			{
				GameObjectRowContentGUI(rowPosition, contentPosition, gameObjectItem, args);
			}
			else if (item is SceneItem sceneItem)
			{
				SceneItemRowContentGUI(rowPosition, contentPosition, sceneItem, args);
			}
			else
			{
				base.RowGUI(args);
			}
		}

		public IEnumerable<GameObject> GetActiveGameObjects()
		{
			var selection = GetSelection();
			var rows = GetRows();

			if (selection.Count > 0)
			{
				foreach (var id in selection)
				{
					var item = FindItem(id, rootItem);

					if (item is GameObjectItem gameObjectItem)
					{
						yield return gameObjectItem.gameObject;
					}
				}
			}
			/*else if (hoveredItem != null && (hoveredItem is GameObjectItem gameObjectItem))
			{
				yield return gameObjectItem.gameObject;
			}*/
			else if (rows.Count > 0)
			{
				var item = rows[0];

				if (item is GameObjectItem gameObjectItem2)
				{
					yield return gameObjectItem2.gameObject;
				}
			}
		}

		public bool SelectActive()
		{
			var activeIds = GetActiveGameObjects().Select(go => go.GetInstanceID()).ToArray();

			if (activeIds.Length == 0)
			{
				return false;
			}

			Selection.instanceIDs = activeIds;

			switch (PeekPlugin.Configuration.hierarchyFraming)
			{
				case HierarchyFramingOption.Always: SceneView.lastActiveSceneView?.FrameSelected(); break;
				case HierarchyFramingOption.WhenOutOfView: SceneView.lastActiveSceneView?.FrameSelectedIfOutOfView(); break;
			}

			return true;
		}

		private int GetSelectionInstanceID(int id)
		{
			var item = FindItem(id, rootItem);

			if (item is GameObjectItem gameObjectItem)
			{
				return gameObjectItem.gameObject.GetInstanceID();
			}
			else if (item is SceneItem sceneItem)
			{
				return sceneItem.scene.handle;
			}
			else if (item is PrefabStageItem prefabStageItem)
			{
				return prefabStageItem.prefabStage.scene.handle;
			}
			else
			{
				throw new InvalidImplementationException();
			}
		}

		protected override bool CanRename(TreeViewItem item)
		{
			return item is GameObjectItem;
		}

		protected override void RenameEnded(RenameEndedArgs args)
		{
			base.RenameEnded(args);

			var item = FindGameObjectItem(args.itemID);

			var go = item.gameObject;
			Undo.RecordObject(go, $"Rename {args.originalName} to {args.newName}");
			go.name = args.newName;
		}

		private Rect GetItemRect(int id)
		{
			var item = FindItem(id, rootItem);
			var row = GetRows().IndexOf(item);
			return GetRowRect(row);
		}

		protected override void ContextClickedItem(int id)
		{
			var item = FindItem(id, rootItem);

			if (item is GameObjectItem gameObjectItem)
			{
				var selection = selectedGameObjects;

				GenericMenu.MenuFunction rename = null;

				if (selection.Length == 1)
				{
					rename = () => BeginRename(gameObjectItem);
				}

				var activatorPosition = LudiqGUIUtility.GUIToScreenRect(GetItemRect(id));

				var menu = new GenericMenu();
				menu.allowDuplicateNames = true;
				GameObjectContextMenu.Fill(selection, menu, activatorPosition, rename);
				menu.ShowAsContext();
			}
		}

		private GameObject[] selectedGameObjects
		{
			get { return GetSelection().Select(_id => FindItem(_id, rootItem)).OfType<GameObjectItem>().Select(ti => ti.gameObject).ToArray(); }
			set
			{
				var ids = new List<int>();

				foreach (var go in value)
				{
					var transform = go.transform;
					var id = transform.GetInstanceID();

					if (FindItem(id, rootItem) != null)
					{
						ids.Add(id);
					}
				}

				SetSelection(ids, TreeViewSelectionOptions.RevealAndFrame);
			}
		}

		protected override void CommandEventHandling()
		{
			base.CommandEventHandling();

			if (e.type != EventType.ExecuteCommand && e.type != EventType.ValidateCommand)
			{
				return;
			}

			var execute = e.type == EventType.ExecuteCommand;

			if (e.commandName == "Delete" || e.commandName == "SoftDelete")
			{
				if (execute)
				{
					GameObjectOperations.Delete(selectedGameObjects);
				}

				e.Use();
				GUIUtility.ExitGUI();
			}
			else if (e.commandName == "Duplicate")
			{
				if (execute)
				{
					GameObjectOperations.Duplicate(selectedGameObjects);
					Reload();
					selectedGameObjects = Selection.gameObjects;
				}

				e.Use();
				GUIUtility.ExitGUI();
			}
			else if (e.commandName == "Copy")
			{
				if (execute)
				{
					GameObjectOperations.Copy(selectedGameObjects);
				}

				e.Use();
				GUIUtility.ExitGUI();
			}
			else if (e.commandName == "Paste")
			{
				if (execute)
				{
					GameObjectOperations.Paste(selectedGameObjects);
				}

				e.Use();
				GUIUtility.ExitGUI();
			}
		}
		
		private void GameObjectRowContentGUI(Rect rowPosition, Rect contentPosition, GameObjectItem item, RowGUIArgs args)
		{
			var openPrefabButtonPosition = new Rect
			(
				rowPosition.xMax - HierarchyStyles.openPrefabButton.fixedWidth - HierarchyStyles.openPrefabButton.margin.right,
				rowPosition.y,
				HierarchyStyles.openPrefabButton.fixedWidth,
				HierarchyStyles.openPrefabButton.fixedHeight
			);

			if (item.canOpenPrefab)
			{
				if (GUI.Button(openPrefabButtonPosition, GUIContent.none, HierarchyStyles.openPrefabButton))
				{
					GameObjectOperations.OpenPrefab(item.gameObject);
					parentWindow?.Close();
					GUIUtility.ExitGUI();
					return;
				}
			}

			contentPosition.xMax = openPrefabButtonPosition.xMin;
			
			var iconPosition = new Rect
			(
				contentPosition.x,
				contentPosition.y,
				IconSize.Small,
				IconSize.Small
			);
			
			if (PeekPlugin.Configuration.enablePreviewIcons && PreviewUtility.TryGetPreview(item.gameObject, out var preview) && preview != null)
			{
				GUI.DrawTexture(iconPosition, preview);
			}
			else if (item.icon != null)
			{
				GUI.DrawTexture(iconPosition, item.icon);
			}

			if (item.overlay != null)
			{
				GUI.DrawTexture(iconPosition, item.overlay);
			}

			var labelPosition = contentPosition;
			labelPosition.xMin += iconPosition.width;
			labelPosition.xMin += 2;

			var label = item.displayName;

			if (hasSearch)
			{
				label = SearchUtility.HighlightQuery(label, searchString);
			}
			
			if (e.type == EventType.Repaint)
			{
				item.style.Draw(labelPosition, label, false, false, args.selected, args.focused);
			}

			var target = item.gameObject;

			if (target == null)
			{
				return;
			}
				
			Object[] targets;

			if (args.selected)
			{
				targets = GetSelection().Where(i => FindItem(i, rootItem) is GameObjectItem).Select(i => FindGameObjectItem(i).gameObject).ToArray();
			}
			else
			{
				targets = new[] {target.gameObject};
			}

			TreeViewToolbars.Draw(false, toolbarControlProvider, target, targets, args.selected, args.label, contentPosition, rowPosition, HasFocus());
		}

		private void SceneItemRowContentGUI(Rect rowPosition, Rect contentPosition, SceneItem item, RowGUIArgs args)
		{
			contentPosition.yMin++;
			
			// Lots of messy code to display the row like Unity does,
			// because we don't have access to the same DrawItemBackground
			// override as they do
			var isHovered = rowPosition.Contains(e.mousePosition);
			var isSelected = args.selected;

			var backgroundColor = GUI.backgroundColor;

			if (isHovered && !isSelected)
			{
				GUI.backgroundColor = ColorUtility.Gray(0.75f);
			}

			using (LudiqGUI.color.Override(new Color(1, 1, 1, 0.9f)))
			{
				GUI.Label(rowPosition, GUIContent.none, HierarchyStyles.sceneHeader);
			}

			GUI.backgroundColor = backgroundColor;

			if (isSelected)
			{
				var selectionHighlightPosition = rowPosition;
				selectionHighlightPosition.height--;
				EditorGUI.DrawRect(selectionHighlightPosition, ColorPalette.unitySelectionHighlight);
			}
			
			var iconPosition = new Rect
			(
				contentPosition.x,
				contentPosition.y,
				IconSize.Small,
				IconSize.Small
			);
			
			if (item.icon != null)
			{
				GUI.DrawTexture(iconPosition, item.icon);
			}
			
			contentPosition.xMin += iconPosition.width;
			contentPosition.xMin += 2;

			var label = item.displayName;

			if (hasSearch)
			{
				label = SearchUtility.HighlightQuery(label, searchString);
			}

			var style = item.style;
			
			if (SceneManager.GetActiveScene() == item.scene)
			{
				style = style.BoldedStyle(true);
			}
			
			if (e.type == EventType.Repaint)
			{
				style.Draw(contentPosition, label, false, false, args.selected, args.focused);
			}
		}
	}
}