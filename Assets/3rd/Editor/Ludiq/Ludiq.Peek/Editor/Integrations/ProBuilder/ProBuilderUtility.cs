#if PROBUILDER_4_OR_NEWER
using System;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class PeekProBuilderIntegration
	{
		static PeekProBuilderIntegration()
		{

		}

		public static void UpdateSelection()
		{
			ProBuilderEditor.instance?.AsDynamic().UpdateSelection(true);
		}

		public static void DrawHighlight(SceneSelection highlight)
		{
			if (Event.current.type == EventType.Repaint && highlight != null)
			{
#if PROBUILDER_4_4_OR_NEWER
				UnityEditorProBuilderDynamic.EditorHandleDrawing.DrawSceneSelection(highlight);
#else
				UnityEditorProBuilderDynamic.EditorMeshHandles.DrawSceneSelection(highlight);
#endif
			}
		}

		public static class Icons
		{
			public static readonly EditorTexture vertex;

			public static readonly EditorTexture edge;

			public static readonly EditorTexture face;

			public static readonly EditorTexture @object;

			static Icons()
			{
				var iconSkin = Enum.ToObject(UnityEditorProBuilderDynamic.IconSkinType, 0);
				vertex = EditorTexture.Single((Texture2D)UnityEditorProBuilderDynamic.IconUtility.GetIcon("Modes/Mode_Vertex", iconSkin));
				edge = EditorTexture.Single((Texture2D)UnityEditorProBuilderDynamic.IconUtility.GetIcon("Modes/Mode_Edge", iconSkin));
				face = EditorTexture.Single((Texture2D)UnityEditorProBuilderDynamic.IconUtility.GetIcon("Modes/Mode_Face", iconSkin));
				@object = EditorTexture.Single((Texture2D)UnityEditorProBuilderDynamic.IconUtility.GetIcon("Modes/Mode_Object", iconSkin));
			}
		}
	}
}
#endif