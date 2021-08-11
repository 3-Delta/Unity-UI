using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class FuzzyWindow : EditorWindow
	{
		public void Populate(FuzzyOptionNode node, IEnumerable<IFuzzyOption> children, CancellationToken? cancellation = null)
		{
			if (node.isPopulated)
			{
				return;
			}
			
			if (node.option.mode == FuzzyOptionMode.Branch)
			{
				var i = 0;
					
				var _children = children.ToArray();

				lock (guiLock)
				{
					if (node.hasChildren == null)
					{
						node.hasChildren = _children.Length > 0;
					}
				}

				var childNodes = new List<FuzzyOptionNode>();
				
				foreach (var child in _children)
				{
					if (child == null)
					{
						continue;
					}

					try
					{
						child.OnPopulate();
					}
					catch (Exception ex)
					{
						Debug.LogWarning($"Failed to display {child.GetType()}: \n{ex}");
						continue;
					}

					string label;
					
					if (node.option is SearchOption)
					{
						label = tree.SearchResultLabel(child, query);
					}
					else if (node == favoritesRoot || tree.UseExplicitLabel(node.option, child))
					{
						label = tree.ExplicitLabel(child) ?? child.label;
					}
					else
					{
						label = child.label;
					}

					FuzzyOptionNode childNode;

					try
					{
						childNode = new FuzzyOptionNode(child, label);
					}
					catch (Exception ex)
					{
						Debug.LogWarning($"Failed to create option node for {child.GetType()} (value = {child.value}): \n{ex}");
						continue;
					}

					DisplayProgressBar($"{child.label}... ({++i} / {_children.Length})", (float) i / _children.Length);

					var evaluateBranchesEarly = false;

					if (child.mode == FuzzyOptionMode.Leaf)
					{
						childNode.hasChildren = false;
						childNodes.Add(childNode);
					}
					else if (child.mode == FuzzyOptionMode.Branch)
					{
						if (evaluateBranchesEarly)
						{
							if (childNode.EvaluateHasChildren(tree))
							{
								childNodes.Add(childNode);
							}
						}
						else
						{
							childNodes.Add(childNode);
						}
					}

					cancellation?.ThrowIfCancellationRequested();
				}

				lock (guiLock)
				{
					node.children.AddRange(childNodes);
				}
			}

			lock (guiLock)
			{
				node.isPopulated = true;
			}
		}

		private static Event e => Event.current;

		public static class Styles
		{
			static Styles()
			{
				var headerFontSize = new GUIStyle("In BigTitle").fontSize;

				headerBackground = new GUIStyle(EditorStyles.toolbar)
				{
					font = EditorStyles.boldLabel.font,
					fontSize = headerFontSize,
					fixedHeight = headerHeight,
					alignment = TextAnchor.MiddleCenter,
					margin = new RectOffset(1, 1, 0, 0),
				};
				headerBreadcrumbRoot = new GUIStyle(LudiqStyles.toolbarBreadcrumbRoot)
				{
					font = EditorStyles.label.font,
					fontSize = headerFontSize,
					fixedHeight = headerHeight,
					alignment = TextAnchor.MiddleCenter,
				};
				headerBreadcrumbElement = new GUIStyle(LudiqStyles.toolbarBreadcrumb)
				{
					font = EditorStyles.label.font,
					fontSize = headerFontSize,
					fixedHeight = headerHeight,
					alignment = TextAnchor.MiddleCenter,
				};
				headerBreadcrumbRoot.padding.bottom += 2;
				headerBreadcrumbElement.padding.bottom += 2;
				headerBreadcrumbRootCurrent = new GUIStyle(headerBreadcrumbRoot);
				headerBreadcrumbElementCurrent = new GUIStyle(headerBreadcrumbElement);

				footerBackground = new GUIStyle("In BigTitle");

				optionBackground = new GUIStyle("PR Label");
				optionBackground.fixedHeight = 20f;
				optionBackground.padding.left = 2;
				optionBackground.padding.top = 2;

				optionForeground = new GUIStyle("PR Label");
				optionForeground.richText = true;
				optionForeground.alignment = TextAnchor.MiddleLeft;
				optionForeground.padding.left = 0;
				optionForeground.fixedHeight = 20f;

				separator = new GUIStyle("PR Label");
				separator.normal.textColor = ColorPalette.unityForegroundDim;
				separator.fontSize = 10;
				separator.padding = new RectOffset(4, 4, 6, 4);
				separator.alignment = TextAnchor.MiddleLeft;
				separator.fixedHeight = separatorHeight;

				separatorLine = ColorPalette.unityForegroundDim.CreateBackground();
				separatorLine.fixedHeight = 1;

				background = new GUIStyle("grey_border");

				rightArrow = new GUIStyle("AC RightArrow");

				leftArrow = new GUIStyle("AC LeftArrow");
				
				searchNotFound = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
				searchNotFound.wordWrap = true;
				searchNotFound.padding = new RectOffset(10, 10, 10, 10);
				searchNotFound.alignment = TextAnchor.MiddleCenter;

				check = new GUIStyle();
				var checkTexture = LudiqCore.Resources.LoadTexture("Fuzzy/Check.png", new TextureResolution[] { 12, 24 }, CreateTextureOptions.PixelPerfect);
				check.normal.background = checkTexture[12];
				check.normal.scaledBackgrounds = new[] { checkTexture[24] };
				check.fixedHeight = 12;
				check.fixedWidth = 12;

				searchIcon = LudiqCore.Resources.LoadIcon("Fuzzy/Search.png");

				star = new GUIStyle();
				var starOffTexture = LudiqCore.Resources.LoadIcon("Fuzzy/StarOff.png");
				var starOnTexture = LudiqCore.Resources.LoadIcon("Fuzzy/StarOn.png");
				star.normal.background = starOffTexture[16];
				star.normal.scaledBackgrounds = new[] { starOffTexture[32] };
				star.onNormal.background = starOnTexture[16];
				star.onNormal.scaledBackgrounds = new[] { starOnTexture[32] };
				star.fixedHeight = 16;
				star.fixedWidth = 16;
				favoritesIcon = starOnTexture;
			}

			public static readonly GUIStyle headerBackground;
			public static readonly GUIStyle headerBreadcrumbRoot;
			public static readonly GUIStyle headerBreadcrumbElement;
			public static readonly GUIStyle headerBreadcrumbRootCurrent;
			public static readonly GUIStyle headerBreadcrumbElementCurrent;
			public static readonly GUIStyle footerBackground;
			public static readonly GUIStyle optionBackground;
			public static readonly GUIStyle optionForeground;
			public static readonly GUIStyle separator;
			public static readonly GUIStyle separatorLine;
			public static readonly GUIStyle background;
			public static readonly GUIStyle rightArrow;
			public static readonly GUIStyle leftArrow;
			public static readonly GUIStyle searchNotFound;
			public static readonly GUIStyle check;
			public static readonly GUIStyle star;
			public static readonly EditorTexture searchIcon;
			public static readonly EditorTexture favoritesIcon;
			public static readonly float headerHeight = 20;
			public static readonly float optionHeight = 20;
			public static readonly float separatorHeight = 24;
			public static readonly float maxOptionWidth = 800;
			public static readonly float spaceBetweenIcons = 1;
			public static readonly float spaceAfterIcons = 3;

			public static void Initialize() { }
		}

		public class Root : FuzzyOption<object>
		{
			public Root(GUIContent header) : base(FuzzyOptionMode.Branch)
			{
				label = header?.text ?? "Fuzzy";
				icon = EditorTexture.Single(header?.image);
			}
		}

		public class SearchOption : FuzzyOption<object>
		{
			public SearchOption(string query) : base(FuzzyOptionMode.Branch)
			{
				this.query = query;
				label = string.Format(searchHeaderFormat, query);
			}

			public string query { get; private set; }

			public override EditorTexture Icon()
			{
				return Styles.searchIcon;
			}
		}

		public class FavoritesRoot : FuzzyOption<object>
		{
			public FavoritesRoot() : base(FuzzyOptionMode.Branch)
			{
				label = "Favorites";
			}

			public override EditorTexture Icon()
			{
				return Styles.favoritesIcon;
			}
		}

		#region Lifecycle

		private Action<IFuzzyOption> callback;

		public static FuzzyWindow instance { get; private set; }

		private IFuzzyOptionTree tree;

		public bool isValid => tree != null; // Will return false after reloading the window from serialization

		public static void Show(Rect activatorPosition, IFuzzyOptionTree optionTree, Action<IFuzzyOption> callback)
		{
			Ensure.That(nameof(optionTree)).IsNotNull(optionTree);

			// Makes sure control exits DelayedTextFields before opening the window
			GUIUtility.keyboardControl = 0;

			if (instance == null)
			{
				instance = CreateInstance<FuzzyWindow>();
			}
			else
			{
				throw new InvalidOperationException("Cannot open two instances of the fuzzy window at once.");
			}

			instance.Initialize(activatorPosition, optionTree, callback);
		}

		private void OnEnable()
		{
			instance = this;
			query = string.Empty;
			EditorApplicationUtility.LockReloadAssemblies();
		}

		private void OnDisable()
		{
			if (transparentBackground != null)
			{
				DestroyImmediate(transparentBackground);
				transparentBackground = null;
			}

			if (isValid && activeNode != null)
			{
				activeNode.option.OnFocusLeave(activeNode);
			}

			instance = null;
			EditorApplicationUtility.UnlockReloadAssemblies();
		}

		private void Update()
		{
			if (!isValid)
			{
				Close();
				return;
			}

			if (activeParent.isPopulated)
			{
				if (activeParent.isLoading && (animTarget == 0 || prevAnim == 1))
				{
					activeParent.isLoading = false;
				}
			}
			else
			{
				activeParent.isLoading = true;
			}

			if (requireRepaint)
			{
				Repaint();
				requireRepaint = false;
			}
		}

		private bool requireRepaint;

		private void Initialize(Rect activatorPosition, IFuzzyOptionTree optionTree, Action<IFuzzyOption> callback)
		{
			tree = optionTree;

			// Activator position is assumed to be in screen space, not GUI space
			// This lets us properly position fuzzy windows from context menus where no GUI context is available
			
			this.activatorPosition = activatorPosition;

			// Create the hierarchy

			stack = new List<FuzzyOptionNode>();
			root = new FuzzyOptionNode(new Root(optionTree.header));
			favoritesRoot = new FuzzyOptionNode(new FavoritesRoot());
			stack.Add(root);

			Styles.Initialize();
			
			ExecuteTask(() =>
			{
				if (!optionTree.prewarmed)
				{
					optionTree.Prewarm();
					optionTree.prewarmed = true;
				}
				else
				{
					optionTree.Rewarm();
				}

				Populate(root, optionTree.Root());

				UpdateFavorites();

				// Fit height to children if there is no depth and no search

				var hasSubChildren = root.children.Any(option => option.hasChildren != false);

				if (!optionTree.searchable && !hasSubChildren)
				{
					var height = 0f;

					if (!string.IsNullOrEmpty(root.option.headerLabel))
					{
						height += Styles.headerHeight;
					}

					foreach (var child in root.children)
					{
						if (child.option is FuzzySeparator)
						{
							height += Styles.separatorHeight;
						}
						else
						{
							height += Styles.optionHeight;
						}
					}

					this.height = height + 1;
				}
			});


			// Setup the search

			Search();

			// Assign the callback

			this.callback = callback;

			// Show and focus the window
			
			wantsMouseMove = true;
			var initialSize = new Vector2(activatorPosition.width, height);
			this.ShowAsDropDown(activatorPosition, initialSize);
			Focus();
		}

		#endregion

		#region Hierarchy

		private FuzzyOptionNode root;
		private List<FuzzyOptionNode> stack;

		private FuzzyOptionNode activeParent => stack[stack.Count - 1];

		private int activeSelectedIndex
		{
			get
			{
				return activeParent.selectedIndex;
			}
			set
			{
				lock (guiLock)
				{
					activeParent.selectedIndex = value;
				}
			}
		}

		private IList<FuzzyOptionNode> activeNodes => activeParent.children;

		private FuzzyOptionNode activeNode
		{
			get
			{
				if (activeSelectedIndex >= 0 && activeSelectedIndex < activeNodes.Count)
				{
					return activeNodes[activeSelectedIndex];
				}
				else
				{
					return null;
				}
			}
		}

		private void SelectParent()
		{
			if (stack.Count > 1)
			{
				SelectAncestor(stack[stack.Count - 2]);
			}
		}

		private void SelectAncestor(FuzzyOptionNode node)
		{
			if (stack.Contains(node))
			{
				animTarget = 0;
				animAncestor = node;
				lastRepaintTime = DateTime.UtcNow;

				query = node.option is SearchOption searchOption ? searchOption.query : string.Empty;
				delayedQuery = null;
				GUIUtility.keyboardControl = 0;
				letQueryClear = true;
			}
		}

		private void EnterChild(FuzzyOptionNode node)
		{
			if (node == null || node.hasChildren == false || node.option is FuzzySeparator)
			{
				return;
			}

			ExecuteTask(() =>
			{
				Populate(node, tree.Children(node.option));
			});

			lastRepaintTime = DateTime.UtcNow;

			query = string.Empty;
			delayedQuery = null;
			GUIUtility.keyboardControl = 0;
			letQueryClear = true;

			if (animTarget == 0)
			{
				animTarget = 1;
				animAncestor = null;
			}
			else if (anim == 1)
			{
				anim = 0;
				prevAnim = 0;
				stack.Add(node);
			}
		}

		private void SelectChild(FuzzyOptionNode node)
		{
			if (node == null || node.option is FuzzySeparator)
			{
				return;
			}

			if (node.option.mode == FuzzyOptionMode.Branch)
			{
				if (node.EvaluateHasChildren(tree))
				{
					EnterChild(node);
				}
			}
			else if (node.option.mode == FuzzyOptionMode.Leaf)
			{
				if (node == activeNode)
				{
					node.option.OnFocusLeave(node);
				}

				callback?.Invoke(node.option);
			}
		}

		#endregion

		#region Search

		private string query;
		private string delayedQuery;
		private bool letQueryClear;
		private string searchFieldName = "FuzzySearch";
		private static string searchHeaderFormat = "\"{0}\"";
		private static string searchNotFoundFormat = "No results found for \"{0}\".";
		private CancellationTokenSource searchCancellationTokenSource;

		private bool hasSearch => !string.IsNullOrEmpty(query);

		private void Search()
		{
			while (stack.Count > 0 && stack[stack.Count - 1].option is SearchOption)
			{
				stack.RemoveAt(stack.Count - 1);
			}

			if (hasSearch)
			{
				searchCancellationTokenSource?.Cancel();

				var query = this.query;

				var parent = (stack.Count > 1 ? stack[stack.Count - 1] : null);
				var searchNode = new FuzzyOptionNode(new SearchOption(query));

				searchCancellationTokenSource = new CancellationTokenSource();
				var searchCancellationToken = searchCancellationTokenSource.Token;

				ExecuteTask(() =>
				{
					DisplayProgressBar($"Searching for \"{query}\"...", 0);
						
					if (searchCancellationToken.IsCancellationRequested)
					{
						return;
					}

					lock (guiLock)
					{
						searchNode.children.Clear();
						searchNode.isPopulated = false;
					}

					Populate(searchNode, tree.OrderedSearchResults(query, parent?.option, searchCancellationToken).Take(LudiqCore.Configuration.maxSearchResults));
					activeSelectedIndex = activeNodes.Count >= 1 ? 0 : -1;
				});				

				stack.Add(searchNode);
			}
			else
			{				
				animTarget = 1;
				animAncestor = null;
				lastRepaintTime = DateTime.UtcNow;
			}
		}

		#endregion

		#region Favorites

		private FuzzyOptionNode favoritesRoot;

		private void UpdateFavorites()
		{
			if (tree.favorites != null)
			{
				DisplayProgressBar("Fetching favorites...", 0);
				favoritesRoot.children.Clear();
				favoritesRoot.hasChildren = null;
				favoritesRoot.isPopulated = false;
				// Adding a where clause in case a favorited item was later changed to be unfavoritable.
				Populate(favoritesRoot, tree.favorites.Where(favorite => tree.CanFavorite(favorite)));
			}
			else
			{
				favoritesRoot.children.Clear();
				favoritesRoot.isPopulated = true;
			}

			root.children.Remove(favoritesRoot);

			if (favoritesRoot.EvaluateHasChildren(tree))
			{
				root.children.Insert(0, favoritesRoot);
			}
		}

		#endregion

		#region Animation

		private float anim = 1;
		private float prevAnim = 1;
		private int animTarget = 1;
		private FuzzyOptionNode animAncestor = null;
		private float animationSpeed = 4;

		private DateTime lastRepaintTime;

		public float repaintDeltaTime => (float)(DateTime.UtcNow - lastRepaintTime).TotalSeconds;

		private bool isAnimating => anim != animTarget;

		#endregion

		#region Positioning

		private float maxHeight = 320;
		private float height = 320;
		private float minWidth = 200;
		private float minOptionWidth;
		private float headerWidth;
		private float footerHeight;
		private Rect activatorPosition;
		private bool scrollToSelected;
		private float initialY;
		private bool initialYSet;
		private float totalWidth;
		private float totalHeight;

		private void OnPositioning()
		{
			lock (guiLock)
			{
				if (!initialYSet)
				{
					initialY = this.position.y;
					initialYSet = true;
				}

				var totalWidth = Mathf.Max(minWidth, activatorPosition.width, minOptionWidth + 36, headerWidth + 36);

				var totalHeight = Mathf.Min(height, maxHeight);

				var position = this.GetDropdownPosition(activatorPosition, new Vector2(totalWidth, totalHeight));
				
				position.y = initialY;

				if (!isAnimating && !activeParent.isLoading && activeNode?.option != null && activeNode.option.hasFooter)
				{
					footerHeight = activeNode.option.GetFooterHeight(activeNode, totalWidth);

					position.height += footerHeight;
				}

				position = LudiqGUIUtility.CropDropdownPosition(position);
				minSize = maxSize = position.size;
				this.position = position;
			}
		}

		#endregion

		#region GUI

		private FuzzyOptionNode previousActiveNode;

		private Texture2D transparentBackground;

		public float alpha { get; set; } = 1;

		private void ReadTransparentBackground()
		{
			if (transparentBackground != null)
			{
				DestroyImmediate(transparentBackground);
				transparentBackground = null;
			}

			transparentBackground = new Texture2D((int)position.width, (int)position.height);
			var pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(position.position, transparentBackground.width, transparentBackground.height);
			transparentBackground.SetPixels(pixels);
			transparentBackground.Apply();
		}

		private void OnGUI()
		{
			if (!isValid)
			{
				Close();
				return;
			}

			try
			{
				lock (guiLock)
				{
					if (alpha < 1 && e.type == EventType.Repaint)
					{
						ReadTransparentBackground();
						var innerPosition = new Rect(0, 0, position.width, position.height);
						GUI.DrawTexture(innerPosition, transparentBackground);
						GUI.DrawTexture(innerPosition, ColorPalette.unityBackgroundDark.WithAlpha(alpha).GetPixel());
					}

					LudiqGUI.color.BeginOverride(Color.white.WithAlpha(alpha));

					GUI.Label(new Rect(0, 0, position.width, position.height), GUIContent.none, Styles.background);

					HandleKeyboard();

					if (tree.searchable)
					{
						LudiqGUI.Space(7);

						if (letQueryClear)
						{
							letQueryClear = false;
						}
						else
						{
							EditorGUI.FocusTextInControl(searchFieldName);
						}

						var searchFieldPosition = GUILayoutUtility.GetRect(10, LudiqStyles.searchFieldInnerHeight);
						searchFieldPosition.x += 8;
						searchFieldPosition.width -= 16;

						var newQuery = OnSearchGUI(searchFieldPosition, delayedQuery ?? query);

						if (newQuery != query || delayedQuery != null)
						{
							if (!isAnimating)
							{
								query = delayedQuery ?? newQuery;
								Search();
								delayedQuery = null;
							}
							else
							{
								delayedQuery = newQuery;
							}
						}

						LudiqGUI.Space(5);
					}

					OnLevelGUI(anim);

					prevAnim = anim;

					if (isAnimating && e.type == EventType.Repaint)
					{
						anim = Mathf.MoveTowards(anim, animTarget, repaintDeltaTime * animationSpeed);

						if (animTarget == 0 && anim == 0 && animAncestor != null)
						{
							while (stack.Count > 1 && stack[stack.Count - 1] != animAncestor)
							{
								stack.RemoveAt(stack.Count - 1);
							}

							anim = 1;
							prevAnim = 1;
							animTarget = 1;
							animAncestor = null;
						}

						Repaint();
					}

					if (e.type == EventType.Repaint)
					{
						lastRepaintTime = DateTime.UtcNow;
					}

					if (!activeParent.isLoading)
					{
						if (tree.searchable && hasSearch && activeParent.hasChildren == false)
						{
							var searchNotFoundLabel = new GUIContent(string.Format(searchNotFoundFormat, query));

							minOptionWidth = Styles.searchNotFound.CalcSize(searchNotFoundLabel).x;

							EditorGUI.LabelField
							(
								GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)),
								searchNotFoundLabel,
								Styles.searchNotFound
							);
						}

						if (activeNode != null && activeNode.option.hasFooter)
						{
							OnFooterGUI();
						}
					}

					LudiqGUI.color.EndOverride();

					if (previousActiveNode != activeNode)
					{
						previousActiveNode?.option.OnFocusLeave(previousActiveNode);
						activeNode?.option?.OnFocusEnter(activeNode);
					}

					previousActiveNode = activeNode;

					OnPositioning();
				}
			}
			catch (ArgumentException ex)
			{
				if (tree.multithreaded && ex.Message.StartsWith("Getting control "))
				{
					// A bunch of happens that might affect the GUI could happen on a 
					// secondary thread, leading to Unity complaining about the amount
					// of controls changing between the draw call and the layout call.
					// Because these are hamless and last just one frame, we can safely
					// ignore them and repaint right away.
					requireRepaint = true;
				}
				else
				{
					throw;
				}
			}

			if (requireRepaint)
			{
				Repaint();
				requireRepaint = false;
			}
		}

		private string OnSearchGUI(Rect position, string query)
		{
			var fieldPosition = position;
			fieldPosition.width -= 15;

			var cancelButtonPosition = position;
			cancelButtonPosition.x += position.width - 15;
			cancelButtonPosition.width = 15;
			
			GUI.SetNextControlName(searchFieldName);
			query = EditorGUI.TextField(fieldPosition, query, LudiqStyles.searchField);

			if (GUI.Button(cancelButtonPosition, GUIContent.none, string.IsNullOrEmpty(query) ? LudiqStyles.searchFieldCancelButtonEmpty : LudiqStyles.searchFieldCancelButton) && query != string.Empty)
			{
				query = string.Empty;
				GUIUtility.keyboardControl = 0;
				letQueryClear = true;
			}

			return query;
		}

		private void OnHeaderGUI()
		{	
			if (e.type == EventType.Layout || e.type == EventType.Repaint)
			{
				headerWidth = 0;
			}

			var previousIconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

			LudiqGUI.BeginHorizontal(Styles.headerBackground);

			foreach (var node in stack)
			{
				node.EnsureDrawable();

				var content = new GUIContent(node.option.headerLabel, node.option.showHeaderIcon ? node.icon?[IconSize.Small] : null);
				var isCurrent = node == stack[stack.Count - 1];

				var style = node == stack[0] ? Styles.headerBreadcrumbRoot : Styles.headerBreadcrumbElement;
				if (isCurrent)
				{
					style = node == stack[0] ? Styles.headerBreadcrumbRootCurrent : Styles.headerBreadcrumbElementCurrent;
				}
				var width = style.CalcSize(content).x;

				if (GUILayout.Toggle(isCurrent, content, style) && !isCurrent)
				{
					SelectAncestor(node);
				}

				if (e.type == EventType.Layout || e.type == EventType.Repaint)
				{
					headerWidth += width;
				}
			}

			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndHorizontal();

			EditorGUIUtility.SetIconSize(previousIconSize);
		}

		private void OnLevelGUI(float anim)
		{
			var parent = stack[stack.Count - 1];

			anim = Mathf.Floor(anim) + Mathf.SmoothStep(0, 1, Mathf.Repeat(anim, 1));

			if (stack.Count > 1 || !string.IsNullOrEmpty(parent.option.headerLabel))
			{
				OnHeaderGUI();
			}

			if (e.type == EventType.Layout || e.type == EventType.Repaint)
			{
				minOptionWidth = 0;
			}

			OnOptionsGUI(anim, parent);

			if (anim < 1)
			{
				FuzzyOptionNode grandparent = null;

				if (animTarget == 0 && animAncestor != null)
				{
					grandparent = animAncestor;
				}
				else if (stack.Count > 1)
				{
					grandparent = stack[stack.Count - 2];
				}

				OnOptionsGUI(anim + 1, grandparent);
			}
		}

		private Vector2 lastMouseMovePosition;

		private void OnOptionsGUI(float anim, FuzzyOptionNode parent)
		{
			var hasHeader = !string.IsNullOrEmpty(parent.option.headerLabel);
			var headerHeight = hasHeader ? Styles.headerHeight : 0;
			var searchFieldHeight = tree.searchable ? LudiqStyles.searchFieldOuterHeight : 0;
			
			var levelPosition = new Rect
			(
				position.width * (1 - anim) + 1,
				searchFieldHeight + headerHeight,
				position.width - 2,
				height - (searchFieldHeight + 1) - headerHeight
			);

			if (e.type == EventType.MouseDown && e.button == (int)MouseButton.Right && levelPosition.Contains(e.mousePosition))
			{
				SelectParent();
				e.Use();
			}

			GUILayout.BeginArea(levelPosition);

			if (parent.isLoading)
			{
				LudiqGUI.BeginVertical();
				LudiqGUI.FlexibleSpace();

				LudiqGUI.BeginHorizontal();
				LudiqGUI.FlexibleSpace();
				LudiqGUI.LoaderLayout();
				LudiqGUI.FlexibleSpace();
				LudiqGUI.EndHorizontal();

				LudiqGUI.Space(16);

				LudiqGUI.BeginHorizontal();
				LudiqGUI.Space(10);
				var progressBarPosition = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(19), GUILayout.ExpandWidth(true));
				if (showProgress)
				{
					EditorGUI.ProgressBar(progressBarPosition, progress, progressText);
				}

				LudiqGUI.Space(10);
				LudiqGUI.EndHorizontal();
				LudiqGUI.Space(LudiqGUI.loaderSize * 1.5f);

				LudiqGUI.FlexibleSpace();
				LudiqGUI.EndVertical();
				Repaint();
			}
			else
			{

				parent.scroll = GUILayout.BeginScrollView(parent.scroll);

				EditorGUIUtility.SetIconSize(new Vector2(IconSize.Small, IconSize.Small));

				var selectedOptionPosition = default(Rect);

				foreach (var node in parent.children)
				{
					node.EnsurePositionable();

					if (e.type == EventType.Layout || e.type == EventType.Repaint)
					{
						minOptionWidth = Mathf.Max(minOptionWidth, Mathf.Min(node.width, Styles.maxOptionWidth));
					}
				}

				for (var i = 0; i < parent.children.Count; i++)
				{
					var node = parent.children[i];
					var isSeparator = node.option is FuzzySeparator;

					var optionPosition = GUILayoutUtility.GetRect(IconSize.Small, isSeparator ? Styles.separator.fixedHeight : Styles.optionHeight, GUILayout.ExpandWidth(true));

					if (((e.type == EventType.MouseMove && GUIUtility.GUIToScreenPoint(e.mousePosition) != lastMouseMovePosition) || e.type == EventType.MouseDown) &&
						parent.selectedIndex != i &&
						optionPosition.Contains(e.mousePosition))
					{
						parent.selectedIndex = i;
						Repaint();
						lastMouseMovePosition = GUIUtility.GUIToScreenPoint(e.mousePosition);
					}

					var optionIsSelected = false;

					if (i == parent.selectedIndex)
					{
						optionIsSelected = true;
						selectedOptionPosition = optionPosition;
					}

					// Clipping
					if (optionPosition.yMax < parent.scroll.y || optionPosition.yMin > parent.scroll.y + levelPosition.height)
					{
						continue;
					}

					EditorGUI.BeginDisabledGroup(node.option.mode == FuzzyOptionMode.Branch && node.hasChildren == false);

					node.EnsureDrawable();

					if (e.type == EventType.Repaint)
					{
						using (LudiqGUI.color.Override(node.dim ? LudiqGUI.color.value.WithAlphaMultiplied(0.66f) : LudiqGUI.color.value))
						{
							if (node.option is FuzzySeparator)
							{
								Styles.separator.Draw(optionPosition, node.label, false, false, false, false);

								var linePosition = new Rect(optionPosition.x, optionPosition.yMax - 1, optionPosition.width, 1);

								Styles.separatorLine.Draw(linePosition, GUIContent.none, false, false, false, false);
							}
							else
							{
								Styles.optionBackground.Draw(optionPosition, GUIContent.none, false, false, optionIsSelected, optionIsSelected);

								float x = Styles.optionBackground.padding.left;

								var spaced = false;

								foreach (var icon in node.icons)
								{
									if (icon == null)
									{
										continue;
									}

									var iconPosition = new Rect
									(
										optionPosition.x + x,
										optionPosition.y + Styles.optionBackground.padding.top,
										IconSize.Small,
										IconSize.Small
									);

									GUI.DrawTexture(iconPosition, icon[IconSize.Small]);

									x += IconSize.Small;
									x += Styles.spaceBetweenIcons;
									spaced = true;
								}

								if (spaced)
								{
									x -= Styles.spaceBetweenIcons;
									x += Styles.spaceAfterIcons;
								}

								var foregroundPosition = optionPosition;
								foregroundPosition.x += x;
								foregroundPosition.width -= x;

								Styles.optionForeground.Draw(foregroundPosition, node.label, false, false, optionIsSelected, optionIsSelected);
							}
						}
					}

					var right = optionPosition.xMax;

					if (node.option.mode == FuzzyOptionMode.Branch)
					{
						right -= 13;
						var rightArrowPosition = new Rect(right, optionPosition.y + 4, 13, 13);

						if (e.type == EventType.Repaint)
						{
							Styles.rightArrow.Draw(rightArrowPosition, false, false, false, false);
						}
					}

					if (node.option.mode == FuzzyOptionMode.Leaf && tree.selected.Contains(node.option.value) && !isSeparator)
					{
						right -= 16;
						var checkPosition = new Rect(right, optionPosition.y + 4, 12, 12);

						if (e.type == EventType.Repaint)
						{
							Styles.check.Draw(checkPosition, false, false, false, false);
						}
					}

					EditorGUI.EndDisabledGroup();

					if (tree.favorites != null && tree.CanFavorite(node.option) && (optionIsSelected || tree.favorites.Contains(node.option)))
					{
						right -= 19;
						var starPosition = new Rect(right, optionPosition.y + 2, IconSize.Small, IconSize.Small);

						EditorGUI.BeginChangeCheck();

						var isFavorite = tree.favorites.Contains(node.option);

						isFavorite = GUI.Toggle(starPosition, isFavorite, GUIContent.none, Styles.star);

						if (EditorGUI.EndChangeCheck())
						{
							if (isFavorite)
							{
								tree.favorites.Add(node.option);
							}
							else
							{
								tree.favorites.Remove(node.option);
							}

							tree.OnFavoritesChange();

							ExecuteTask(() => UpdateFavorites());							
						}
					}

					if (e.type == EventType.MouseUp && e.button == (int)MouseButton.Left && optionPosition.Contains(e.mousePosition))
					{
						e.Use();
						parent.selectedIndex = i;
						SelectChild(node);
					}
				}

				EditorGUIUtility.SetIconSize(default(Vector2));

				GUILayout.EndScrollView();

				if (scrollToSelected && e.type == EventType.Repaint)
				{
					scrollToSelected = false;

					var lastRect = GUILayoutUtility.GetLastRect();

					if (selectedOptionPosition.yMax - lastRect.height > parent.scroll.y)
					{
						var scroll = parent.scroll;
						scroll.y = selectedOptionPosition.yMax - lastRect.height;
						parent.scroll = scroll;
						Repaint();
					}

					if (selectedOptionPosition.y < parent.scroll.y)
					{
						var scroll = parent.scroll;
						scroll.y = selectedOptionPosition.y;
						parent.scroll = scroll;
						Repaint();
					}
				}
			}

			GUILayout.EndArea();
		}

		private void OnFooterGUI()
		{
			var footerPosition = new Rect
			(
				1,
				height - 1,
				position.width - 2,
				footerHeight
			);

			var backgroundPosition = footerPosition;
			backgroundPosition.height += 1;

			if (e.type == EventType.Repaint)
			{
				Styles.footerBackground.Draw(backgroundPosition, false, false, false, false);
			}

			activeNode.option.OnFooterGUI(activeNode, footerPosition);
		}

		private void HandleKeyboard()
		{
			if (e.type == EventType.KeyDown)
			{
				if (e.keyCode == KeyCode.DownArrow)
				{
					activeSelectedIndex = Mathf.Clamp(activeSelectedIndex + 1, 0, activeNodes.Count - 1);
					scrollToSelected = true;
					e.Use();
				}
				else if (e.keyCode == KeyCode.UpArrow)
				{
					activeSelectedIndex = Mathf.Clamp(activeSelectedIndex - 1, 0, activeNodes.Count);
					scrollToSelected = true;
					e.Use();
				}
				else if ((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) && !activeParent.isLoading)
				{
					SelectChild(activeNode);
					e.Use();
				}
				else if ((e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.Backspace) && !hasSearch && activeParent != root)
				{
					SelectParent();
					e.Use();
				}
				else if (e.keyCode == KeyCode.RightArrow && !activeParent.isLoading)
				{
					EnterChild(activeNode);
					e.Use();
				}
				else if (e.keyCode == KeyCode.Escape)
				{
					Close();
					e.Use();
				}
			}
		}

		#endregion

		#region Threading

		private readonly object guiLock = new object(); 

		private void ExecuteTask(Action task)
		{
			Ensure.That(nameof(task)).IsNotNull(task);

			if (tree.multithreaded)
			{
				lock (workerLock)
				{
					queue.Enqueue(task);

					if (workerThread == null)
					{
						workerThread = new Thread(Work);
						workerThread.Name = "Fuzzy Window";
						workerThread.Start();
					}
				}
			}
			else
			{
				RunTaskSynchronous(task);
			}
		}

		private readonly Queue<Action> queue = new Queue<Action>();
		private Thread workerThread;

		private object workerLock = new object();

		private static object taskLock = new object();

		private void RunTaskSynchronous(Action task)
		{
			lock (taskLock)
			{
				try
				{
					task();
				}
				catch (OperationCanceledException) { }
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				finally
				{
					ClearProgressBar();
					requireRepaint = true;
				}
			}
		}

		private void Work()
		{
			while (true)
			{
				Action task = null;

				lock (workerLock)
				{
					if (queue.Count > 0)
					{
						task = queue.Dequeue();
					}
					else
					{
						workerThread = null;
						return;
					}
				}

				RunTaskSynchronous(task);
			}
		}

		private string progressText;
		private float progress;
		private bool showProgress;

		public void DisplayProgressBar(string text, float progress)
		{
			progressText = text;
			this.progress = progress;
			showProgress = true;
		}

		public void DisplayProgressBar(float progress)
		{
			DisplayProgressBar(null, progress);
		}

		public static void ClearProgressBar()
		{
			if (instance == null)
			{
				return;
			}

			instance.progressText = null;
			instance.progress = 0;
			instance.showProgress = false;
		}

		#endregion
	}
}