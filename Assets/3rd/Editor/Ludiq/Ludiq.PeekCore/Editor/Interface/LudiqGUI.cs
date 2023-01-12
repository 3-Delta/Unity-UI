using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ludiq.PeekCore.Bolt;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using GUIEvent = UnityEngine.Event;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class LudiqGUI
	{
		private static GUIEvent e => GUIEvent.current;



		#region Overrides

		public static readonly OverrideStack<Color> color = new OverrideStack<Color>
		(
			() => GUI.color,
			(value) => GUI.color = value
		);

		public static readonly OverrideStack<Color> backgroundColor = new OverrideStack<Color>
		(
			() => GUI.backgroundColor,
			(value) => GUI.backgroundColor = value
		);

		public static readonly OverrideStack<Matrix4x4> matrix = new OverrideStack<Matrix4x4>
		(
			() => GUI.matrix,
			(value) => GUI.matrix = value
		);

		#endregion



		#region Drawing

		private static GUIStyle emptyRect;
		
		private static Material coloredTextureMaterial;

		public static void DrawEmptyRect(Rect position, Color color)
		{
			if (emptyRect == null)
			{
				emptyRect = new GUIStyle();
				emptyRect.normal.background = ColorUtility.CreateBox(ColorPalette.transparent, Color.white);
				emptyRect.normal.scaledBackgrounds = new Texture2D[] { ColorUtility.CreateRetinaBox(ColorPalette.transparent, Color.white) };
				emptyRect.border = new RectOffset(1, 1, 1, 1);
			}

			if (e.type == EventType.Repaint)
			{
				using (LudiqGUI.color.Override(color))
				{
					emptyRect.Draw(position, false, false, false, false);
				}
			}
		}

		public static void DrawTextureColored(Rect position, Texture texture, Color color)
		{
			if (e == null || e.type != EventType.Repaint)
			{
				return;
			}

			if (coloredTextureMaterial == null)
			{
				coloredTextureMaterial = new Material(Shader.Find("Ludiq/Editor/ColoredTexture"));
			}

			coloredTextureMaterial.SetColor("_Color", color);
			Graphics.DrawTexture(position, texture, coloredTextureMaterial);
		}

		#endregion



		#region Windows

		public static void WindowHeader(string label, EditorTexture icon)
		{
			GUILayout.BeginVertical(LudiqStyles.windowHeaderBackground, GUILayout.ExpandWidth(true));
			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			FlexibleSpace();

			if (icon != null)
			{
				GUILayout.Box(new GUIContent(icon[(int)LudiqStyles.windowHeaderIcon.fixedWidth]), LudiqStyles.windowHeaderIcon);
				Space(LudiqStyles.spaceBetweenWindowHeaderIconAndTitle);
			}

			GUILayout.Label(label, LudiqStyles.windowHeaderTitle);
			FlexibleSpace();
			EndHorizontal();
			EndVertical();
		}

		#endregion


		#region Unity Editor Overrides

		public static void EraseComponentTitle(Rect editorPosition)
		{
			EditorGUI.DrawRect(GetComponentTitlePosition(editorPosition), ColorPalette.unityBackgroundMid);
		}

		public static void EraseComponentIcon(Rect editorPosition)
		{
			EditorGUI.DrawRect(GetComponentIconPosition(editorPosition), ColorPalette.unityBackgroundMid);
		}

		public static void DrawCustomComponentTitle(Rect editorPosition, string title)
		{
			EraseComponentTitle(editorPosition);
			GUI.Label(GetComponentTitlePosition(editorPosition), title, LudiqStyles.componentTitle);
		}

		public static string DrawCustomComponentTitleField(Rect editorPosition, string title, string placeholder = null)
		{
			DrawCustomComponentTitle(editorPosition, StringUtility.FallbackWhitespace(title, placeholder));
			return title;
			// Buggy because Unity takes input before we do
			/*
			EraseComponentTitle(editorPosition);

			var position = GetComponentTitlePosition(editorPosition);

			var result = GUI.TextField(position, title, LudiqStyles.componentTitleFieldHidable);

			if (StringUtility.IsNullOrWhiteSpace(title) && placeholder != null)
			{
				GUI.Label(position, placeholder, LudiqStyles.componentTitlePlaceholder);
			}

			return result;
			*/
		}

		public static void DrawCustomComponentTitleField(Rect editorPosition, Accessor titleAccessor, string placeholder = null)
		{
			EditorGUI.BeginChangeCheck();

			var newTitle = DrawCustomComponentTitleField(editorPosition, (string)titleAccessor.value, StringUtility.FallbackWhitespace(placeholder, titleAccessor.label.text));

			if (EditorGUI.EndChangeCheck())
			{
				titleAccessor.RecordUndo();
				titleAccessor.value = newTitle;
			}
		}

		public static void DrawCustomComponentIcon(Rect editorPosition, EditorTexture icon)
		{
			if (icon != null)
			{
				EraseComponentIcon(editorPosition);
				GUI.DrawTexture(GetComponentIconPosition(editorPosition), icon[IconSize.Small]);
			}
		}

		public static void DrawCustomComponentIconField(Rect editorPosition, Accessor iconAccessor, EditorTexture iconPlaceholder)
		{
			DrawCustomComponentIcon(editorPosition, EditorTexture.Single((Texture2D)iconAccessor.value) ?? iconPlaceholder);
			// Buggy because Unity takes input before we do
			/*
			EraseComponentIcon(editorPosition);
			var iconPosition = GetComponentIconPosition(editorPosition);
			ObjectField(iconPosition, iconAccessor, UnityObjectFieldVisualType.Thumbnail, true);
			*/
		}

		public static Rect GetComponentTitlePosition(Rect editorPosition)
		{
			return new Rect
			(
				editorPosition.x + 34,
				editorPosition.y - EditorGUIUtility.singleLineHeight,
				editorPosition.width - 34 - (3 * 16),
				EditorGUIUtility.singleLineHeight
			);
		}

		public static Rect GetComponentIconPosition(Rect editorPosition)
		{
			return new Rect
			(
				editorPosition.x,
				editorPosition.y - IconSize.Small,
				IconSize.Small,
				IconSize.Small
			);
		}

		public static Rect ExpandPosition(Rect editorPosition)
		{
			editorPosition.width = LudiqGUIUtility.currentInspectorWidthWithoutScrollbar - 2;
			editorPosition.x = 0;
			return editorPosition;
		}

		public static Rect GetAssetHeaderPosition(Rect editorPosition)
		{
			var headerPosition = ExpandPosition(editorPosition);
			headerPosition.y -= 54;
			headerPosition.width -= 3 * (16 + 3);
			return headerPosition;
		}
		
		public static void EraseAssetHeader(Rect editorPosition)
		{
			EditorGUI.DrawRect(GetAssetHeaderPosition(editorPosition), ColorPalette.unityBackgroundMid);
		}

		#endregion



		#region Loaders

		public static readonly TextureResolution loaderResolution = new TextureResolution(loaderSize * loaderFrames, loaderSize);

		public const int loaderSize = 24;

		private const int loaderFrames = 12;

		private const float loaderSpeed = 12; // FPS

		private static EditorTexture temporaryLoader;

		public static void Loader(Rect position, Color? color = null)
		{
			EditorTexture loader;

			if (PluginContainer.initialized)
			{
				loader = LudiqCore.Resources.loader;
			}
			else
			{
				if (temporaryLoader == null)
				{
					temporaryLoader = EditorTexture.Load
					(
						new EditorAssetResourceProvider(Path.Combine(Paths.assets, "Ludiq/Ludiq.PeekCore/Editor/Resources")),
						"Loader/Loader.png",
						CreateTextureOptions.PixelPerfect,
						true
					);
				}

				loader = temporaryLoader;
			}

			if (loader != null)
			{
				var frame = (int)((EditorApplication.timeSinceStartup * loaderSpeed) % loaderFrames);
				var uv = new Rect((float)frame / loaderFrames, 0, 1f / loaderFrames, 1);

				using (LudiqGUI.color.Override(color ?? ColorPalette.unityForeground))
				{
					GUI.DrawTextureWithTexCoords(position, loader[loaderSize], uv);
				}
			}
		}

		public static void LoaderLayout(Color? color = null)
		{
			Loader(GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(loaderSize), GUILayout.Height(loaderSize)), color);
		}

		public static void CenterLoader(bool verticalAlign = true)
		{
			if (verticalAlign)
			{
				BeginVertical();
				FlexibleSpace();
			}

			BeginHorizontal();
			FlexibleSpace();
			LoaderLayout();
			FlexibleSpace();
			EndHorizontal();

			if (verticalAlign)
			{
				FlexibleSpace();
				EndVertical();
			}
		}

		#endregion



		#region Fields

		public static float GetTextFieldAdaptiveWidth(object content, float min = 16)
		{
			return Mathf.Max(min, EditorStyles.textField.CalcSize(new GUIContent(content?.ToString())).x + 1);
		}
		
		public static int Spinner(Rect position, bool upEnabled = true, bool downEnabled = true)
		{
			var upPosition = new Rect
			(
				position.x,
				position.y,
				position.width,
				position.height / 2
			);

			var downPosition = new Rect
			(
				position.x,
				position.y + (position.height / 2),
				position.width,
				position.height / 2
			);

			EditorGUI.BeginDisabledGroup(!upEnabled);

			if (GUI.Button(upPosition, GUIContent.none, LudiqStyles.spinnerButton))
			{
				return 1;
			}

			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(!downEnabled);

			if (GUI.Button(downPosition, GUIContent.none, LudiqStyles.spinnerButton))
			{
				return -1;
			}

			EditorGUI.EndDisabledGroup();

			var arrow = LudiqStyles.spinnerDownArrow;

			var upArrowPosition = new Rect
			(
				upPosition.x + ((upPosition.width - arrow.width) / 2),
				(upPosition.y + ((upPosition.height - arrow.height) / 2) + arrow.height) - 1,
				arrow.width,
				-arrow.height
			);

			var downArrowPosition = new Rect
			(
				downPosition.x + ((downPosition.width - arrow.width) / 2),
				downPosition.y + ((downPosition.height - arrow.height) / 2) + 1,
				arrow.width,
				arrow.height
			);

			using (color.Override(upEnabled ? GUI.color : GUI.color.WithAlpha(0.3f)))
			{
				GUI.DrawTexture(upArrowPosition, arrow);
			}

			using (color.Override(downEnabled ? GUI.color : GUI.color.WithAlpha(0.3f)))
			{
				GUI.DrawTexture(downArrowPosition, arrow);
			}

			return 0;
		}

		#endregion



		#region Reflection Fields

		public static float GetTypeFieldAdaptiveWidth(Type type)
		{
			using (LudiqGUIUtility.iconSize.Override(14))
			{
				return Mathf.Max(18, EditorStyles.popup.CalcSize(GetTypeFieldPopupLabel(type)).x + 1);
			}
		}

		public static float GetTypeTreeFieldHeight(TypeTree tree, bool includeSelf = true)
		{
			return EditorGUIUtility.singleLineHeight * Mathf.Max(tree.recursiveNodeCount - (includeSelf ? 0 : 1), 0);
		}

		public static float GetTypeTreeFieldAdaptiveWidth(TypeTree tree, bool includeSelf = true, float indent = 0)
		{
			var width = 0f;

			if (includeSelf)
			{
				width = Mathf.Max(width, GetTypeFieldAdaptiveWidth(tree.type) + indent);
			}

			if (tree.children != null)
			{
				if (includeSelf)
				{
					indent += LudiqStyles.typeTreeIndentation;
				}

				foreach (var child in tree.children)
				{
					var childLabelText = child.displayLabel != null ? child.displayLabel + " " : null;
					var childLabelContent = childLabelText != null ? new GUIContent(childLabelText) : GUIContent.none;

					width = Mathf.Max
					(
						width,
						EditorStyles.label.CalcSize(childLabelContent).x
						+ GetTypeTreeFieldAdaptiveWidth(child, true, indent)
					);
				}

				if (includeSelf)
				{
					indent -= LudiqStyles.typeTreeIndentation;
				}
			}

			return width;
		}

		private static GUIContent GetTypeFieldPopupLabel(Type type, GUIContent nullLabel = null)
		{
			GUIContent popupLabel;

			if ((type != null) && !type.IsGenericParameter)
			{
				var icon = type.Icon();

				if (icon != null)
				{
					var iconTexture = icon[IconSize.Small];

					popupLabel = new GUIContent(type.DisplayName(), iconTexture);
				}
				else
				{
					popupLabel = new GUIContent(type.DisplayName());
				}
			}
			else
			{
				if (nullLabel == null)
				{
					nullLabel = new GUIContent("(No Type)");
				}

				popupLabel = nullLabel;
			}

			if (popupLabel.image != null)
			{
				popupLabel.text = " " + popupLabel.text;
			}

			return popupLabel;
		}

		private static GUIContent GetNamespaceFieldPopupLabel(Namespace @namespace, GUIContent nullLabel = null)
		{
			GUIContent popupLabel;

			if (@namespace != null)
			{
				popupLabel = new GUIContent(@namespace.DisplayName(), @namespace.Icon()[IconSize.Small]);
			}
			else
			{
				if (nullLabel == null)
				{
					nullLabel = new GUIContent("(No Namespace)");
				}

				popupLabel = nullLabel;
			}

			if (popupLabel.image != null)
			{
				popupLabel.text = " " + popupLabel.text;
			}

			return popupLabel;
		}

		private static GUIContent GetAssemblyFieldPopupLabel(LooseAssemblyName assembly, GUIContent nullLabel = null)
		{
			GUIContent popupLabel;

			if (assembly != null)
			{
				popupLabel = new GUIContent(assembly);
			}
			else
			{
				if (nullLabel == null)
				{
					nullLabel = new GUIContent("(No Assembly)");
				}

				popupLabel = nullLabel;
			}

			if (popupLabel.image != null)
			{
				popupLabel.text = " " + popupLabel.text;
			}

			popupLabel.tooltip = popupLabel.text;

			return popupLabel;
		}

		public static Type TypeField(Rect position, GUIContent label, Type type, Func<IFuzzyOptionTree> getOptions, GUIContent nullLabel = null, GUIStyle style = null)
		{
			position = EditorGUI.PrefixLabel(position, label);

			return (Type)FuzzyPopup
			(
				position,
				getOptions,
				type,
				GetTypeFieldPopupLabel(type, nullLabel),
				style
			);
		}

		public static Namespace NamespaceField(Rect position, GUIContent label, Namespace @namespace, Func<IFuzzyOptionTree> getOptions, GUIContent nullLabel = null, GUIStyle style = null)
		{
			position = EditorGUI.PrefixLabel(position, label);

			return (Namespace)FuzzyPopup
			(
				position,
				getOptions,
				@namespace,
				GetNamespaceFieldPopupLabel(@namespace, nullLabel),
				style
			);
		}

		public static LooseAssemblyName AssemblyField(Rect position, GUIContent label, LooseAssemblyName assembly, Func<IFuzzyOptionTree> getOptions, GUIContent nullLabel = null, GUIStyle style = null)
		{
			position = EditorGUI.PrefixLabel(position, label);

			return (LooseAssemblyName)FuzzyPopup
			(
				position,
				getOptions,
				assembly,
				GetAssemblyFieldPopupLabel(assembly, nullLabel),
				style
			);
		}

		public static void TypeTreeField(ref Rect position, GUIContent label, TypeTree tree, bool includeSelf, Func<TypeTree, Func<IFuzzyOptionTree>> getOptions, GUIContent nullLabel = null)
		{
			if (includeSelf)
			{
				var selfPosition = position;
				selfPosition.height = EditorGUIUtility.singleLineHeight;

				using (LudiqGUIUtility.labelWidth.Override(EditorStyles.label.CalcSize(label).x))
				{
					tree.ChangeType(TypeField(selfPosition, label, tree.type, getOptions(tree), nullLabel));
				}

				position.y += selfPosition.height;
			}

			if (tree.children != null)
			{
				if (includeSelf)
				{
					position.x += LudiqStyles.typeTreeIndentation;
					position.width -= LudiqStyles.typeTreeIndentation;
				}

				foreach (var child in tree.children)
				{
					var childLabelText = child.displayLabel != null ? child.displayLabel + " " : null;
					var childLabelContent = childLabelText != null ? new GUIContent(childLabelText) : GUIContent.none;

					TypeTreeField(ref position, childLabelContent, child, true, getOptions, nullLabel);
				}

				if (includeSelf)
				{
					position.width += LudiqStyles.typeTreeIndentation;
					position.x -= LudiqStyles.typeTreeIndentation;
				}
			}
		}

		#endregion



		#region Vector Fields

		public static Vector2 CompactVector2Field(Rect position, GUIContent label, Vector2 value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				(position.width / 2) - ((LudiqStyles.compactHorizontalSpacing) / 1),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 2) - ((LudiqStyles.compactHorizontalSpacing) / 1),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector2
			(
				DraggableFloatField(xPosition, value.x),
				DraggableFloatField(yPosition, value.y)
			);
		}

		public static Vector2Int CompactVector2IntField(Rect position, GUIContent label, Vector2Int value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				(position.width / 2) - ((LudiqStyles.compactHorizontalSpacing) / 1),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 2) - ((LudiqStyles.compactHorizontalSpacing) / 1),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector2Int
			(
				DraggableIntField(xPosition, value.x),
				DraggableIntField(yPosition, value.y)
			);
		}

		public static Vector3 CompactVector3Field(Rect position, GUIContent label, Vector3 value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				(position.width / 3) - ((LudiqStyles.compactHorizontalSpacing) / 2),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 3) - ((LudiqStyles.compactHorizontalSpacing) / 2),
				EditorGUIUtility.singleLineHeight
			);

			var zPosition = new Rect
			(
				yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 3) - ((LudiqStyles.compactHorizontalSpacing) / 2),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector3
			(
				DraggableFloatField(xPosition, value.x),
				DraggableFloatField(yPosition, value.y),
				DraggableFloatField(zPosition, value.z)
			);
		}

		public static Vector3Int CompactVector3IntField(Rect position, GUIContent label, Vector3Int value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				(position.width / 3) - ((LudiqStyles.compactHorizontalSpacing) / 2),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 3) - ((LudiqStyles.compactHorizontalSpacing) / 2),
				EditorGUIUtility.singleLineHeight
			);

			var zPosition = new Rect
			(
				yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 3) - ((LudiqStyles.compactHorizontalSpacing) / 2),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector3Int
			(
				DraggableIntField(xPosition, value.x),
				DraggableIntField(yPosition, value.y),
				DraggableIntField(zPosition, value.z)
			);
		}

		public static Vector4 CompactVector4Field(Rect position, GUIContent label, Vector4 value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				(position.width / 4) - ((LudiqStyles.compactHorizontalSpacing) / 3),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 4) - ((LudiqStyles.compactHorizontalSpacing) / 3),
				EditorGUIUtility.singleLineHeight
			);

			var zPosition = new Rect
			(
				yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 4) - ((LudiqStyles.compactHorizontalSpacing) / 3),
				EditorGUIUtility.singleLineHeight
			);

			var wPosition = new Rect
			(
				zPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				(position.width / 4) - ((LudiqStyles.compactHorizontalSpacing) / 3),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector4
			(
				DraggableFloatField(xPosition, value.x),
				DraggableFloatField(yPosition, value.y),
				DraggableFloatField(zPosition, value.z),
				DraggableFloatField(wPosition, value.w)
			);
		}

		public static Vector2 AdaptiveVector2Field(Rect position, GUIContent label, Vector2 value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				GetTextFieldAdaptiveWidth(value.x),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.y),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector2
			(
				DraggableFloatField(xPosition, value.x),
				DraggableFloatField(yPosition, value.y)
			);
		}

		public static Vector2Int AdaptiveVector2IntField(Rect position, GUIContent label, Vector2Int value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				GetTextFieldAdaptiveWidth(value.x),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.y),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector2Int
			(
				DraggableIntField(xPosition, value.x),
				DraggableIntField(yPosition, value.y)
			);
		}

		public static Vector3 AdaptiveVector3Field(Rect position, GUIContent label, Vector3 value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				GetTextFieldAdaptiveWidth(value.x),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.y),
				EditorGUIUtility.singleLineHeight
			);

			var zPosition = new Rect
			(
				yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.z),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector3
			(
				DraggableFloatField(xPosition, value.x),
				DraggableFloatField(yPosition, value.y),
				DraggableFloatField(zPosition, value.z)
			);
		}

		public static Vector3Int AdaptiveVector3IntField(Rect position, GUIContent label, Vector3Int value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				GetTextFieldAdaptiveWidth(value.x),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.y),
				EditorGUIUtility.singleLineHeight
			);

			var zPosition = new Rect
			(
				yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.z),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector3Int
			(
				DraggableIntField(xPosition, value.x),
				DraggableIntField(yPosition, value.y),
				DraggableIntField(zPosition, value.z)
			);
		}

		public static Vector4 AdaptiveVector4Field(Rect position, GUIContent label, Vector4 value)
		{
			position = EditorGUI.PrefixLabel(position, label);

			var xPosition = new Rect
			(
				position.x,
				position.y,
				GetTextFieldAdaptiveWidth(value.x),
				EditorGUIUtility.singleLineHeight
			);

			var yPosition = new Rect
			(
				xPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.y),
				EditorGUIUtility.singleLineHeight
			);

			var zPosition = new Rect
			(
				yPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.z),
				EditorGUIUtility.singleLineHeight
			);

			var wPosition = new Rect
			(
				zPosition.xMax + LudiqStyles.compactHorizontalSpacing,
				position.y,
				GetTextFieldAdaptiveWidth(value.w),
				EditorGUIUtility.singleLineHeight
			);

			return new Vector4
			(
				DraggableFloatField(xPosition, value.x),
				DraggableFloatField(yPosition, value.y),
				DraggableFloatField(zPosition, value.z),
				DraggableFloatField(wPosition, value.w)
			);
		}

		public static GUIContent GetEnumPopupContent(Enum value)
		{
			Ensure.That(nameof(value)).IsNotNull(value);

			if (EditorGUI.showMixedValue)
			{
				return new GUIContent("Mixed ...");
			}

			var enumType = value.GetType();

			if (enumType.IsPseudoFlagsEnum())
			{
				var mask = Convert.ToInt64(value);

				if (mask == 0)
				{
					return new GUIContent("None");
				}
				else if (mask == ~0)
				{
					return new GUIContent("Everything");
				}

				var flags = Enum.GetValues(enumType).Cast<Enum>().ToArray();

				var activeFlagsCount = 0;

				foreach (var flag in flags)
				{
					if (value.HasFlag(flag))
					{
						activeFlagsCount++;
					}
				}

				if (activeFlagsCount == 0)
				{
					return new GUIContent("None");
				}
				else if (activeFlagsCount == 1)
				{
					return new GUIContent(value.ToString().Prettify());
				}
				else if (activeFlagsCount == flags.Length)
				{
					return new GUIContent("Everything");
				}
				else
				{
					return new GUIContent("Mixed ...");
				}
			}
			else
			{
				return new GUIContent(value.ToString().Prettify());
			}
		}
		
		#endregion



		#region Object Field
		
		private static readonly int fieldHash = nameof(UnityObjectInspector).GetHashCode();

		public static float GetFuzzyObjectFieldHeight(UnityObjectFieldVisualType visualType, float width)
		{
			switch (visualType)
			{
				case UnityObjectFieldVisualType.NameAndTarget:
				case UnityObjectFieldVisualType.Target:
					return EditorGUIUtility.singleLineHeight;

				case UnityObjectFieldVisualType.Thumbnail:
					return width;

				default: throw visualType.Unexpected();
			}
		}

		public static float GetFuzzyObjectFieldWidth(UnityObject obj, UnityObjectFieldVisualType visualType)
		{
			switch (visualType)
			{
				case UnityObjectFieldVisualType.NameAndTarget:
				{
					string label;
					bool hasIcon;

					if (obj == null)
					{
						label = "(None)";
						hasIcon = false;
					}
					else
					{
						label = obj.name;
						hasIcon = true;
					}

					var width = EditorStyles.objectField.CalcSize(new GUIContent(label)).x;

					if (hasIcon)
					{
						width += 15;
					}

					return width;
				}

				case UnityObjectFieldVisualType.Target:
					return LudiqStyles.objectFieldTarget.fixedWidth;

				case UnityObjectFieldVisualType.Thumbnail:
					return EditorGUIUtility.singleLineHeight; // ?

				default: throw visualType.Unexpected();
			}
		}

		private static readonly GUIContent objectFieldContent = new GUIContent();

		public static void ObjectField
		(
			Rect position, 
			Accessor accessor,
			UnityObjectFieldVisualType visualType = UnityObjectFieldVisualType.NameAndTarget,
			bool hidableFrame = false, 
			Texture2D thumbnailPlaceholder = null)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = ObjectField
			(
				position, 
				(UnityObject)accessor.value,
				accessor.definedType, 
				accessor.serializedObject?.AsGameObject()?.scene,
				visualType,
				hidableFrame,
				thumbnailPlaceholder
			);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		public static UnityObject ObjectField
		(
			Rect position,
			UnityObject obj,
			Type objType,
			Scene? scene,
			UnityObjectFieldVisualType visualType = UnityObjectFieldVisualType.NameAndTarget,
			bool hidableFrame = false,
			Texture2D thumbnailPlaceholder = null
		)
		{
			return ObjectField
			(
				position, 
				obj,
				objType, 
				scene,
				true, 
				objType.DisplayName(), 
				objType.Icon(), 
				null, 
				null,
				visualType,
				hidableFrame,
				thumbnailPlaceholder
			);
		}

		public static UnityObject ObjectField
		(
			Rect position, 
			UnityObject obj,
			Type objType,
			Scene? scene, 
			bool allowAssetObjects, 
			string typeLabel,
			EditorTexture typeIcon,
			Func<UnityObject, bool> filter = null, 
			Func<IFuzzyOptionTree> getOptions = null,
			UnityObjectFieldVisualType visualType = UnityObjectFieldVisualType.NameAndTarget,
			bool hidableFrame = false,
			Texture2D thumbnailPlaceholder = null
		)
		{
			var controlID = GUIUtility.GetControlID(fieldHash, FocusType.Keyboard, position);
			var eventType = e.type;

			// Make sure the immediate mode handling happens
			var newValue = (UnityObject)FuzzyPopupRaw(controlID, false, position, null, obj);

			// Ping/select objects even when the object field is disabled
			if (!GUI.enabled && e.rawType == EventType.MouseDown)
			{
				eventType = e.rawType;
			}

			// Fit the icon in the field
			LudiqGUIUtility.iconSize.BeginOverride(12);

			switch (eventType)
			{
				case EventType.DragExited:
				{
					if (GUI.enabled)
					{
						HandleUtility.Repaint();
					}

					break;
				}

				case EventType.DragUpdated:
				case EventType.DragPerform:
				{
					if (position.Contains(e.mousePosition) && GUI.enabled)
					{
						var references = DragAndDrop.objectReferences;

						var validatedObject = references.OfType(objType).FirstOrDefault();

						if (validatedObject != null)
						{
							if (validatedObject.IsSceneBound() && validatedObject.GameObject().scene != scene)
							{
								validatedObject = null;
							}
							else if (filter != null && !filter(validatedObject))
							{
								validatedObject = null;
							}
						}

						if (validatedObject != null)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

							if (eventType == EventType.DragPerform)
							{
								newValue = validatedObject;

								GUI.changed = true;
								DragAndDrop.AcceptDrag();
								DragAndDrop.activeControlID = 0;
							}
							else
							{
								DragAndDrop.activeControlID = controlID;
							}

							e.Use();
						}
					}

					break;
				}

				case EventType.MouseDown:
				{
					// Ignore right clicks
					if (e.button != (int)MouseButton.Left)
					{
						break;
					}

					if (position.Contains(e.mousePosition))
					{
						// Get button rect for Object Selector
						Rect targetSelectorPosition;

						switch (visualType)
						{
							case UnityObjectFieldVisualType.NameAndTarget:
							{
								targetSelectorPosition = new Rect(position.xMax - 15, position.y, 15, position.height);
								break;
							}

							case UnityObjectFieldVisualType.Thumbnail:
							{
								targetSelectorPosition = new Rect(position.xMax - 15, position.yMax - 15, 15, 15);
								break;
							}

							case UnityObjectFieldVisualType.Target:
							{
								targetSelectorPosition = position;
								break;
							}

							default: throw visualType.Unexpected();
						}

						EditorGUIUtility.editingTextField = false;
						
						// Clicking the target selector
						if (targetSelectorPosition.Contains(e.mousePosition))
						{
							if (GUI.enabled)
							{
								GUIUtility.keyboardControl = controlID;

								if (getOptions == null)
								{
									getOptions = () => new UnityObjectOptionTree
									(
										objType,
										typeLabel,
										typeIcon,
										scene,
										allowAssetObjects,
										filter,
										typeLabel
									);
								}

								newValue = (UnityObject)FuzzyPopupRaw(controlID, true, position, getOptions, obj);
								e.Use();
							}
						}
						// Clicking the field
						else
						{
							var actualTargetObject = newValue;

							var isComponent = actualTargetObject as Component;

							if (isComponent)
							{
								actualTargetObject = isComponent.gameObject;
							}

							// Single click pings the object
							if (e.clickCount == 1)
							{
								GUIUtility.keyboardControl = controlID;
								EditorGUIUtility.PingObject(actualTargetObject);
								e.Use();
							}
							// Double click opens the asset in external app or changes selection to referenced object
							else if (e.clickCount == 2)
							{
								if (actualTargetObject)
								{
									AssetDatabase.OpenAsset(actualTargetObject);
									GUIUtility.ExitGUI();
								}

								e.Use();
							}
						}
					}

					break;
				}

				case EventType.KeyDown:
				{
					if (GUIUtility.keyboardControl == controlID)
					{
						if (e.keyCode == KeyCode.Backspace || (e.keyCode == KeyCode.Delete && (e.modifiers & EventModifiers.Shift) == 0))
						{
							newValue = null;
							GUI.changed = true;
							e.Use();
						}
					}

					break;
				}

				case EventType.Repaint:
				{
					objectFieldContent.text = $"{(obj == null ? "None" : obj.name)} ({typeLabel})";
					objectFieldContent.image = (obj == null ? typeIcon : obj.Icon())?[IconSize.Small];
					
					switch (visualType)
					{
						case UnityObjectFieldVisualType.NameAndTarget:
						{
							EditorStyles.objectField.Draw(position, objectFieldContent, controlID, DragAndDrop.activeControlID == controlID);
							break;
						}
						
						case UnityObjectFieldVisualType.Thumbnail:
						{
							ObjectFieldThumbnail(position, controlID, newValue, objectFieldContent, hidableFrame, thumbnailPlaceholder);
							break;
						}

						case UnityObjectFieldVisualType.Target:
						{
							position.x++;
							position.width--;
							position.y-=2;
							position.height+=2;
							LudiqStyles.objectFieldTarget.Draw(position, GUIContent.none, controlID,  DragAndDrop.activeControlID == controlID);
							break;
						}
						
						default: throw visualType.Unexpected();
					}

					break;
				}
			}

			LudiqGUIUtility.iconSize.EndOverride();

			return newValue;
		}

		private static void ObjectFieldThumbnail(Rect position, int controlID, UnityObject obj, GUIContent content, bool hidableFrame, Texture2D thumbnailPlaceholder)
		{
			var outerRect = position.ExpandBy(1);
			var innerRect = position.ShrinkBy(0);

			var isHover = position.Contains(e.mousePosition);
			var isDragging = DragAndDrop.activeControlID == controlID;
			var hasKeyboardFocus = GUIUtility.keyboardControl == controlID;
			
			var drawFrame = !hidableFrame || isHover || (obj == null && thumbnailPlaceholder == null);

			if (drawFrame)
			{
				LudiqStyles.objectFieldThumbnailBackground.Draw(outerRect, false, false, false, hasKeyboardFocus || isDragging);
			}
			
			var thumbnail = obj != null ? (content.image as Texture2D ?? thumbnailPlaceholder) : thumbnailPlaceholder;

			if (drawFrame)
			{
				if (thumbnail == null)
				{
					UnityEditorDynamic.EditorGUI.DrawTransparencyCheckerTexture(innerRect, ScaleMode.StretchToFill, innerRect.width / innerRect.height);
				}
				else if (thumbnail.alphaIsTransparency)
				{
					UnityEditorDynamic.EditorGUI.DrawTextureTransparent(innerRect, thumbnail);
				}
				else
				{
					UnityEditorDynamic.EditorGUI.DrawPreviewTexture(innerRect, content.image);
				}

				if (hasKeyboardFocus || isDragging || isHover)
				{
					position.y++;
				}
			
				LudiqStyles.objectFieldThumbnailForeground.Draw(position, false, false, false, hasKeyboardFocus || isDragging || isHover);
			}
			else
			{
				if (thumbnail != null)
				{
					GUI.DrawTexture(innerRect, thumbnail);
				}
			}
		}



		#endregion



		#region Number Dragging

		// Lots of re-implementation from internal EditorGUI methods to allow us custom control trigger

		private static double CalculateDragSensitivityContinuous(double value)
		{
			if (double.IsInfinity(value) || double.IsNaN(value))
			{
				return 0.0;
			}

			return Math.Max(1.0, Math.Pow(Math.Abs(value), 0.5)) * 0.0299999993294477;
		}

		private static long CalculateDragSensitivityDiscrete(long value)
		{
			return (long)Math.Max(1.0, Math.Pow(Math.Abs((double)value), 0.5) * 0.0299999993294477);
		}

		private static NumberDragState numberDragState = NumberDragState.NotDragging;

		private static double numberDragStartValueContinuous;

		private static long numberDragStartValueDiscrete;

		private static Vector2 numberDragStartPosition;

		private static double numberDragSensitivity;

		private const float numberDragDeadZone = 16;

		private static readonly int numberDragControlIDHint = "DraggableFieldOverlay".GetHashCode();

		private enum NumberDragState
		{
			NotDragging,

			RequestedDragging,

			Dragging
		}

		public static float DraggableFloatField(Rect position, float value)
		{
			var controlId = GUIUtility.GetControlID(numberDragControlIDHint, FocusType.Passive, position);

			if (e.shift)
			{
				value = DragNumber(position, true, controlId, value);
			}

			return EditorGUI.FloatField(position, value);
		}

		public static int DraggableIntField(Rect position, int value)
		{
			var controlId = GUIUtility.GetControlID(numberDragControlIDHint, FocusType.Passive, position);

			if (e.shift)
			{
				value = DragNumber(position, true, controlId, value);
			}

			return EditorGUI.IntField(position, value);
		}

		public static float DragNumber(Rect hotZone, bool deadZone, int controlId, float value)
		{
			double continuousValue = value;
			long discreteValue = 0;
			DragNumber(hotZone, deadZone, controlId, true, ref continuousValue, ref discreteValue);
			return (float)continuousValue;
		}

		public static int DragNumber(Rect hotZone, bool deadZone, int controlId, int value)
		{
			double continuousValue = 0;
			long discreteValue = value;
			DragNumber(hotZone, deadZone, controlId, false, ref continuousValue, ref discreteValue);
			return (int)discreteValue;
		}

		private static void DragNumber(Rect hotZone, bool deadZone, int controlId, bool isContinuous, ref double continuousValue, ref long discreteValue)
		{
			var e = GUIEvent.current;

			switch (e.GetTypeForControl(controlId))
			{
				case EventType.MouseDown:

				{
					if (!hotZone.Contains(e.mousePosition) || (e.button != (int)MouseButton.Left))
					{
						break;
					}

					EditorGUIUtility.editingTextField = false;
					GUIUtility.hotControl = controlId;
					GUIUtility.keyboardControl = controlId;

					if (deadZone)
					{
						numberDragState = NumberDragState.RequestedDragging;
					}
					else
					{
						numberDragState = NumberDragState.Dragging;
					}

					numberDragStartValueContinuous = continuousValue;
					numberDragStartValueDiscrete = discreteValue;
					numberDragStartPosition = e.mousePosition;

					if (isContinuous)
					{
						numberDragSensitivity = CalculateDragSensitivityContinuous(continuousValue);
					}
					else
					{
						numberDragSensitivity = CalculateDragSensitivityDiscrete(discreteValue);
					}

					e.Use();
					EditorGUIUtility.SetWantsMouseJumping(1);
					break;
				}

				case EventType.MouseUp:

				{
					if ((GUIUtility.hotControl != controlId) || (numberDragState == NumberDragState.NotDragging))
					{
						break;
					}

					GUIUtility.hotControl = 0;
					numberDragState = NumberDragState.NotDragging;
					e.Use();
					EditorGUIUtility.SetWantsMouseJumping(0);
					break;
				}

				case EventType.MouseDrag:

				{
					if (GUIUtility.hotControl != controlId)
					{
						break;
					}

					switch (numberDragState)
					{
						case NumberDragState.RequestedDragging:

						{
							if ((e.mousePosition - numberDragStartPosition).sqrMagnitude > numberDragDeadZone)
							{
								numberDragState = NumberDragState.Dragging;
								GUIUtility.keyboardControl = controlId;
							}

							e.Use();
							break;
						}

						case NumberDragState.Dragging:

						{
							if (isContinuous)
							{
								continuousValue = continuousValue + (HandleUtility.niceMouseDelta * numberDragSensitivity);
								continuousValue = RoundBasedOnMinimumDifference(continuousValue, numberDragSensitivity);
							}
							else
							{
								discreteValue = discreteValue + (long)Math.Round(HandleUtility.niceMouseDelta * numberDragSensitivity);
							}

							GUI.changed = true;
							e.Use();
							break;
						}
					}

					break;
				}

				case EventType.KeyDown:

				{
					if ((GUIUtility.hotControl != controlId) || (e.keyCode != KeyCode.Escape) || (numberDragState == NumberDragState.NotDragging))
					{
						break;
					}

					continuousValue = numberDragStartValueContinuous;
					discreteValue = numberDragStartValueDiscrete;
					GUI.changed = true;
					GUIUtility.hotControl = 0;
					e.Use();
					break;
				}

				case EventType.Repaint:

				{
					EditorGUIUtility.AddCursorRect(hotZone, MouseCursor.SlideArrow);
					break;
				}
			}
		}

		private static double DiscardLeastSignificantDecimal(double v)
		{
			var digits = Math.Max(0, (int)(5d - Math.Log10(Math.Abs(v))));

			try
			{
				return Math.Round(v, digits);
			}
			catch (ArgumentOutOfRangeException)
			{
				return 0d;
			}
		}

		private static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
		{
			return (int)Math.Max(0d, -Math.Floor(Math.Log10(Math.Abs(minDifference))));
		}

		private static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
		{
			if (minDifference == 0d)
			{
				return DiscardLeastSignificantDecimal(valueToRound);
			}

			return Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
		}

		#endregion



		#region Big Buttons

		public static bool BigButton(Rect buttonPosition, GUIContent content)
		{
			var style = LudiqStyles.bigButton;
			var iconSize = LudiqStyles.bigButtonIconSize;

			var iconPosition = Rect.zero;
			var x = buttonPosition.x + style.padding.left;
			var width = buttonPosition.width - style.padding.left - style.padding.right;

			if (content.image != null)
			{
				iconPosition = new Rect
				(
					x,
					buttonPosition.y + style.padding.right,
					iconSize,
					iconSize
				);

				x += iconSize + LudiqStyles.spaceAfterBigButtonIcon;
				width -= iconSize + LudiqStyles.spaceAfterBigButtonIcon;
			}

			var titlePosition = new Rect
			(
				x,
				buttonPosition.y + style.padding.top,
				width,
				LudiqStyles.bigButtonTitle.fixedHeight
			);

			var subtitlePosition = new Rect
			(
				titlePosition.x,
				titlePosition.yMax,
				titlePosition.width,
				buttonPosition.height - style.padding.top - style.padding.bottom - titlePosition.height
			);

			var result = GUI.Button(buttonPosition, GUIContent.none);
			GUI.Label(titlePosition, content.text, LudiqStyles.bigButtonTitle);
			GUI.Label(subtitlePosition, content.tooltip, LudiqStyles.bigButtonSubtitle);

			if (content.image != null)
			{
				GUI.DrawTexture(iconPosition, content.image);
			}

			return result;
		}

		public static bool BigButtonLayout(GUIContent content)
		{
			var buttonPosition = GUILayoutUtility.GetRect(GUIContent.none, LudiqStyles.bigButton, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			return BigButton(buttonPosition, content);
		}

		#endregion



		#region Headers

		public delegate float GetHeaderTitleHeightDelegate(float innerWidth);

		public delegate float GetHeaderSummaryHeightDelegate(float innerWidth);

		public delegate void OnHeaderTitleGUIDelegate(Rect titlePosition);

		public delegate void OnHeaderSummaryGUIDelegate(Rect summaryPosition);

		public delegate void OnIconGUIDelegate(Rect iconPosition);

		public static float GetHeaderHeight
		(
			GetHeaderTitleHeightDelegate getTitleHeight,
			GetHeaderSummaryHeightDelegate getSummaryHeight,
			bool hasIcon,
			float totalWidth,
			bool bottomMargin = true,
			float spaceBetweenTitleAndSummary = 0
		)
		{
			var innerWidth = GetHeaderInnerWidth(totalWidth, hasIcon);

			var height = 0f;

			height += LudiqStyles.headerBackground.padding.top;

			var innerHeight = 0f;

			innerHeight += getTitleHeight(innerWidth);

			var summaryHeight = getSummaryHeight(innerWidth);

			if (summaryHeight > 0)
			{
				innerHeight += spaceBetweenTitleAndSummary;
				innerHeight += summaryHeight;
			}

			if (hasIcon)
			{
				innerHeight = Mathf.Max(innerHeight, LudiqStyles.headerIcon.fixedHeight + LudiqStyles.headerIcon.margin.top);
			}

			height += innerHeight;

			height += LudiqStyles.headerBackground.padding.bottom;

			if (bottomMargin)
			{
				height += LudiqStyles.headerBackground.margin.bottom;
			}

			return height;
		}

		public static void OnHeaderGUI
		(
			GetHeaderTitleHeightDelegate getTitleHeight,
			GetHeaderSummaryHeightDelegate getSummaryHeight,
			OnHeaderTitleGUIDelegate onTitleGUI,
			OnHeaderSummaryGUIDelegate onSummaryGui,
			OnIconGUIDelegate onIconGUI,
			Rect position,
			ref float y
		)
		{
			var hasIcon = onIconGUI != null;

			var innerWidth = GetHeaderInnerWidth(position.width, hasIcon);
			var x = position.x;

			var headerPosition = new Rect
			(
				x,
				y,
				position.width,
				GetHeaderHeight(getTitleHeight, getSummaryHeight, hasIcon, position.width, false)
			);

			if (e.type == EventType.Repaint)
			{
				LudiqStyles.headerBackground.Draw(headerPosition, false, false, false, false);
			}

			x += LudiqStyles.headerBackground.padding.left;

			if (hasIcon)
			{
				var iconPosition = new Rect
				(
					x,
					y + LudiqStyles.headerBackground.padding.top + LudiqStyles.headerIcon.margin.top,
					LudiqStyles.headerIcon.fixedWidth,
					LudiqStyles.headerIcon.fixedHeight
				);

				onIconGUI(iconPosition);

				x += iconPosition.width + LudiqStyles.headerIcon.margin.right;
			}

			var titlePosition = new Rect
			(
				x,
				y + LudiqStyles.headerBackground.padding.top,
				innerWidth,
				getTitleHeight(innerWidth)
			);

			onTitleGUI(titlePosition);

			var summaryHeight = getSummaryHeight(innerWidth);

			if (summaryHeight > 0)
			{
				var summaryPosition = new Rect
				(
					x,
					titlePosition.yMax + EditorGUIUtility.standardVerticalSpacing,
					innerWidth,
					summaryHeight
				);

				onSummaryGui(summaryPosition);
			}

			y = headerPosition.yMax;
		}

		private static float GetHeaderInnerWidth(float totalWidth, bool hasIcon)
		{
			var width = totalWidth;

			width -= LudiqStyles.headerBackground.padding.left;
			width -= LudiqStyles.headerBackground.padding.right;

			if (hasIcon)
			{
				width -= LudiqStyles.headerIcon.fixedWidth;
				width -= LudiqStyles.headerIcon.margin.right;
			}

			return width;
		}



		#region Static

		public static float GetHeaderHeight(GUIContent header, float totalWidth)
		{
			return GetHeaderHeight
			(
				header.text,
				header.tooltip,
				EditorTexture.Single(header.image),
				totalWidth
			);
		}

		public static float GetHeaderHeight(string title, string summary, EditorTexture icon, float totalWidth)
		{
			return GetHeaderHeight
			(
				innerWidth => GetHeaderTitleHeight(title, innerWidth),
				innerWidth => GetHeaderSummaryHeight(summary, innerWidth),
				icon != null,
				totalWidth
			);
		}

		public static void OnHeaderGUI(GUIContent header, Rect position, ref float y)
		{
			OnHeaderGUI
			(
				header.text,
				header.tooltip,
				EditorTexture.Single(header.image),
				position,
				ref y
			);
		}

		public static void OnHeaderGUI(string title, string summary, EditorTexture icon, Rect position, ref float y)
		{
			OnHeaderGUI
			(
				innerWidth => GetHeaderTitleHeight(title, innerWidth),
				innerWidth => GetHeaderSummaryHeight(summary, innerWidth),
				titlePosition => OnHeaderTitleGUI(title, titlePosition),
				summaryPosition => OnHeaderSummaryGUI(summary, summaryPosition),
				icon != null ? iconPosition => OnHeaderIconGUI(icon, iconPosition) : (OnIconGUIDelegate)null,
				position,
				ref y
			);
		}

		private static readonly GUIContent tempHeaderTitleContent = new GUIContent();

		private static readonly GUIContent tempHeaderSummaryContent = new GUIContent();

		public static float GetHeaderTitleHeight(string title, float width)
		{
			tempHeaderTitleContent.text = title;
			return LudiqStyles.headerTitle.CalcHeight(tempHeaderTitleContent, width);
		}

		public static float GetHeaderSummaryHeight(string summary, float width)
		{
			if (StringUtility.IsNullOrWhiteSpace(summary))
			{
				return 0;
			}

			tempHeaderSummaryContent.text = summary;
			return LudiqStyles.headerSummary.CalcHeight(tempHeaderSummaryContent, width);
		}

		public static void OnHeaderTitleGUI(string title, Rect titlePosition)
		{
			EditorGUI.LabelField(titlePosition, title, LudiqStyles.headerTitle);
		}

		public static void OnHeaderSummaryGUI(string summary, Rect summaryPosition)
		{
			EditorGUI.LabelField(summaryPosition, summary, LudiqStyles.headerSummary);
		}

		public static void OnHeaderIconGUI(EditorTexture icon, Rect iconPosition)
		{
			if (icon == null)
			{
				return;
			}

			GUI.DrawTexture(iconPosition, icon[IconSize.Medium]);
		}

		#endregion



		#region Editable

		public static float GetHeaderHeight(Inspector parentInspector, Accessor titleAccessor, Accessor summaryAccessor, EditorTexture icon, float totalWidth)
		{
			return GetHeaderHeight
			(
				innerWidth => GetHeaderTitleHeight(parentInspector, titleAccessor, innerWidth),
				innerWidth => GetHeaderSummaryHeight(parentInspector, summaryAccessor, innerWidth),
				icon != null,
				totalWidth
			);
		}

		public static float GetHeaderHeight(Inspector parentInspector, Accessor titleAccessor, Accessor summaryAccessor, Accessor iconAccessor, float totalWidth)
		{
			return GetHeaderHeight
			(
				innerWidth => GetHeaderTitleHeight(parentInspector, titleAccessor, innerWidth),
				innerWidth => GetHeaderSummaryHeight(parentInspector, summaryAccessor, innerWidth),
				iconAccessor != null,
				totalWidth
			);
		}

		public static void OnHeaderGUI(Accessor titleAccessor, Accessor summaryAccessor, EditorTexture icon, Rect position, ref float y)
		{
			OnHeaderGUI
			(
				innerWidth => GetHeaderTitleHeight(null, titleAccessor, innerWidth),
				innerWidth => GetHeaderSummaryHeight(null, summaryAccessor, innerWidth),
				titlePosition => OnHeaderTitleGUI(titleAccessor, titlePosition),
				summaryPosition => OnHeaderSummaryGUI(summaryAccessor, summaryPosition),
				iconPosition => OnHeaderIconGUI(icon, iconPosition),
				position,
				ref y
			);
		}

		public static void OnHeaderGUI(Accessor titleAccessor, Accessor summaryAccessor, Accessor iconAccessor, Rect position, ref float y)
		{
			OnHeaderGUI
			(
				innerWidth => GetHeaderTitleHeight(null, titleAccessor, innerWidth),
				innerWidth => GetHeaderSummaryHeight(null, summaryAccessor, innerWidth),
				titlePosition => OnHeaderTitleGUI(titleAccessor, titlePosition),
				summaryPosition => OnHeaderSummaryGUI(summaryAccessor, summaryPosition),
				iconAccessor != null ? iconPosition => OnHeaderIconGUI(iconAccessor, iconPosition) : (OnIconGUIDelegate)null,
				position,
				ref y
			);
		}

		public static float GetHeaderTitleHeight(Inspector parentInspector, Accessor titleAccessor, float width)
		{
			return LudiqStyles.headerTitleField.fixedHeight;
		}

		public static float GetHeaderSummaryHeight(Inspector parentInspector, Accessor summaryAccessor, float width)
		{
			var attribute = summaryAccessor.GetAttribute<InspectorTextAreaAttribute>();

			var height = LudiqStyles.textAreaWordWrapped.CalcHeight(new GUIContent((string)summaryAccessor.value), width);

			if (attribute?.hasMinLines ?? false)
			{
				var minHeight = (EditorStyles.textArea.lineHeight * attribute.minLines) + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

				height = Mathf.Max(height, minHeight);
			}

			if (attribute?.hasMaxLines ?? false)
			{
				var maxHeight = (EditorStyles.textArea.lineHeight * attribute.maxLines) + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

				height = Mathf.Min(height, maxHeight);
			}

			return height;
		}

		public static void OnHeaderTitleGUI(Accessor titleAccessor, Rect titlePosition, string placeholder = null)
		{
			Inspector.BeginBlock(titleAccessor, titlePosition);

			var hidable = !StringUtility.IsNullOrWhiteSpace((string)titleAccessor.value);

			GUI.SetNextControlName(titleAccessor.path);

			var newTitle = EditorGUI.TextField(titlePosition, (string)titleAccessor.value, hidable ? LudiqStyles.headerTitleFieldHidable : LudiqStyles.headerTitleField);

			if (Inspector.EndBlock(titleAccessor))
			{
				titleAccessor.RecordUndo();
				titleAccessor.value = newTitle;
			}

			if (string.IsNullOrEmpty(newTitle))
			{
				GUI.Label(titlePosition, StringUtility.FallbackWhitespace(placeholder, $"({titleAccessor.label.text})"), LudiqStyles.headerTitlePlaceholder);
			}
		}

		public static void OnHeaderSummaryGUI(Accessor summaryAccessor, Rect summaryPosition)
		{
			Inspector.BeginBlock(summaryAccessor, summaryPosition);

			var hidable = !StringUtility.IsNullOrWhiteSpace((string)summaryAccessor.value);

			GUI.SetNextControlName(summaryAccessor.path);

			var newTitle = EditorGUI.TextField(summaryPosition, (string)summaryAccessor.value, hidable ? LudiqStyles.headerSummaryFieldHidable : LudiqStyles.headerSummaryField);

			if (Inspector.EndBlock(summaryAccessor))
			{
				summaryAccessor.RecordUndo();
				summaryAccessor.value = newTitle;
			}

			if (string.IsNullOrEmpty((string)summaryAccessor.value))
			{
				GUI.Label(summaryPosition, $"({summaryAccessor.label.text})", LudiqStyles.headerSummaryPlaceholder);
			}
		}

		public static void OnHeaderIconGUI(Accessor iconAccessor, Rect iconPosition, EditorTexture iconPlaceholder = null)
		{
			ObjectField(iconPosition, iconAccessor, UnityObjectFieldVisualType.Thumbnail, true, iconPlaceholder?[IconSize.Medium]);
		}

		#endregion

		#endregion



		#region Version Mismatch

		private const string VersionMismatchMessage = "Inspectors are disabled when plugin versions mismatch to prevent data corruption. ";

		public static float GetVersionMismatchShieldHeight(float width)
		{
			var height = 0f;

			height += LudiqGUIUtility.GetHelpBoxHeight(VersionMismatchMessage, MessageType.Warning, width);
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUIUtility.singleLineHeight;

			return height;
		}

		public static void VersionMismatchShield(Rect position)
		{
			var warningPosition = new Rect
			(
				position.x,
				position.y,
				position.width,
				LudiqGUIUtility.GetHelpBoxHeight(VersionMismatchMessage, MessageType.Warning, position.width)
			);

			var buttonPosition = new Rect
			(
				position.x,
				warningPosition.yMax + EditorGUIUtility.standardVerticalSpacing,
				position.width,
				EditorGUIUtility.singleLineHeight
			);

			EditorGUI.HelpBox(warningPosition, VersionMismatchMessage, MessageType.Warning);

			if (GUI.Button(buttonPosition, "Update Wizard"))
			{
				UpdateWizard.Show();
			}
		}

		public static void VersionMismatchShieldLayout()
		{
			BeginVertical();

			EditorGUILayout.HelpBox(VersionMismatchMessage, MessageType.Warning);

			if (GUILayout.Button("Update Wizard"))
			{
				UpdateWizard.Show();
			}

			EndVertical();
		}

		#endregion



		#region Lists

		private static float CalculateListWidth(IList<ListOption> options)
		{
			var width = 0f;

			foreach (var option in options)
			{
				width = Mathf.Max(width, LudiqStyles.listRow.CalcSize(ListOptionContent(option.label)).x);
			}
			
			return width + LudiqGUIUtility.scrollBarWidth;
		}

		public static Vector2 List(Vector2 scroll, bool flexible, IList<ListOption> options, object selected, Action<object> selectionChanged)
		{
			var selectedIndex = options.IndexOf(options.FirstOrDefault(o => Equals(o.value, selected)));

			if (e.type == EventType.KeyDown)
			{
				if (e.keyCode == KeyCode.DownArrow)
				{
					selectionChanged(options[Mathf.Min(options.Count - 1, selectedIndex + 1)].value);
					e.Use();
				}
				else if (e.keyCode == KeyCode.UpArrow)
				{
					selectionChanged(options[Mathf.Max(0, selectedIndex - 1)].value);
					e.Use();
				}
			}

			var newScroll = EditorGUILayout.BeginScrollView(scroll, LudiqStyles.listBackground, flexible ? GUILayout.ExpandWidth(true) : GUILayout.Width(CalculateListWidth(options)), GUILayout.ExpandHeight(true));
			BeginVertical();

			foreach (var option in options)
			{
				var wasSelected = Equals(option.value, selected);
				var isSelected = ListOption(option.label, wasSelected);

				if (!wasSelected && isSelected)
				{
					selectionChanged(option.value);
				}
			}

			EndVertical();
			GUILayout.EndScrollView();

			return newScroll;
		}

		private static GUIContent ListOptionContent(GUIContent label)
		{
			if (!string.IsNullOrEmpty(label.tooltip))
			{
				return new GUIContent($"{label.text}\n<size=10>{label.tooltip}</size>", label.image);
			}

			return label;
		}

		private static bool ListOption(GUIContent label, bool selected)
		{
			return GUILayout.Toggle(selected, ListOptionContent(label), LudiqStyles.listRow, GUILayout.ExpandWidth(true)) && !selected;
		}

		#endregion



		#region Standard Dropdowns

		public static void Dropdown
		(
			Vector2 position,
			Action<object> callback,
			IEnumerable<DropdownOption> options,
			object selected
		)
		{
			var hasMultipleDifferentValues = EditorGUI.showMixedValue;

			ICollection<DropdownOption> optionsCache = null;

			bool hasOptions;

			if (options != null)
			{
				optionsCache = options.AsReadOnlyCollection();
				hasOptions = optionsCache.Count > 0;
			}
			else
			{
				hasOptions = false;
			}

			var menu = new GenericMenu();

			GenericMenu.MenuFunction2 menuCallback = (o) =>
			{
				try
				{
					if (o is PopupFunc func)
					{
						if (func(out var v))
						{
							callback(v);
						}
					}
					else
					{
						callback(o);
					}
				}
				catch (ExitGUIException) { }
			};

			if (hasOptions)
			{
				var wasSeparator = false;

				foreach (var option in optionsCache)
				{
					var isSeparator = option is DropdownSeparator;

					if (isSeparator)
					{
						if (!wasSeparator)
						{
							menu.AddSeparator(((DropdownSeparator)option).path);
						}
					}
					else
					{
						var on = option.on ?? (!hasMultipleDifferentValues && OptionValuesEqual(selected, option.value));

						menu.AddItem(new GUIContent(option.label), @on, menuCallback, option.value);
					}

					wasSeparator = isSeparator;
				}
			}

			menu.DropDown(new Rect(position, Vector2.zero));
		}

		public static void Dropdown
		(
			Vector2 position,
			Action<HashSet<object>> callback,
			IEnumerable<DropdownOption> options,
			HashSet<object> selected,
			bool showNothingEverything = true
		)
		{
			var hasMultipleDifferentValues = EditorGUI.showMixedValue;

			ICollection<DropdownOption> optionsCache = null;

			bool hasOptions;

			if (options != null)
			{
				optionsCache = options.AsReadOnlyCollection();
				hasOptions = optionsCache.Count > 0;
			}
			else
			{
				hasOptions = false;
			}

			var selectedCopy = selected.ToHashSet();

			// Remove options outside range
			selectedCopy.RemoveWhere(so => !optionsCache.Any(o => OptionValuesEqual(o.value, so)));

			var menu = new GenericMenu();

			// The callback when a normal option has been selected
			GenericMenu.MenuFunction2 switchCallback = (o) =>
			{
				var switchValue = o;

				if (selectedCopy.Contains(switchValue))
				{
					selectedCopy.Remove(switchValue);
				}
				else
				{
					selectedCopy.Add(switchValue);
				}

				try
				{
					callback(selectedCopy);
				}
				catch (ExitGUIException) { }
			};

			// The callback when the special "Nothing" option has been selected
			GenericMenu.MenuFunction nothingCallback = () => { callback(new HashSet<object>()); };

			// The callback when the special "Everything" option has been selected
			GenericMenu.MenuFunction everythingCallback = () => { callback(optionsCache.Select((o) => o.value).ToHashSet()); };

			// Add the special "Nothing" / "Everything" options
			if (showNothingEverything)
			{
				menu.AddItem
				(
					new GUIContent("Nothing"),
					!hasMultipleDifferentValues && (selectedCopy.Count == 0),
					nothingCallback
				);

				if (hasOptions)
				{
					menu.AddItem
					(
						new GUIContent("Everything"),
						!hasMultipleDifferentValues && (selectedCopy.Count == optionsCache.Count) && selectedCopy.OrderBy(t => t).SequenceEqual(optionsCache.Select(o => o.value).OrderBy(t => t)),
						everythingCallback
					);
				}
			}

			// Add a separator (not in Unity default, but pretty)
			if (showNothingEverything && hasOptions)
			{
				menu.AddSeparator(string.Empty);
			}

			// Add the normal options
			if (hasOptions)
			{
				foreach (var option in optionsCache)
				{
					menu.AddItem
					(
						new GUIContent(option.label),
						!hasMultipleDifferentValues && (selectedCopy.Any(selectedValue => OptionValuesEqual(selectedValue, option.value))),
						switchCallback,
						option.value
					);
				}
			}

			// Show the dropdown
			menu.DropDown(new Rect(position, Vector2.zero));
		}

		#endregion



		#region Standard Popups

		public static object Popup
		(
			Rect position,
			Func<IEnumerable<DropdownOption>> getOptions,
			object selected,
			GUIContent label = null,
			GUIStyle style = null
		)
		{
			return ImmediatePopup
			(
				position,
				label ?? DefaultPopupLabel(selected),
				style,
				selected,
				() => Dropdown
				(
					new Vector2(position.xMin, position.yMax),
					UpdateImmediatePopupValue,
					getOptions(),
					selected
				)
			);
		}

		public static HashSet<object> Popup
		(
			Rect position,
			Func<IEnumerable<DropdownOption>> getOptions,
			HashSet<object> selected,
			bool showNothingEverything = true,
			GUIContent label = null,
			GUIStyle style = null
		)
		{
			return ImmediatePopup
			(
				position,
				label ?? DefaultPopupLabel(selected, getOptions().Count()),
				style,
				selected,
				() => Dropdown
				(
					new Vector2(position.xMin, position.yMax),
					UpdateImmediatePopupValue,
					getOptions(),
					selected,
					showNothingEverything
				)
			);
		}

		#endregion



		#region Fuzzy Dropdowns

		public static void FuzzyDropdown
		(
			Rect activatorPosition,
			IFuzzyOptionTree optionTree,
			object selected,
			Action<object> callback
		)
		{
			optionTree.selected.Clear();
			optionTree.selected.Add(selected);

			FuzzyWindow.Show
			(
				activatorPosition, optionTree, (option) =>
				{
					if (option.value is PopupFunc func)
					{
						if (func(out var v))
						{
							callback(v);
						}
					}
					else
					{
						callback(option.value);
					}

					FuzzyWindow.instance.Close();
					InternalEditorUtility.RepaintAllViews();
				}
			);
		}

		public static void FuzzyDropdown
		(
			Rect activatorPosition,
			IFuzzyOptionTree optionTree,
			HashSet<object> selected,
			Action<HashSet<object>> callback
		)
		{
			optionTree.selected.Clear();
			optionTree.selected.AddRange(selected);

			FuzzyWindow.Show
			(
				activatorPosition, optionTree, (option) =>
				{
					callback(optionTree.selected.ToHashSet());
					FuzzyWindow.instance.Close();
					InternalEditorUtility.RepaintAllViews();
				}
			);
		}

		#endregion



		#region Fuzzy Popups

		public static object FuzzyPopup
		(
			Rect position,
			Func<IFuzzyOptionTree> getOptions,
			object selected,
			GUIContent label = null,
			GUIStyle style = null
		)
		{
			Ensure.That(nameof(getOptions)).IsNotNull(getOptions);

			return ImmediatePopup
			(
				position,
				label ?? DefaultPopupLabel(selected),
				style,
				selected,
				() => FuzzyDropdown
				(
					LudiqGUIUtility.GUIToScreenRect(position),
					getOptions(),
					selected,
					UpdateImmediatePopupValue
				)
			);
		}

		public static object FuzzyPopupRaw
		(
			int controlID,
			bool activated,
			Rect activatorPosition,
			Func<IFuzzyOptionTree> getOptions,
			object selected
		)
		{
			return ImmediatePopupRaw
			(
				controlID,
				activated,
				selected,
				() => FuzzyDropdown
				(
					activatorPosition,
					getOptions(),
					selected,
					UpdateImmediatePopupValue
				)
			);
		}

		public static HashSet<object> FuzzyPopup
		(
			Rect position,
			Func<IFuzzyOptionTree> getOptions,
			HashSet<object> selected,
			GUIContent label = null,
			GUIStyle style = null
		)
		{
			Ensure.That(nameof(getOptions)).IsNotNull(getOptions);

			return ImmediatePopup
			(
				position,
				label ?? DefaultPopupLabel(getOptions().selected),
				style,
				selected,
				() => FuzzyDropdown
				(
					LudiqGUIUtility.GUIToScreenRect(position),
					getOptions(),
					selected,
					UpdateImmediatePopupValues
				)
			);
		}

		#endregion



		#region Immediate State Handling

		private static int activeActivatorControlID = -1;

		private static bool activeDropdownChanged;

		private static object activeDropdownValue;

		private static HashSet<object> activeDropdownValues;

		public static void UpdateImmediatePopupValue(object value)
		{
			activeDropdownValue = value;
			activeDropdownChanged = true;
		}

		public static void UpdateImmediatePopupValues(HashSet<object> value)
		{
			activeDropdownValues = value;
			activeDropdownChanged = true;
		}

		private static bool PopupActivatorRaw
		(
			int controlID,
			bool activated,
			Action dropdown
		)
		{
			if (activated)
			{
				// Assign the active control ID
				activeActivatorControlID = controlID;

				// Display the dropdown
				dropdown();
			}

			if ((controlID == activeActivatorControlID) && activeDropdownChanged)
			{
				GUI.changed = true;
				activeActivatorControlID = -1;
				activeDropdownChanged = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static object ImmediatePopup
		(
			int controlID,
			bool activated,
			object selected,
			Action dropdown
		)
		{
			if (PopupActivatorRaw(controlID, activated, dropdown))
			{
				return activeDropdownValue;
			}
			else
			{
				return selected;
			}
		}

		public static HashSet<object> ImmediatePopup
		(
			int controlID,
			bool activated,
			HashSet<object> selected,
			Action dropdown
		)
		{
			if (PopupActivatorRaw(controlID, activated, dropdown))
			{
				return activeDropdownValues;
			}
			else
			{
				return selected;
			}
		}

		private static readonly int PopupHash = "LudiqPopup".GetHashCode();

		private static bool PopupActivator
		(
			Rect position,
			GUIContent label,
			GUIStyle style,
			Action dropdown
		)
		{
			if (style == null)
			{
				style = EditorStyles.popup;
			}

			style = LudiqGUIUtility.BoldedStyle(style);

			// Render a button and get its control ID
			// Note: I'm having a really hard time ensuring control ID
			// continuity across immediate mode GUI. Internally, Unity
			// simply uses a static hash for its popups, but that doesn't
			// seem to be reliable. Using the internal s_LastControlID doesn't
			// seem to be constant across OnGUI calls either. Maybe it's because
			// I open another window in the mean time. To mitigate the effect,
			// we're using the current inspector block accessor to strenghten our
			// hash.
			var activatorControlID = GUIUtility.GetControlID(HashUtility.GetHashCode(PopupHash, Inspector.currentBlock.accessor), FocusType.Keyboard, position);
			var activatorClicked = GUI.Button(position, label, style);
			//var activatorControlID = LudiqGUIUtility.GetLastControlID();

			if (activatorClicked)
			{
				// Cancel button click
				GUI.changed = false;
			}

			return PopupActivatorRaw(activatorControlID, activatorClicked, dropdown);
		}

		public delegate bool PopupFunc(out object value);

		private static object ImmediatePopup
		(
			Rect position,
			GUIContent label,
			GUIStyle style,
			object selected,
			Action dropdown
		)
		{
			if (PopupActivator(position, label, style, dropdown))
			{
				return activeDropdownValue;
			}
			else
			{
				return selected;
			}
		}

		private static object ImmediatePopupRaw
		(
			int controlID,
			bool activated,
			object selected,
			Action dropdown
		)
		{
			if (PopupActivatorRaw(controlID, activated, dropdown))
			{
				return activeDropdownValue;
			}
			else
			{
				return selected;
			}
		}

		private static HashSet<object> ImmediatePopup
		(
			Rect position,
			GUIContent label,
			GUIStyle style,
			HashSet<object> selected,
			Action dropdown
		)
		{
			if (PopupActivator(position, label, style, dropdown))
			{
				return activeDropdownValues;
			}
			else
			{
				return selected;
			}
		}

		#endregion



		#region Popup Utility

		private static bool OptionValuesEqual(object a, object b)
		{
			return Equals(a, b);
		}

		private static GUIContent DefaultPopupLabel(object selectedValue)
		{
			string text;

			if (EditorGUI.showMixedValue)
			{
				text = "\u2014"; // Em Dash
			}
			else if (selectedValue != null)
			{
				text = selectedValue.ToString();
			}
			else
			{
				text = string.Empty;
			}

			return new GUIContent(text);
		}

		private static GUIContent DefaultPopupLabel(HashSet<object> selectedValues)
		{
			string text;

			if (EditorGUI.showMixedValue)
			{
				text = "\u2014"; // Em Dash
			}
			else
			{
				if (selectedValues.Count == 0)
				{
					text = "Nothing";
				}
				else if (selectedValues.Count == 1)
				{
					text = selectedValues.First().ToString();
				}
				else
				{
					text = "(Multiple)";
				}
			}

			return new GUIContent(text);
		}

		private static GUIContent DefaultPopupLabel(HashSet<object> selectedValues, int totalOptionsCount)
		{
			string text;

			if (EditorGUI.showMixedValue)
			{
				text = "\u2014"; // Em Dash
			}
			else
			{
				if (selectedValues.Count == 0)
				{
					text = "Nothing";
				}
				else if (selectedValues.Count == 1)
				{
					text = selectedValues.First().ToString();
				}
				else if (selectedValues.Count == totalOptionsCount)
				{
					text = "Everything";
				}
				else
				{
					text = "(Mixed)";
				}
			}

			return new GUIContent(text);
		}

		#endregion



		#region Layout

		// Some editor GUI functions on Mac throw a NRE on Unity < 2018.2. Wrap them in safer methods.
		// https://forum.unity.com/threads/mac-os-various-gui-exceptions-after-os-dialogs-have-been-shown.515421/

		public static void Space(float pixels)
		{
			try
			{
				GUILayout.Space(pixels);
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void FlexibleSpace()
		{
			try
			{
				GUILayout.FlexibleSpace();
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void BeginHorizontal(params GUILayoutOption[] options)
		{
			try
			{
				GUILayout.BeginHorizontal(options);
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
		{
			try
			{
				GUILayout.BeginHorizontal(style, options);
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void EndHorizontal()
		{
			try
			{
				GUILayout.EndHorizontal();
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void BeginVertical(params GUILayoutOption[] options)
		{
			try
			{
				GUILayout.BeginVertical(options);
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
		{
			try
			{
				GUILayout.BeginVertical(style, options);
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		public static void EndVertical()
		{
			try
			{
				GUILayout.EndVertical();
			}
			catch
			{
				GUIUtility.ExitGUI();
			}
		}

		#endregion



		#region Dropdown Toggle

		// Like EditorGUI.DropdownButton, but allows an IsActive parameter for display.

		private static readonly int dropdownToggleHash = "DropdownToggle".GetHashCode();

		public static bool DropdownToggle(Rect position, bool value, GUIContent content, GUIStyle style)
		{
			var id = GUIUtility.GetControlID(dropdownToggleHash, FocusType.Keyboard, position);
			return DropdownToggle(id, value, position, content, style);
		}
		
		internal static bool DropdownToggle(int id, bool value, Rect position, GUIContent content, GUIStyle style)
		{
			switch (e.type)
			{
				case EventType.Repaint:
					style.Draw(position, content, id, value);
					break;

				case EventType.MouseDown:
					if (position.Contains(e.mousePosition))
					{
						value = !value;
						GUIUtility.hotControl = id;
						e.Use();
					}

					break;

				case EventType.MouseUp:
					if (position.Contains(e.mousePosition))
					{
						GUIUtility.hotControl = 0;
						e.Use();
					}

					break;

				case EventType.KeyDown:
					if (GUIUtility.keyboardControl == id && e.character == ' ')
					{
						value = !value;
						GUIUtility.hotControl = id;
						GUIUtility.keyboardControl = id;
						e.Use();
					}

					break;
			}

			return value;
		}

		#endregion
	}
}