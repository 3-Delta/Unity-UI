using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityResources = UnityEngine.Resources;

namespace Ludiq.PeekCore
{
	public static class LudiqGUIUtility
	{
		static LudiqGUIUtility()
		{
			try
			{
				EditorGUI = typeof(EditorGUI).AsDynamicType();

				var binding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

				GUIClipType = typeof(GUIUtility).Assembly.GetType("UnityEngine.GUIClip", true);
				GUIClip_Unclip_Vector2 = GUIClipType.GetMethod("Unclip", binding, null, new[] {typeof(Vector2)}, null);
				GUIClip_Unclip_Rect = GUIClipType.GetMethod("Unclip", binding, null, new[] {typeof(Rect)}, null);
				GUIClip_Clip_Vector2 = GUIClipType.GetMethod("Clip", binding, null, new[] {typeof(Vector2)}, null);
				GUIClip_Clip_Rect = GUIClipType.GetMethod("Clip", binding, null, new[] {typeof(Rect)}, null);
				GUIClip_topmostRect = GUIClipType.GetProperty("topmostRect", binding);
				GUIClip_visibleRect = GUIClipType.GetProperty("visibleRect", binding);
				GUIClip_GetTopRect = GUIClipType.GetMethod("GetTopRect", binding, null, Empty<Type>.array, null);
				GUIClip_GetMatrix = GUIClipType.GetMethod("GetMatrix", binding, null, Empty<Type>.array, null);
				GUIClip_SetMatrix = GUIClipType.GetMethod("SetMatrix", binding, null, new[] {typeof(Matrix4x4)}, null);

				if (GUIClip_Unclip_Vector2 == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "Unclip");
				}

				if (GUIClip_Unclip_Rect == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "Unclip");
				}

				if (GUIClip_Clip_Vector2 == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "Clip");
				}

