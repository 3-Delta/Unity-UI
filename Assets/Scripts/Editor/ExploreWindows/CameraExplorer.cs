using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


namespace ExplorerWindows
{
	public sealed class CameraExplorer : ExploreWindow<Camera>
	{
		Column[] m_columns;
		string m_searchString = string.Empty;
		string[] m_layerOptions;


		//------------------------------------------------------
		// static function
		//------------------------------------------------------

		[MenuItem("Window/Explorer/Camera Explorer")]
		public static CameraExplorer Open()
		{
			return GetWindow<CameraExplorer>();
		}


		//------------------------------------------------------
		// unity system function
		//------------------------------------------------------

		protected override void OnEnable()
		{
			titleContent = new GUIContent("Camera Explorer");
			minSize = new Vector2(340, 150);

			m_columns = new Column[]
			{
				new Column("On", 26f, EnabledField, CompareEnabled, flexible:false),
				new Column("Depth", 60f, DepthField, CompareDepth),
				new Column("Culling Mask", 120f, CullingMaskField, CompareCullingMask),
				new Column("Clear Flags", 200f, ClearFlagsField, CompareClearFlags),
			};

			UpdateLayerOptions();

			base.OnEnable();
		}

		protected override void OnFocus()
		{
			base.OnFocus();
			UpdateLayerOptions();
		}


		//------------------------------------------------------
		// abstract methods
		//------------------------------------------------------

		protected override Column[] GetColumns()
		{
			return m_columns;
		}

		protected override List<Camera> GetItemList(List<Camera> prev)
		{
			var tmp = new List<Camera>(Camera.allCameras);

			// ここで寝かせた奴はここで有効にしたいので追加しておく
			// > 通常寝た奴はそもそもCamera.allCamerasで取得されない
			tmp.AddRange(prev.Where(i => i && !i.enabled));

			if (!string.IsNullOrEmpty(m_searchString))
			{
				tmp.RemoveAll(i => !i.name.Contains(m_searchString));
			}

			return tmp;
		}


		protected override void DrawHeader()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				m_searchString = GUILayout.TextField(m_searchString, "SearchTextField", GUILayout.Width(300));
				if (GUILayout.Button(GUIContent.none, "SearchCancelButton"))
				{
					m_searchString = string.Empty;
					GUI.FocusControl(null);
				}
			}
		}


		//------------------------------------------------------
		// camera column field
		//------------------------------------------------------

		void UpdateLayerOptions()
		{
			m_layerOptions = Enumerable.Range(0, 32)
					.Select(i => LayerMask.LayerToName(i))
					.ToArray();
		}

		void EnabledField(Rect r, Camera camera, bool selected)
		{
			camera.enabled = EditorGUI.Toggle(r, camera.enabled);
		}

		int CompareEnabled(Camera x, Camera y)
		{
			var res = x.enabled.CompareTo(y.enabled);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}

		void DepthField(Rect r, Camera camera, bool selected)
		{
			camera.depth = EditorGUI.FloatField(r, camera.depth);
		}

		int CompareDepth(Camera x, Camera y)
		{
			var res = x.depth.CompareTo(y.depth);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}
		
		void CullingMaskField(Rect r, Camera camera, bool selected)
		{
			camera.cullingMask = EditorGUI.MaskField(r, GUIContent.none, camera.cullingMask, m_layerOptions);
		}

		int CompareCullingMask(Camera x, Camera y)
		{
			var res = x.cullingMask.CompareTo(y.cullingMask);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}

		void ClearFlagsField(Rect r, Camera camera, bool selected)
		{
			camera.clearFlags = (CameraClearFlags)EditorGUI.EnumPopup(r, camera.clearFlags);
		}

		int CompareClearFlags(Camera x, Camera y)
		{
			var res = x.clearFlags.CompareTo(y.clearFlags);
			return res != 0 ? res : x.name.CompareTo(y.name);
		}
	}
}