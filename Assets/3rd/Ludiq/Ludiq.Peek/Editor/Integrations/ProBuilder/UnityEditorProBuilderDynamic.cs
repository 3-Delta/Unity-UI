#if PROBUILDER_4_OR_NEWER
using System;
using System.Reflection;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor.ProBuilder;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class UnityEditorProBuilderDynamic
	{
		public static readonly Assembly UnityEditorProBuilderAssembly;

		public static readonly dynamic IconUtility;
		
		public static readonly Type IconSkinType;

#if PROBUILDER_4_4_OR_NEWER
		public static readonly dynamic EditorHandleDrawing;
#else
		public static readonly dynamic EditorMeshHandles;
#endif

		static UnityEditorProBuilderDynamic()
		{
			UnityEditorProBuilderAssembly = typeof(ProBuilderEditor).Assembly;

			IconUtility = UnityEditorProBuilderAssembly.GetType("UnityEditor.ProBuilder.IconUtility", true).AsDynamicType();
			IconSkinType = UnityEditorProBuilderAssembly.GetType("UnityEditor.ProBuilder.IconSkin", true);

#if PROBUILDER_4_4_OR_NEWER
			EditorHandleDrawing = UnityEditorProBuilderAssembly.GetType("UnityEditor.ProBuilder.EditorHandleDrawing", true).AsDynamicType();
#else
			EditorMeshHandles = UnityEditorProBuilderAssembly.GetType("UnityEditor.ProBuilder.EditorMeshHandles", true).AsDynamicType();
#endif
		}
	}
}
#endif