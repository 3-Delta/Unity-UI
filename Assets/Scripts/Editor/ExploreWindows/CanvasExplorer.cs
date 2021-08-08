﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ExplorerWindows
{
	public sealed class CanvasExplorer : ExploreWindow<Canvas>
	{
		readonly string[] m_renderModeOptions =
		{
			"ScreenSpaceOverlay",
			"ScreenSpaceCamera",
			"WorldSpace",
		};

		Column[] m_columnsScreenOverlay;
		Column[] m_columnsScreenCamera;
		Column[] m_columnsWorldSpace;
		string[] m_sortingLayerNames;
		int[] m_sortingLayerUniquIDs;
		RenderMode m_renderMode;
		string m_searchString = string.Empty;
		bool m_lockList;


		//------------------------------------------------------
		// static function
		//------------------------------------------------------

		[MenuItem("Window/Explorer/Canvas Explorer")]
		public static CanvasExplorer Open()
		{
			return GetWindow<CanvasExplorer>();
		}


		//------------------------------------------------------
		// unity system function
		//------------------------------------------------------

		protected override void OnEnable()
		{
			titleContent = new GUIContent("Canvas Explorer");
			minSize = new Vector2(500, 150);

			var on = new Column("On", 26f, EnabledField, CompareEnabled, flexible: false);
			var cam = new Column("Camera", 100f, CameraField, CompareCamera);
			var sortingLayer = new Column("Sorting Layer", 100f, SortingLayerField, CompareSortingLayer);
			var sortingOrder = new Column("Order in Layer", 100f, SortingOrderField, CompareSortingOrder);

			m_columnsScreenOverlay = new Column[]
			{
				on,
				sortingLayer,
				sortingOrder,
			};
			m_columnsScreenCamera = new Column[]
			{
				on,
				cam,
				sortingLayer,
				sortingOrder,
			};
			m_columnsWorldSpace = new Column[]
			{
				on,
				cam,
				sortingLayer,
				sortingOrder,
			};

			base.OnEnable();
		}

		protected override void OnGUI()
		{
			// 表示しながらレイヤーを編集している可能性も考慮して毎回更新する
			m_sortingLayerNames = GetSortingLayerNames();
			m_sortingLayerUniquIDs = GetSortingLayerUniqueIDs();

			base.OnGUI();
		}

		//------------------------------------------------------
		// abstract methods
		//------------------------------------------------------

		protected override Column[] GetColumns()
		{
			switch (m_renderMode)
			{
				default:
				case RenderMode.ScreenSpaceOverlay: return m_columnsScreenOverlay;
				case RenderMode.ScreenSpaceCamera: return m_columnsScreenCamera;
				case RenderMode.WorldSpace: return m_columnsWorldSpace;
			}
		}

		protected override List<Canvas> GetItemList(List<Canvas> prev) 
		{
			if (m_lockList)
			{
				prev.RemoveAll(i => i == null);
				return prev;
			}

			var tmp = new List<Canvas>(FindObjectsOfType<Canvas>().Where(i => i.renderMode == m_renderMode));
			if (!string.IsNullOrEmpty(m_searchString))
			{
				tmp.RemoveAll(i => !i.name.Contains(m_searchString));
			}

			return tmp;
		}


		static float GetCameraDepth(Canvas canvas)
		{
			return canvas.worldCamera ? canvas.worldCamera.depth : 0f;
		}

		protected override void DrawHeader()
		{
			GUI.enabled = !m_lockList;
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(30);
				m_renderMode = (RenderMode)GUILayout.Toolbar((int)m_renderMode, m_renderModeOptions, GUILayout.Height(24));
				GUILayout.Space(30);
			}
			GUI.enabled = true;

			using (new EditorGUILayout.HorizontalScope())
			{
				m_lockList = GUILayout.Toggle(m_lockList, "Lock List");

				m_searchString = GUILayout.TextField(m_searchString, "SearchTextField", GUILayout.Width(300));
				if (GUILayout.Button(GUIContent.none, "SearchCancelButton"))
				{
					m_searchString = string.Empty;
					GUI.FocusControl(null);
				}
			}
		}


		//------------------------------------------------------
		// Canvas column field
		//------------------------------------------------------

		void EnabledField(Rect r, Canvas canvas, bool selected)
		{
			canvas.enabled = EditorGUI.Toggle(r, canvas.enabled);
		}

		int CompareEnabled(Canvas x, Canvas y)
		{
			var res = x.enabled.CompareTo(y.enabled);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}

		void CameraField(Rect r, Canvas canvas, bool selected)
		{
			canvas.worldCamera = EditorGUI.ObjectField(r, canvas.worldCamera, typeof(Camera), true) as Camera;
		}

		int CompareCamera(Canvas x, Canvas y)
		{
			if (x.worldCamera == null && y.worldCamera == null)
			{
				return x.name.CompareTo(y.name);
			}

			if (x.worldCamera == null) return -1;
			if (y.worldCamera == null) return 1;
			return x.worldCamera.name.CompareTo(y.worldCamera.name);
		}

		void SortingLayerField(Rect r, Canvas canvas, bool selected)
		{
			canvas.sortingLayerID = EditorGUI.IntPopup(r, canvas.sortingLayerID,
				m_sortingLayerNames,
				m_sortingLayerUniquIDs);
		}

		int CompareSortingLayer(Canvas x, Canvas y)
		{
			var res = x.sortingLayerName.CompareTo(y.sortingLayerName);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}
		
		void SortingOrderField(Rect r, Canvas canvas, bool selected)
		{
			canvas.sortingOrder = EditorGUI.IntField(r, canvas.sortingOrder);
		}

		int CompareSortingOrder(Canvas x, Canvas y)
		{
			var res = CompareSortingLayer(x, y);
			if (res != 0) return res;
			
			res = x.sortingOrder.CompareTo(y.sortingOrder);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}


		//------------------------------------------------------
		// unity internals
		//------------------------------------------------------

		static string[] GetSortingLayerNames()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}

		static int[] GetSortingLayerUniqueIDs()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
			return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
		}
	}
}