				if (GUIClip_Clip_Rect == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "Clip");
				}

				if (GUIClip_topmostRect == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "topmostRect");
				}

				if (GUIClip_visibleRect == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "visibleRect");
				}

				if (GUIClip_GetTopRect == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "GetTopRect");
				}

				if (GUIClip_GetMatrix == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "GetMatrix");
				}

				if (GUIClip_SetMatrix == null)
				{
					throw new MissingMemberException(GUIClipType.FullName, "SetMatrix");
				}

				GUIStyle_CalcSizeWithConstraints = typeof(GUIStyle).GetMethod("CalcSizeWithConstraints", BindingFlags.Instance | BindingFlags.NonPublic);

				if (GUIStyle_CalcSizeWithConstraints == null)
				{
					throw new MissingMemberException(typeof(GUIStyle).FullName, "CalcSizeWithConstraints");
				}

				EditorGUIUtility_GetHelpIcon = typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.Static | BindingFlags.NonPublic);
				EditorGUIUtility_GetBoldDefaultFont = typeof(EditorGUIUtility).GetMethod("GetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
				EditorGUIUtility_SetBoldDefaultFont = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
				EditorGUIUtility_s_LastControlID = typeof(EditorGUIUtility).GetField("s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic);

				if (EditorGUIUtility_GetHelpIcon == null)
				{
					throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "GetHelpIcon");
				}

				if (EditorGUIUtility_GetBoldDefaultFont == null)
				{
					throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "GetBoldDefaultFont");
				}

				if (EditorGUIUtility_SetBoldDefaultFont == null)
				{
					throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "SetBoldDefaultFont");
				}

				if (EditorGUIUtility_s_LastControlID == null)
				{
					throw new MissingMemberException(typeof(EditorGUIUtility).FullName, "s_LastControlID");
				}

				InspectorWindowType = UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.InspectorWindow", true);
				InspectorWindow_RepaintAllInspectors = InspectorWindowType.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic);

				if (InspectorWindow_RepaintAllInspectors == null)
				{
					throw new MissingMemberException("UnityEditor.InspectorWindow", "RepaintAllInspectors");
				}

				ContainerWindowType = UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.ContainerWindow", true);
				DockAreaType = UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.DockArea", true);
				ContainerWindow_m_ShowMode = ContainerWindowType.GetField("m_ShowMode", BindingFlags.Instance | BindingFlags.NonPublic);
				ContainerWindow_position = ContainerWindowType.GetProperty("position", BindingFlags.Instance | BindingFlags.Public);

				if (ContainerWindow_m_ShowMode == null)
				{
					throw new MissingMemberException("UnityEditor.ContainerWindow", "m_ShowMode");
				}

				if (ContainerWindow_position == null)
				{
					throw new MissingMemberException("UnityEditor.ContainerWindow", "position");
				}

				EditorWindow_ShowModal = typeof(EditorWindow).GetMethod("ShowModal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (EditorWindow_ShowModal == null)
				{
					throw new MissingMemberException("UnityEditor.EditorWindow", "ShowModal");
				}

				ShowModeType = UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.ShowMode", true);

				EditorWindow_ShowAsDropDown = typeof(EditorWindow).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == "ShowAsDropDown" && m.GetParameters().Length == 4);
				EditorWindow_ShowAsDropDownFitToScreen = typeof(EditorWindow).GetMethod("ShowAsDropDownFitToScreen", BindingFlags.Instance | BindingFlags.NonPublic);
				EditorWindow_ShowWithMode = typeof(EditorWindow).GetMethod("ShowWithMode", BindingFlags.Instance | BindingFlags.NonPublic);

				if (EditorWindow_ShowAsDropDown == null)
				{
					throw new MissingMemberException("UnityEditor.EditorWindow", "ShowAsDropDown");
				}

				if (EditorWindow_ShowAsDropDownFitToScreen == null)
				{
					throw new MissingMemberException("UnityEditor.EditorWindow", "ShowAsDropDownFitToScreen");
				}

				if (EditorWindow_ShowWithMode == null)
				{
					throw new MissingMemberException("UnityEditor.EditorWindow", "ShowWithMode");
				}

				// Different signature in different Unity versions
				GUIUtility_guiDepth = typeof(GUIUtility).GetProperty("guiDepth", BindingFlags.Static | BindingFlags.NonPublic);
				GUIUtility_Internal_GetGUIDepth = typeof(GUIUtility).GetMethod("Internal_GetGUIDepth", BindingFlags.Static | BindingFlags.NonPublic);

				if (GUIUtility_guiDepth == null && GUIUtility_Internal_GetGUIDepth == null)
				{
					throw new MissingMemberException("GUIUtility", "guiDepth");
				}
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		private static readonly dynamic EditorGUI;

		public static UnityObject ValidateObjectFieldAssignment(UnityObject[] references, Type objType, SerializedProperty property = null)
		{
			return EditorGUI.ValidateObjectFieldAssignment(references, objType, property, 0);
		}

		public static void BeginLabelHighlight(string searchContext, Color searchHighlightSelectionColor, Color searchHighlightColor)
		{
			UnityEditorDynamic.EditorGUI.BeginLabelHighlight(searchContext, searchHighlightSelectionColor, searchHighlightColor);
		}
		
		public static void BeginLabelHighlight(string searchContext)
		{
			BeginLabelHighlight(searchContext, ColorPalette.unitySelectionHighlight, ColorPalette.unityForegroundSelected);
		}

		public static void EndLabelHighlight()
		{
			UnityEditorDynamic.EditorGUI.EndLabelHighlight();
		}

		public static LabelHighlightScope LabelHighlight(string searchContext, Color searchHighlightSelectionColor, Color searchHighlightColor)
		{
			return new LabelHighlightScope(searchContext, searchHighlightSelectionColor, searchHighlightColor);
		}
		
		public static LabelHighlightScope LabelHighlight(string searchContext)
		{
			return new LabelHighlightScope(searchContext, ColorPalette.unitySelectionHighlight, ColorPalette.unityForegroundSelected);
		}

		public struct LabelHighlightScope : IDisposable
		{
			public LabelHighlightScope(string searchContext, Color searchHighlightSelectionColor, Color searchHighlightColor)
			{
				BeginLabelHighlight(searchContext, searchHighlightSelectionColor, searchHighlightColor);
			}

			public void Dispose()
			{
				EndLabelHighlight();
			}
		}

		private static readonly GUIContent tempContent = new GUIContent();

		public static GUIContent TempContent(string text, Texture image, string tooltip)
		{
			tempContent.text = text;
			tempContent.image = image;
			tempContent.tooltip = tooltip;
			return tempContent;
		}

		public static GUIContent TempContent(string text, Texture image)
		{
			tempContent.text = text;
			tempContent.image = image;
			tempContent.tooltip = null;
			return tempContent;
		}

		public static GUIContent TempContent(string text)
		{
			tempContent.text = text;
			tempContent.image = null;
			tempContent.tooltip = null;
			return tempContent;
		}

		public static GUIContent TempContent(Texture image)
		{
			tempContent.text = null;
			tempContent.image = image;
			tempContent.tooltip = null;
			return tempContent;
		}

		public static GUIContent TempContent(Texture image, string tooltip)
		{
			tempContent.text = null;
			tempContent.image = image;
			tempContent.tooltip = tooltip;
			return tempContent;
		}

		public static GUIContent TempContent(string text, string tooltip)
		{
			tempContent.text = text;
			tempContent.image = null;
			tempContent.tooltip = tooltip;
			return tempContent;
		}

		public static bool isFlatSkin { get; } =
#if UNITY_2019_3_OR_NEWER
		true;
#else
			false;
