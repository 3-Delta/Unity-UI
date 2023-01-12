using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class EditorTextureUtility
	{
		private static readonly Dictionary<Vector2Int, Dictionary<Texture2D, Texture2D>> resizeCache = new Dictionary<Vector2Int, Dictionary<Texture2D, Texture2D>>();
		
		public static Texture2D Resized(this Texture2D source, int size)
		{
			return source.Resized(size, size);
		}

		public static Texture2D Resized(this Texture2D source, int width, int height)
		{
			if (source.width == width && source.height == height)
			{
				return source;
			}

			lock (resizeCache)
			{
				var resolution = new Vector2Int(width, height);

				if (!resizeCache.TryGetValue(resolution, out var resolutionCache))
				{
					resolutionCache = new Dictionary<Texture2D, Texture2D>();
					resizeCache.Add(resolution, resolutionCache);
				}

				if (!resolutionCache.TryGetValue(source, out var resized))
				{
					resized = source.ReadableCopy(width, height);
					resolutionCache.Add(source, resized);
				}

				return resized;
			}
		}

		private static Texture2D ReadableCopy(this Texture2D source, int width, int height)
		{
			// Works around non-readable textures and mipmap limitations
			
			if (source.isReadable && source.width == width && source.height == height)
			{
				return source;
			}

			var rt = RenderTexture.GetTemporary
			(
				width,
				height,
				0,
				RenderTextureFormat.Default,
				RenderTextureReadWrite.Linear
			);

			Graphics.Blit(source, rt);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = rt;
			Texture2D readable = new Texture2D(width, height);
			readable.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			readable.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(rt);
			return readable;
		}

		public static Texture2D ReadableCopy(this Texture2D source)
		{
			return source.ReadableCopy(source.width, source.height);
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Export All Editor Textures...", priority = LudiqProduct.InternalToolsMenuPriority + 302)]
#endif
		public static void ExportAllEditorIcon()
		{
			var assetBundle = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.NonPublic)
			                                          .Invoke(null,null) as AssetBundle;
			
			var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "EditorTextures");

			PathUtility.CreateDirectoryIfNeeded(outputFolder);

			var textures = assetBundle.LoadAllAssets<Texture2D>();
			
			try
			{
				for (int i = 0; i < textures.Length; i++)
				{
					var texture = textures[i];

					try
					{
						ProgressUtility.DisplayProgressBar("Export Editor Textures", texture.name, (float)i / textures.Length);
						var outputPath = Path.Combine(outputFolder, texture.name + ".png");
						File.WriteAllBytes(outputPath, texture.ReadableCopy().EncodeToPNG());
					}
					catch (Exception ex)
					{
						Debug.LogError($"Failed to export {texture.name}:\n{ex}");
					}
				}
			}
			finally
			{
				ProgressUtility.ClearProgressBar();
			}
		}
	}
}