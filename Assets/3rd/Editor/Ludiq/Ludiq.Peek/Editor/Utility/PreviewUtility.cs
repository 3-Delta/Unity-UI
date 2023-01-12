using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	public static class PreviewUtility
	{
		public static Texture2D GetPreview(int instanceID)
		{
			return UnityEditorDynamic.AssetPreview.GetAssetPreview(instanceID);
		}

		public static bool TryGetPreview(HierarchyPropertyCache hierarchyProperty, out Texture2D preview)
		{
			preview = null;

			if (!hierarchyProperty.hasFullPreviewImage)
			{
				return false;
			}

			return GetPreview(hierarchyProperty.instanceID);
		}

		public static bool HasPreview(HierarchyPropertyCache target)
		{
			return TryGetPreview(target, out var preview);
		}

		public static bool HasPreview(UnityObject target)
		{
			return TryGetPreview(target, out var preview);
		}

		public static Texture2D GetPreview(UnityObject target)
		{
			if (TryGetPreview(target, out var preview))
			{
				return preview;
			}

			return null;
		}

		public static bool TryGetPreview(UnityObject target, out Texture2D preview)
		{
			preview = null;
			
			if (target == null)
			{
				return false;
			}

			if (target is GameObject go)
			{
				var renderers = ListPool<Renderer>.New();

				try
				{
					go.GetComponentsInChildren(renderers);

					foreach (var renderer in renderers)
					{
						if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
						{
							preview = AssetPreview.GetAssetPreview(target);
							return true;
						}
						else if (renderer is SpriteRenderer spriteRenderer && spriteRenderer.sprite != null)
						{
							preview = AssetPreview.GetAssetPreview(spriteRenderer.sprite);
							return true;
						}
					}
				}
				finally
				{
					renderers.Free();
				}

				return false;
			}

			if (target is Material || target is Sprite || target is Texture2D)
			{
				preview = AssetPreview.GetAssetPreview(target);
				return true;
			}
			
			return false;
		}
	}
} 