#endif

		private static readonly Type GUIClipType;

		private static readonly MethodInfo GUIClip_Unclip_Vector2; // public static Vector2 Unclip(Vector2 pos)

		private static readonly MethodInfo GUIClip_Unclip_Rect; // public static Rect Unclip(Rect rect)

		private static readonly MethodInfo GUIClip_Clip_Vector2; // public static Vector2 Clip(Vector2 absolutePos)

		private static readonly MethodInfo GUIClip_Clip_Rect; // public static Rect Clip(Rect absoluteRect)

		private static readonly PropertyInfo GUIClip_topmostRect; // public static Rect topmostRect { get; }

		private static readonly PropertyInfo GUIClip_visibleRect; // public static Rect topmostRect { get; }

		private static readonly MethodInfo GUIClip_GetTopRect; // internal static Rect GetTopRect()

		private static readonly MethodInfo GUIClip_GetMatrix; // internal static Matrix4x4 GetMatrix()

		private static readonly MethodInfo GUIClip_SetMatrix; // internal static Matrix4x4 GetMatrix(Matrix4x4 matrix)

		private static readonly MethodInfo GUIStyle_CalcSizeWithConstraints; // internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)

		// 2018+
		private static readonly PropertyInfo GUIUtility_guiDepth; // internal static extern int guiDepth { get; }

		// 2017
		private static readonly MethodInfo GUIUtility_Internal_GetGUIDepth; // extern internal static  int Internal_GetGUIDepth () ;

		private static readonly Vector2[] corners1 = new Vector2[4];

		private static readonly Vector2[] corners2 = new Vector2[4];

		public static Rect GUIToScreenRect(Rect rect)
		{
			return new Rect(GUIUtility.GUIToScreenPoint(rect.position), rect.size);
		}

		public static Rect VerticalSection(this Rect rect, ref float y, float height)
		{
			var section = new Rect
			(
				rect.x,
				y,
				rect.width,
				height
			);

			y += height;

			return section;
		}

		public static RectOffset Clone(this RectOffset rectOffset)
		{
			return new RectOffset
			(
				rectOffset.left,
				rectOffset.right,
				rectOffset.top,
				rectOffset.bottom
			);
		}

		public static Rect Lerp(Rect a, Rect b, float t)
		{
			return new Rect
			(
				Vector2.Lerp(a.position, b.position, t),
				Vector2.Lerp(a.size, b.size, t)
			);
		}

		public static Rect ExpandBy(this Rect rect, float padding)
		{
			rect.xMin -= padding;
			rect.yMin -= padding;
			rect.xMax += padding;
			rect.yMax += padding;
			return rect;
		}

		public static Rect ShrinkBy(this Rect rect, float padding)
		{
			return rect.ExpandBy(-padding);
		}

		public static Rect ExpandBy(this Rect rect, RectOffset offset)
		{
			if (offset == null)
			{
				return rect;
			}

			rect.x -= offset.left;
			rect.y -= offset.top;
			rect.width += offset.left + offset.right;
			rect.height += offset.top + offset.bottom;
			return rect;
		}

		public static Rect ShrinkBy(this Rect rect, RectOffset offset)
		{
			if (offset == null)
			{
				return rect;
			}

			rect.x += offset.left;
			rect.y += offset.top;
			rect.width -= offset.left + offset.right;
			rect.height -= offset.top + offset.bottom;
			return rect;
		}

		public static Rect ExpandByX(this Rect rect, RectOffset offset)
		{
			if (offset == null)
			{
				return rect;
			}

			rect.x -= offset.left;
			rect.width += offset.left + offset.right;
			return rect;
		}

		public static Rect ShrinkByX(this Rect rect, RectOffset offset)
		{
			if (offset == null)
			{
				return rect;
			}

			rect.x += offset.left;
			rect.width -= offset.left + offset.right;
			return rect;
		}

		public static Rect ExpandByY(this Rect rect, RectOffset offset)
		{
			if (offset == null)
			{
				return rect;
			}

			rect.y -= offset.top;
			rect.height += offset.top + offset.bottom;
			return rect;
		}

		public static Rect ShrinkByY(this Rect rect, RectOffset offset)
		{
			if (offset == null)
			{
				return rect;
			}

			rect.y += offset.top;
			rect.height -= offset.top + offset.bottom;
			return rect;
		}

		public static Rect ScaleAroundCenter(this Rect rect, float scale)
		{
			var center = rect.center;
			rect.xMin = center.x + ((rect.xMin - center.x) * scale);
			rect.yMin = center.y + ((rect.yMin - center.y) * scale);
			rect.xMax = center.x + ((rect.xMax - center.x) * scale);
			rect.yMax = center.y + ((rect.yMax - center.y) * scale);
			return rect;
		}

		public static Rect Encompass(this Rect rect, float x, float y)
		{
			if (rect.xMin > x)
			{
				rect.xMin = x;
			}

			if (rect.yMin > y)
			{
				rect.yMin = y;
			}

			if (rect.xMax < x)
			{
				rect.xMax = x;
			}

			if (rect.yMax < y)
			{
				rect.yMax = y;
			}

			return rect;
		}

		public static Rect Encompass(this Rect rect, Vector2 point)
		{
			return rect.Encompass(point.x, point.y);
		}

		public static Rect Encompass(this Rect? rect, Vector2 point)
		{
			if (rect == null)
			{
				return new Rect(point.x, point.y, 0, 0);
			}
			else
			{
				return rect.Value.Encompass(point);
			}
		}

		public static Rect Encompass(this Rect? rect, Rect other)
		{
			if (rect == null)
			{
				return other;
			}
			else
			{
				return rect.Value.Encompass(other);
			}
		}

		public static Rect? Encompass(this Rect? rect, Rect? other)
		{
			if (rect == null)
			{
				return other;
			}
			else if (other == null)
			{
				return rect;
			}
			else
			{
				return rect.Value.Encompass(other.Value);
			}
		}

		public static Rect Encompass(this Rect rect, Rect other)
		{
			// Micro-optim
			var xMin = other.xMin;
			var xMax = other.xMax;
			var yMin = other.yMin;
			var yMax = other.yMax;

			rect = rect.Encompass(xMin, yMin);
			rect = rect.Encompass(xMin, yMax);
			rect = rect.Encompass(xMax, yMin);
			rect = rect.Encompass(xMax, yMax);

			return rect;
		}

		public static bool Encompasses(this Rect rect, Rect other)
		{
			return
				rect.Contains(new Vector2(other.xMin, other.yMin)) &&
				rect.Contains(new Vector2(other.xMin, other.yMax)) &&
				rect.Contains(new Vector2(other.xMax, other.yMin)) &&
				rect.Contains(new Vector2(other.xMax, other.yMax));
		}

		public static void ClosestPoints(Rect rect1, Rect rect2, out Vector2 point1, out Vector2 point2)
		{
			corners1[0] = new Vector2(rect1.xMin, rect1.yMin);
			corners1[1] = new Vector2(rect1.xMin, rect1.yMax);
			corners1[2] = new Vector2(rect1.xMax, rect1.yMin);
			corners1[3] = new Vector2(rect1.xMax, rect1.yMax);

			corners2[0] = new Vector2(rect2.xMin, rect2.yMin);
			corners2[1] = new Vector2(rect2.xMin, rect2.yMax);
			corners2[2] = new Vector2(rect2.xMax, rect2.yMin);
			corners2[3] = new Vector2(rect2.xMax, rect2.yMax);

			var minDistance = float.MaxValue;

			point1 = rect1.center;
			point2 = rect2.center;

			for (var i = 0; i < 4; i++)
			{
				for (var j = 0; j < 4; j++)
				{
					var corner1 = corners1[i];
					var corner2 = corners2[j];
					var distance = Vector2.Distance(corner1, corner2);

					if (Vector2.Distance(corner1, corner2) < minDistance)
					{
						point1 = corner1;
						point2 = corner2;
						minDistance = distance;
					}
				}
			}
		}

		public static Vector2 Perpendicular1(this Vector2 vector)
		{
			return new Vector2(vector.y, -vector.x);
		}

		public static Vector2 Perpendicular2(this Vector2 vector)
		{
			return new Vector2(-vector.y, vector.x);
		}

		public static float PixelPerfect(this float f)
		{
			return Mathf.Floor(f);
		}

		public static Vector2 PixelPerfect(this Vector2 vector)
		{
			return new Vector2(vector.x.PixelPerfect(), vector.y.PixelPerfect());
		}

		public static Rect PixelPerfect(this Rect rect)
		{
			return new Rect(rect.position.PixelPerfect(), rect.size.PixelPerfect());
		}

		public static Vector2 Size(this Texture texture)
		{
			return new Vector2(texture.width, texture.height);
		}

		public static bool CtrlOrCmd(this Event e)
		{
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				return e.command;
			}

			return e.control;
		}

		public static bool IsContextMouseButton(this Event e)
		{
			if (Application.platform == RuntimePlatform.OSXEditor && e.control && e.button == (int)MouseButton.Left)
			{
				return true;
			}

			return e.button == (int)MouseButton.Right;
		}

		public static void NineSlice(this Rect r,
			RectOffset o,
			out Rect topLeft, out Rect topCenter, out Rect topRight,
			out Rect middleLeft, out Rect middleCenter, out Rect middleRight,
			out Rect bottomLeft, out Rect bottomCenter, out Rect bottomRight)
		{
			topLeft = new Rect
			(
				r.x,
				r.y,
				o.left,
				o.top
			);

			topCenter = new Rect
			(
				r.x + o.left,
				r.y,
				r.width - o.left - o.right,
				o.top
			);

			topRight = new Rect
			(
				r.xMax - o.right,
				r.y,
				o.right,
				o.top
			);

			middleLeft = new Rect
			(
				r.x,
				r.y + o.top,
				o.left,
				r.height - o.top - o.bottom
			);

			middleCenter = new Rect
			(
				r.x + o.left,
				r.y + o.top,
				r.width - o.left - o.right,
				r.height - o.top - o.bottom
			);

			middleRight = new Rect
			(
				r.xMax - o.right,
				r.y + o.top,
				o.right,
				r.height - o.top - o.bottom
			);

			bottomLeft = new Rect
			(
				r.x,
				r.yMax - o.bottom,
				o.left,
				o.bottom
			);

			bottomCenter = new Rect
			(
				r.x + o.left,
				r.yMax - o.bottom,
				r.width - o.left - o.bottom,
				o.bottom
			);

			bottomRight = new Rect
			(
				r.xMax - o.right,
				r.yMax - o.bottom,
				o.right,
				o.bottom
			);
		}

		private static int guiDepth
		{
			get
			{
				try
				{
					if (GUIUtility_guiDepth != null)
					{
						return (int)GUIUtility_guiDepth.GetValue(null, null);
					}
					else if (GUIUtility_Internal_GetGUIDepth != null)
					{
						return (int)GUIUtility_Internal_GetGUIDepth.Invoke(null, Empty<object>.array);
					}
					else
					{
						throw new MissingMemberException("GUIUtility", "guiDepth");
					}
				}
				catch (Exception ex)
				{
					// Safeguard because I'm not 100% sure how reliable that property is e.g. in other threads.
					// In the worst case, returning 0 should be fine because we're probably not in a GUI call.
					// Warn to get reports if it happens though.
					Debug.LogWarning("Fetching GUI depth failed, returning zero.\n" + ex);
					return 0;
				}
			}
		}

		// In certain cases, mainly editor application callbacks like
		// undo/redo/selection change, we're not actually on GUI but 
		// Unity keeps the GUI depth. This makes throwing ExitGUI
		// exceptions dangerous.
		public static void BeginNotActuallyOnGUI()
		{
			notOnGuiDepth++;
		}

		public static void EndNotActuallyOnGUI()
		{
			if (notOnGuiDepth == 0)
			{
				throw new InvalidOperationException();
			}

			notOnGuiDepth--;
		}

		private static int notOnGuiDepth = 0;

		public static bool isWithinGUI => notOnGuiDepth == 0 && guiDepth > 0;

		public static string EscapeRichText(string s)
		{
			// Unity rich text supports <material=...> and <color=...> tags, which
			// mess up rendering with generic types such as List<Material>. 
			// Escape these edge cases with a zero-width non-breaking space character.
			return s.Replace("<Material>", "<\uFEFFMaterial>")
				.Replace("<Color>", "<\uFEFFColor>");
		}



		#region Textures

		public static Vector2 Size(this Texture2D texture)
		{
			return new Vector2(texture.width, texture.height);
		}

		public static Vector2 PointSize(this Texture2D texture)
		{
			return new Vector2(texture.width / EditorGUIUtility.pixelsPerPoint, texture.height / EditorGUIUtility.pixelsPerPoint);
		}

		public static Texture2D LoadBuiltinTextureRaw(string name, bool required = true)
		{
			Texture2D icon = EditorGUIUtility.FindTexture(name);

			if (icon == null)
			{
				icon = UnityEditorDynamic.EditorGUIUtility.LoadIcon(name);
			}

			if (required && icon == null)
			{
				Debug.LogWarning($"Missing built-in editor texture: \n{name}.");
			}

			return icon; 
		}

		public static EditorTexture LoadBuiltinTexture(string name, bool required = true)
		{
			return EditorTexture.Single(LoadBuiltinTextureRaw(name, required));
		}

		#endregion



		#region Styles

		public static Vector2 CalcSizeWithConstraints(this GUIStyle style, GUIContent content, Vector2 constraints)
		{
			Ensure.That(nameof(style)).IsNotNull(style);

			return (Vector2)GUIStyle_CalcSizeWithConstraints.InvokeOptimized(style, content, constraints);
		}

		public static string DimString(string s)
		{
			return s.Colored(ColorPalette.unityForegroundDim);
		}

		#endregion



		#region Clipping

		public static Vector2 Unclip(Vector2 pos)
		{
			return (Vector2)GUIClip_Unclip_Vector2.InvokeOptimized(null, pos);
		}

		public static Rect Unclip(Rect rect)
		{
			return (Rect)GUIClip_Unclip_Rect.InvokeOptimized(null, rect);
		}

		public static Vector2 Clip(Vector2 absolutePos)
		{
			return (Vector2)GUIClip_Clip_Vector2.InvokeOptimized(null, absolutePos);
		}

		public static Rect Clip(Rect absoluteRect)
		{
			return (Rect)GUIClip_Clip_Rect.InvokeOptimized(null, absoluteRect);
		}

		public static Rect topmostClipRect => (Rect)GUIClip_topmostRect.GetValueOptimized(null);

		public static Rect topClipRect => (Rect)GUIClip_GetTopRect.InvokeOptimized(null);

		public static Rect visibleClipRect => (Rect)GUIClip_visibleRect.GetValueOptimized(null);

		public static Matrix4x4 clipMatrix
		{
			get => (Matrix4x4)GUIClip_GetMatrix.InvokeOptimized(null);
			set => GUIClip_SetMatrix.InvokeOptimized(null, value);
		}

		public struct NoClipContext : IDisposable
		{
			private List<Rect> clipRects;

			public NoClipContext(List<Rect> clipRects)
			{
				this.clipRects = clipRects;
			}

			public void Dispose()
			{
				EndNoClip(clipRects);
			}
		}

		// Inspired from:
		// https://gist.github.com/Seneral/2c8e7dfe712b9f53c60f80722fbce5bd

		public static NoClipContext noClip => new NoClipContext(BeginNoClip());

		private static readonly Rect magicTopMostClip = new Rect(-10000, -10000, 40000, 40000);

		public static List<Rect> BeginNoClip()
		{
			var clipRects = new List<Rect>();

			var clipRect = topClipRect;

			while (clipRect != magicTopMostClip)
			{
				clipRects.Add(clipRect);
				GUI.EndClip();
				clipRect = topClipRect;
			}

			clipRects.Reverse();
			return clipRects;
		}

		public static void EndNoClip(List<Rect> clipRects)
		{
			foreach (var clipRect in clipRects)
			{
				GUI.BeginClip(clipRect);
			}
		}

		#endregion



		#region Layout

		public const float scrollBarWidth = 15f;

		public static bool currentInspectorHasScrollbar { get; set; }

		public static float currentInspectorWidthWithoutScrollbar
		{
			get
			{
				var width = currentInspectorWidth.value;

				if (currentInspectorHasScrollbar)
				{
					width -= scrollBarWidth;
				}

				return width;
			}
		}

		private static RectOffset windowOverdraw;

		public static void BeginScrollablePanel(Rect outerPosition, Func<float, float> getInnerHeight, out Rect innerPosition, ref Vector2 scroll, RectOffset overdraw = null)
		{
			Ensure.That(nameof(getInnerHeight)).IsNotNull(getInnerHeight);

			innerPosition = new Rect(Vector2.zero, outerPosition.size);

			innerPosition = innerPosition.ExpandByX(overdraw);

			innerPosition.height = getInnerHeight(innerPosition.width);

			currentInspectorWidth.BeginOverride(innerPosition.width);

			if (innerPosition.height > outerPosition.height)
			{
				innerPosition.width -= scrollBarWidth;
				innerPosition.height = getInnerHeight(innerPosition.width);
				currentInspectorHasScrollbar = true;
			}
			else
			{
				currentInspectorHasScrollbar = false;
			}

			innerPosition = innerPosition.ExpandByY(overdraw);

			scroll = GUI.BeginScrollView(outerPosition, scroll, innerPosition.ShrinkBy(overdraw));

			GUI.BeginClip(innerPosition, Vector2.zero, Vector2.zero, false);
		}

		public static void EndScrollablePanel()
		{
			GUI.EndClip();
			GUI.EndScrollView();
			currentInspectorWidth.EndOverride();
		}

		public static void BeginScrollableWindow(Rect windowPosition, Func<float, float> getInnerHeight, out Rect innerPosition, ref Vector2 scroll)
		{
			// Needs to be called from the main thread
			if (windowOverdraw == null)
			{
				windowOverdraw = new RectOffset(0, 3, 1, 1);
			}

			var outerPosition = new Rect(Vector2.zero, windowPosition.size);

			BeginScrollablePanel(outerPosition, getInnerHeight, out innerPosition, ref scroll, windowOverdraw);
		}

		public static void EndScrollableWindow()
		{
			EndScrollablePanel();
		}

		public static OverrideStack<float> labelWidth { get; } = new OverrideStack<float>
		(
			() => EditorGUIUtility.labelWidth,
			value => EditorGUIUtility.labelWidth = value
		);

		public static OverrideStack<int> iconSize { get; } = new OverrideStack<int>
		(
			() => (int)EditorGUIUtility.GetIconSize().x,
			value => EditorGUIUtility.SetIconSize(new Vector2(value, value))
		);

		public static OverrideStack<Vector2> realIconSize { get; } = new OverrideStack<Vector2>
		(
			() => EditorGUIUtility.GetIconSize(),
			value => EditorGUIUtility.SetIconSize(value)
		);

		public static void TryUse(this Event e)
		{
			if (e != null && e.type != EventType.Repaint && e.type != EventType.Layout)
			{
				e.Use();
			}
		}

		private static float? _currentInspectorWidthOverride;

		public static OverrideStack<float> currentInspectorWidth { get; } = new OverrideStack<float>
		(
			() => _currentInspectorWidthOverride ?? EditorGUIUtility.currentViewWidth,
			value => _currentInspectorWidthOverride = value,
			() => _currentInspectorWidthOverride = null
		);

		#endregion



		#region Controls

		private static readonly FieldInfo EditorGUIUtility_s_LastControlID; // internal static int EditorGUIUtility.s_LastControlID

		public static int GetLastControlID()
		{
			return (int)EditorGUIUtility_s_LastControlID.GetValue(null);
		}

		#endregion



		#region Inspector

		private static readonly Type InspectorWindowType; // internal class InspectorWindow : EditorWindow, IHasCustomMenu

		private static readonly MethodInfo InspectorWindow_RepaintAllInspectors; // internal static void InspectorWindow.RepaintAllInspectors()

		public static void RepaintAllInspectors()
		{
			InspectorWindow_RepaintAllInspectors.Invoke(null, new object[0]);
		}

		public static void FocusInspector()
		{
			EditorWindow.FocusWindowIfItsOpen(InspectorWindowType);
		}

		#endregion



		#region Windows

		private static readonly Type ContainerWindowType; // internal sealed class ContainerWindow : ScriptableObject

		private static readonly Type DockAreaType; // internal class DockArea : HostView, IDropArea

		private static readonly FieldInfo ContainerWindow_m_ShowMode; // private int ContainerWindow.m_ShowMode;

		private static readonly PropertyInfo ContainerWindow_position; // public Rect ContainerWindow.position;

		private static readonly MethodInfo EditorWindow_ShowModal; // internal void ShowModal()

		private static readonly MethodInfo EditorWindow_ShowWithMode; // internal void ShowWithMode(ShowMode mode);

		private static readonly MethodInfo EditorWindow_ShowAsDropDown; // internal void ShowAsDropDown(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder, ShowMode mode)

		private static readonly MethodInfo EditorWindow_ShowAsDropDownFitToScreen; // internal void ShowAsDropDown(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder, ShowMode mode)

		private static readonly Type ShowModeType; // internal enum ShowMode

		private static UnityObject FindContainerWindow()
		{
			return UnityResources.FindObjectsOfTypeAll(ContainerWindowType).FirstOrDefault(window => (int)ContainerWindow_m_ShowMode.GetValue(window) == 4);
		}

		public static Rect mainEditorWindowPosition
		{
			get
			{
				try
				{
					var containerWindow = FindContainerWindow();

					if (containerWindow == null)
					{
						return new Rect(0, 0, Screen.width, Screen.height);
					}

					return (Rect)ContainerWindow_position.GetValue(containerWindow, null);
				}
				catch (Exception ex)
				{
					throw new UnityEditorInternalException(ex);
				}
			}
		}

		public static void Center(this EditorWindow window)
		{
			var mainEditorWindowPosition = LudiqGUIUtility.mainEditorWindowPosition;

			window.position = new Rect
			(
				(mainEditorWindowPosition.position + (mainEditorWindowPosition.size / 2)) - (window.position.size / 2),
				window.position.size
			);
		}

		public static bool ShowNextTabIfPossible(this EditorWindow window)
		{
			return window.AsDynamic().ShowNextTabIfPossible();
		}

		public static bool ShowPrevTabIfPossible(this EditorWindow window)
		{
			var dynamicWindow = window.AsDynamic();
			var parent = dynamicWindow.m_Parent;

			if (parent.Is(DockAreaType) && parent != null)
			{
				var num = (int)Mathf.Repeat(parent.m_Panes.IndexOf(window) - 1, parent.m_Panes.Count);

				if (parent.selected != num)
				{
					parent.selected = num;
					parent.Repaint();
					return true;
				}
			}

			return false;
		}

		public static EditorWindow[] GetTabs(this EditorWindow window)
		{
			var dynamicWindow = window.AsDynamic();
			var parent = dynamicWindow.m_Parent;

			if (parent.Is(DockAreaType) && parent != null)
			{
				return parent.m_Panes.ToArray();
			}
			else
			{
				return window.Yield().ToArray();
			}
		}

		public static bool IsFocused(this EditorWindow window)
		{
			return EditorWindow.focusedWindow == window;
		}

		public static void ShowModal(this EditorWindow window)
		{
			EditorWindow_ShowModal.InvokeOptimized(window);
		}

		public static void ShowNoBorder(this EditorWindow window)
		{
			window.ShowWithMode(0);
		}

		public static void ShowWithMode(this EditorWindow window, int mode)
		{
			EditorWindow_ShowWithMode.Invoke(window, new object[] {mode});
		}

		public static void ShowAsDropDownWithKeyboardFocus(this EditorWindow window, Rect buttonRect, Vector2 windowSize)
		{
			var PopupMenuWithKeyboardFocus = Enum.ToObject(ShowModeType, 6);

			EditorWindow_ShowAsDropDown.InvokeOptimized(window, buttonRect, windowSize, null, PopupMenuWithKeyboardFocus);
		}

		public static Rect GetDropdownPosition(this EditorWindow window, Rect buttonRect, Vector2 windowSize)
		{
			return (Rect)EditorWindow_ShowAsDropDownFitToScreen.InvokeOptimized(window, buttonRect, windowSize, null);
		}

		public static Rect GetDropdownPositionCropped(this EditorWindow window, Rect buttonRect, Vector2 windowSize)
		{
			return CropDropdownPosition(GetDropdownPosition(window, buttonRect, windowSize));
		}

		public static Rect CropDropdownPosition(Rect dropdownPosition)
		{
			if (Application.platform == RuntimePlatform.OSXEditor && LudiqCore.Configuration.limitFuzzyFinderHeight)
			{
				// OSX disregards the Y entirely if the window is higher than the desktop space
				// and will try to move it up until it fits. Therefore, we'll cut the window down here.
				// However, we can't use the screen resolution, because it doesn't include the dock.

				var maxY = mainEditorWindowPosition.yMax;

				if (dropdownPosition.yMax > maxY)
				{
					dropdownPosition.height -= dropdownPosition.yMax - maxY;
				}
			}

			return dropdownPosition;
		}

		public static bool TryDockNextTo(this EditorWindow window, params Type[] windowTypes)
		{
			var containerWindows = ContainerWindowType.AsDynamicType().windows;

			foreach (var type in windowTypes)
			{
				var desired = type;

				foreach (var containerWindow in containerWindows)
				{
					var dynamicContainerWindow = ((object)containerWindow).AsDynamic();

					foreach (var view in dynamicContainerWindow.rootView.allChildren)
					{
						var dynamicView = ((object)view).AsDynamic();

						if (!DockAreaType.IsInstanceOfTypeNullable((object)view))
						{
							continue;
						}

						foreach (var pane in dynamicView.m_Panes)
						{
							if (pane.GetType() == desired)
							{
								dynamicView.AddTab(window, true);
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public static void ShowNextTo(this EditorWindow window, params Type[] windowTypes)
		{
			if (!window.TryDockNextTo(windowTypes))
			{
				window.Show();
			}
		}

		#endregion



		#region Help Box

		private static readonly MethodInfo EditorGUIUtility_GetHelpIcon; // internal static Texture2D GetHelpIcon(MessageType type)

		public const float HelpBoxHeight = 40;

		public static Texture2D GetHelpIcon(MessageType type)
		{
			try
			{
				return (Texture2D)EditorGUIUtility_GetHelpIcon.Invoke(null, new object[] {type});
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		public static float GetHelpBoxWidth(string message, MessageType messageType)
		{
			return EditorStyles.helpBox.CalcSize(new GUIContent(message, GetHelpIcon(messageType))).x + 8;
		}

		public static float GetHelpBoxHeight(string message, MessageType messageType, float width)
		{
			return EditorStyles.helpBox.CalcHeight(new GUIContent(message, GetHelpIcon(messageType)), width);
		}

		#endregion



		#region Font Bolding

		private static readonly MethodInfo EditorGUIUtility_GetBoldDefaultFont; // internal static bool EditorGUIUtility.GetBoldDefaultFont()

		private static readonly MethodInfo EditorGUIUtility_SetBoldDefaultFont; // internal static void EditorGUIUtility.SetBoldDefaultFont(bool isBold)

		private static readonly Dictionary<GUIStyle, GUIStyle> boldedStyles = new Dictionary<GUIStyle, GUIStyle>();

		public static bool editorHasBoldFont
		{
			get
			{
				try
				{
					return (bool)EditorGUIUtility_GetBoldDefaultFont.InvokeOptimized(null);
				}
				catch (Exception ex)
				{
					throw new UnityEditorInternalException(ex);
				}
			}
			set
			{
				try
				{
					EditorGUIUtility_SetBoldDefaultFont.InvokeOptimized(null, value);
				}
				catch (Exception ex)
				{
					throw new UnityEditorInternalException(ex);
				}
			}
		}

		public static GUIStyle BoldedStyle(this GUIStyle style, bool force = false)
		{
			if (!force && !editorHasBoldFont)
			{
				return style;
			}

			if (!boldedStyles.ContainsKey(style))
			{
				var boldedStyle = new GUIStyle(style);
				boldedStyle.fontStyle = FontStyle.Bold;
				boldedStyles.Add(style, boldedStyle);
			}

			return boldedStyles[style];
		}

		#endregion



		#region Multiline

		public static float GetMultilineHeight(params float[] widths)
		{
			return GetMultilineHeightConfigurable(EditorGUIUtility.singleLineHeight, 2, widths);
		}

		public static Rect[] GetMultilinePositions(Rect totalPosition, params float[] widths)
		{
			return GetMultilinePositionsConfigurable(totalPosition, 2, 3, widths);
		}

		public static float GetMultilineHeightConfigurable(float lineHeight, float verticalSpacing, params float[] widths)
		{
			var total = 0f;

			for (var i = 0; i < widths.Length; i++)
			{
				total += widths[i];
			}

			var lines = Mathf.CeilToInt(total);

			return (lineHeight * lines) + (verticalSpacing * (lines - 1));
		}

		public static Rect[] GetMultilinePositionsConfigurable(Rect totalPosition, float verticalSpacing, float horizontalSpacing, params float[] widths)
		{
			var totalWidth = 0f;

			for (var i = 0; i < widths.Length; i++)
			{
				totalWidth += widths[i];
			}

			var lines = Mathf.CeilToInt(totalWidth);

			var availableHeight = totalPosition.height - (verticalSpacing * (lines - 1));

			var lineHeight = availableHeight / lines;

			var positions = new Rect[widths.Length];

			var currentY = 0f;

			var currentItem = 0;

			for (var line = 0; line < lines; line++)
			{
				var lineTotal = 0f;
				var itemsOnLine = 0;

				for (var i = currentItem; i < widths.Length; i++)
				{
					lineTotal += widths[i];

					if (lineTotal > 1)
					{
						break;
					}

					itemsOnLine++;
				}

				var currentX = 0f;

				var availableWidth = totalPosition.width - (horizontalSpacing * (itemsOnLine - 1));

				for (var i = 0; i < itemsOnLine; i++)
				{
					positions[currentItem] = new Rect
					(
						totalPosition.x + currentX,
						totalPosition.y + currentY,
						availableWidth * widths[currentItem],
						lineHeight
					);

					currentX += positions[currentItem].width + horizontalSpacing;
					currentItem++;
				}

				currentY += lineHeight + verticalSpacing;
			}

			return positions;
		}

		#endregion



		#region Events

		// This is basically a simple way of skipping expensive OnGUI calls early
		// https://support.ludiq.io/communities/5/topics/2187-performance-drop

		public static bool ShouldSkip(this Event e)
		{
			// So apparently skipping Ignore is bad, because Unity uses the event in a bunch
			// of controls, so skipping it changes the CID order.
			// https://answers.unity.com/questions/504017/not-redrawing-controls-on-eventtypeignore-results.html
			// https://books.google.ca/books?id=dS-YCgAAQBAJ&pg=PA138&lpg=PA138&dq=ignore+eventtype+unity&source=bl&ots=VoarspYsdc&sig=XSKr1hJqTwctj6zevZzsDmb6RGQ&hl=en&sa=X&ved=2ahUKEwiV2Kf7_djdAhVGPN8KHaBbCoUQ6AEwCHoECAIQAQ#v=onepage&q=ignore%20eventtype%20unity&f=false
			return e.type == EventType.Used; // || e.type == EventType.Ignore;
		}

		public static bool ShouldSkip(this Event e, Rect position)
		{
			// Unfortunately this optim will mess up CID ordering too...
			return e.ShouldSkip(); // || (e.type == EventType.MouseDrag && !position.Contains(e.mousePosition));
		}

		#endregion



		// Unity 2018.1 changelog: https://unity3d.com/unity/beta/unity2018.1.0b12
		// Editor: Plug-in code that creates textures used in rendering with IMGUI 
		// should now avoid specifying them in linear space (i.e. should set the linear 
		// parameter to false in the Texture2D constructor). Otherwise, GUI elements drawn 
		// with such textures may look washed out when the project is working in Linear space 
		// (Player Settings > Color space: Linear). (908904)
		public static bool createLinearTextures => EditorApplicationUtility.unityVersion.major < 2018 && PlayerSettings.colorSpace == ColorSpace.Linear;
	}
}