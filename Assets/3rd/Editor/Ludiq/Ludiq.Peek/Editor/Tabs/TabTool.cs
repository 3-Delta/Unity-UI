using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class TabTool : Tool
	{
		private Type windowType { get; }

		public string tabKey { get; }

		public EditorWindow tab { get; private set; }

		private ToolControl activatorControl;

		public override bool isActive => tab != null;

		public override bool showText => PeekPlugin.Configuration.showTabsTitles;

		public bool isPinned { get; private set; }

		public bool isPopup => !isPinned;

		public override bool isTransient => isActive ? isPopup : PeekPlugin.Configuration.tabsMode == TabsMode.Popup;
		
		public TabTool(Type windowType)
		{
			Ensure.That(nameof(windowType)).IsNotNull(windowType);

			this.windowType = windowType;
			tabKey = Codebase.SerializeType(windowType);

			try
			{
				FromTitleContent((GUIContent)UnityEditorDynamic.EditorWindow.GetLocalizedTitleContentFromType(windowType));
			}
			catch
			{
				label = windowType.DisplayName();
				icon = windowType.Icon()?[IconSize.Small];
				tooltip = windowType.DisplayName();
			}
		}

		private void FromTitleContent(GUIContent titleContent)
		{
			label = titleContent.text;
			icon = titleContent.image as Texture2D ?? typeof(EditorWindow).Icon()[IconSize.Small];
			tooltip = titleContent.text;
			iconSize = new Vector2(IconSize.Small, IconSize.Small);
		}

		public override void Open(ToolControl control)
		{
			if (isActive)
			{
				Debug.LogWarning($"Trying to open tab tool twice: {tabKey}");
				return;
			}

			isPinned = PeekPlugin.Configuration.tabsMode == TabsMode.Pinned;
			activatorControl = control;

			tab = (EditorWindow)ScriptableObject.CreateInstance(windowType);

			// Adjust tool display from title content
			// Done before showing because image gets cleared in utilities and popups,
			// then serialized in our tabs data. We want the value right after OnEnable.
			FromTitleContent(tab.titleContent);

			// Load tab data
			if (PeekPlugin.Configuration.tabsData.TryGetValue(tabKey, out var windowData))
			{
				EditorJsonUtility.FromJsonOverwrite(windowData, tab);
			}

			// Show tab
			if (isPinned)
			{
				tab.ShowUtility();
			}
			else
			{
				tab.ShowPopup();
			}

			// Load tab position
			if (PeekPlugin.Configuration.tabsPositions.TryGetValue(tabKey, out var savedPosition))
			{
				tab.position = savedPosition;
			}
			else
			{
				AlignWindow();
			}

			// Align popup
			if (isPopup)
			{
				AlignWindow();
			}

			// Focus (makes open speed instant for some reason)
			tab.Focus();

			// Pre-expand scene hierarchy
			ExpandSceneHierarchy();
		}

		private void AlignWindow()
		{
			var position = tab.GetDropdownPositionCropped(activatorControl.activatorScreenPosition, tab.position.size);

			if (isPinned)
			{
				position.y += GetTitlebarHeight();
			}

			tab.position = position;
		}

		private float GetTitlebarHeight()
		{
			// TODO: Fetch internal host view border size instead of hardcoding these values

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsEditor: return 30;
				case RuntimePlatform.OSXEditor: return 16; // ?
				case RuntimePlatform.LinuxEditor: return 16; // ?
				default: return 16;
			}
		}

		public override void OnMove(ToolControl control)
		{
			if (tab != null && activatorControl == control && !isPinned)
			{
				AlignWindow();
			}
		}

		private Rect lastWindowPosition;

		public override void OnGUI(ToolControl control)
		{
			base.OnGUI(control);

			// TODO: This is really hacky to be put inside OnGUI, but it seems like the only hook we have
			SaveWindowPosition();
		}

		private void SaveWindowPosition()
		{
			if (!isActive || isPopup)
			{
				return;
			}

			// Unity has a weird bug where utility window positions are reported as zero
			// while the mouse cursor has not yet hovered the content of the window after a drag.
			// Using an ugly hack to work around it.
			// https://forum.unity.com/threads/incorrect-position-of-moved-utility-editor-window.295645/
			Rect windowPosition;

			try
			{ 
				// TODO: Dynamic calls sometimes causes crashes, moving to manual reflection.
				// If it still crashes at least we'll get a better stack trace.
				// var dWindow = tab.AsDynamic();
				// windowPosition = dWindow.m_Parent.window.position;

				var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				var tab_mParent = tab.GetType().GetField("m_Parent", bindingFlags).GetValue(tab);
				var tab_mParent_window = tab_mParent.GetType().GetProperty("window", bindingFlags)?.GetValue(tab_mParent);
				var tab_mParent_window_position = tab_mParent_window.GetType().GetProperty("position").GetValue(tab_mParent_window);

				windowPosition = tab_mParent_window_position.CastTo<Rect>();
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to get real utility window position: \n{ex}");
				windowPosition = tab.position;
			}

			if (windowPosition != lastWindowPosition && windowPosition.position != Vector2.zero)
			{
				PeekPlugin.Configuration.tabsPositions[tabKey] = windowPosition;
				PeekPlugin.Configuration.Save(nameof(PeekConfiguration.tabsPositions));
			}

			lastWindowPosition = windowPosition;
		}

		public override void Close(ToolControl control)
		{
			if (!isActive)
			{
				return;
			}

			try
			{
				PeekPlugin.Configuration.tabsData[tabKey] = EditorJsonUtility.ToJson(tab, true);
				PeekPlugin.Configuration.Save();
				SaveSceneHierarchyExpansion();
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to close tab tool: \n{ex}");
			}

			tab.Close();
		}

		private bool isSceneHierarchy => tabKey == "UnityEditor.SceneHierarchyWindow";

		private void SaveSceneHierarchyExpansion()
		{
			if (!isSceneHierarchy)
			{
				return;
			}

			try
			{
				// HACK
				// Expanded scene state is only saved in OnQuit call
				// dWindow.OnQuit();
				// But OnQuit also clears TreeView state
				// So we'll manually set that as needed

				var dWindow = tab.AsDynamic();
				dWindow.m_SceneHierarchy.m_ExpandedScenes = ((List<string>)dWindow.m_SceneHierarchy.GetExpandedSceneNames()).ToList();
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to save scene hierarchy expansion: \n{ex}");
			}
		}

		private void ExpandSceneHierarchy()
		{
			if (!isSceneHierarchy)
			{
				return;
			}

			try
			{
				if (SceneManager.sceneCount == 1)
				{
					var dWindow = tab.AsDynamic();
					dWindow.m_SceneHierarchy.SetScenesExpanded(new List<string> {SceneManager.GetSceneAt(0).name});
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to pre-expand scene hierarchy: \n{ex}");
			}
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
			return true;
		}

		public override void OnDropUpdated(ToolControl control)
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Link;

			if (!dropActivated && (DateTime.UtcNow - dropEnterTime).TotalSeconds > PeekPlugin.Configuration.dropActivationDelay)
			{
				DragAndDrop.AcceptDrag();
				control.toolbarControl.CloseAllTransientTools();

				if (!isActive)
				{
					Open(control);
				}

				dropActivated = true;
			}
		}
	}
}