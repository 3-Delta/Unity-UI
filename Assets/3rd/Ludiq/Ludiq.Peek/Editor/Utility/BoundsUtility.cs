using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class BoundsUtility
	{
		public static void GetPointsNoAlloc(this Bounds bounds, Vector3[] array)
		{
			var min = bounds.min;
			var max = bounds.max;

			array[0] = new Vector3(min.x, min.y, min.z);
			array[1] = new Vector3(min.x, min.y, max.z);
			array[2] = new Vector3(min.x, max.y, min.z);
			array[3] = new Vector3(min.x, max.y, max.z);
			array[4] = new Vector3(max.x, min.y, min.z);
			array[5] = new Vector3(max.x, min.y, max.z);
			array[6] = new Vector3(max.x, max.y, min.z);
			array[7] = new Vector3(max.x, max.y, max.z);
		}

		public static Bounds BoundsFromCorners(Vector3[] corners)
		{
			var minX = float.MaxValue;
			var minY = float.MaxValue;
			var minZ = float.MaxValue;

			var maxX = float.MinValue;
			var maxY = float.MinValue;
			var maxZ = float.MinValue;

			foreach (var corner in corners)
			{
				if (corner.x < minX)
				{
					minX = corner.x;
				}

				if (corner.y < minY)
				{
					minY = corner.y;
				}

				if (corner.z < minZ)
				{
					minZ = corner.z;
				}

				if (corner.x > minX)
				{
					maxX = corner.x;
				}

				if (corner.y > minY)
				{
					maxY = corner.y;
				}

				if (corner.z > minZ)
				{
					maxZ = corner.z;
				}
			}

			return new Bounds()
			{
				min = new Vector3(minX, minY, minZ),
				max = new Vector3(maxX, maxY, maxZ)
			};
		}

		public static bool CalculateBounds
		(
			this GameObject root,
			out Bounds bounds,
			Space space,
			bool renderers = true,
			bool colliders = true,
			bool meshes = false,
			bool graphics = true,
			bool particles = false)
		{
			bounds = new Bounds();

			var first = true;

			if (space == Space.Self)
			{
				if (renderers)
				{
					var _renderers = ListPool<Renderer>.New();
					root.GetComponentsInChildren(_renderers);

					foreach (var renderer in _renderers)
					{
						if (!renderer.enabled)
						{
							continue;
						}

						if (!particles && renderer is ParticleSystemRenderer)
						{
							continue;
						}

						var rendererBounds = renderer.bounds;

						rendererBounds.SetMinMax
						(
							root.transform.InverseTransformPoint(rendererBounds.min),
							root.transform.InverseTransformPoint(rendererBounds.max)
						);

						if (first)
						{
							bounds = rendererBounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(rendererBounds);
						}
					}

					_renderers.Free();
				}

				if (meshes)
				{
					var _meshFilters = ListPool<MeshFilter>.New();
					root.GetComponentsInChildren(_meshFilters);

					foreach (var meshFilter in _meshFilters)
					{
						var mesh = meshFilter.sharedMesh;

						if (mesh == null)
						{
							continue;
						}

						var meshBounds = mesh.bounds;

						meshBounds.SetMinMax
						(
							root.transform.InverseTransformPoint(meshFilter.transform.TransformPoint(meshBounds.min)),
							root.transform.InverseTransformPoint(meshFilter.transform.TransformPoint(meshBounds.max))
						);

						if (first)
						{
							bounds = meshBounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(meshBounds);
						}
					}

					_meshFilters.Free();
				}

				if (graphics)
				{
					var _graphics = ListPool<Graphic>.New();
					root.GetComponentsInChildren(_graphics);

					foreach (var graphic in _graphics)
					{
						if (!graphic.enabled)
						{
							continue;
						}

						var graphicCorners = ArrayPool<Vector3>.New(4);
						graphic.rectTransform.GetLocalCorners(graphicCorners);
						var graphicsBounds = BoundsFromCorners(graphicCorners);
						graphicCorners.Free();

						if (first)
						{
							bounds = graphicsBounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(graphicsBounds);
						}
					}

					_graphics.Free();
				}

				if (colliders && first)
				{
					var _colliders = ListPool<Collider>.New();
					root.GetComponentsInChildren(_colliders);

					foreach (var collider in _colliders)
					{
						if (!collider.enabled)
						{
							continue;
						}

						var colliderBounds = collider.bounds;

						colliderBounds.SetMinMax
						(
							root.transform.InverseTransformPoint(colliderBounds.min),
							root.transform.InverseTransformPoint(colliderBounds.max)
						);

						if (first)
						{
							bounds = colliderBounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(colliderBounds);
						}
					}

					_colliders.Free();
				}

				return !first;
			}
			else // if (space == Space.World)
			{
				if (renderers)
				{
					var _renderers = ListPool<Renderer>.New();
					root.GetComponentsInChildren(_renderers);

					foreach (var renderer in _renderers)
					{
						if (!renderer.enabled)
						{
							continue;
						}

						if (!particles && renderer is ParticleSystemRenderer)
						{
							continue;
						}

						if (first)
						{
							bounds = renderer.bounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(renderer.bounds);
						}
					}

					_renderers.Free();
				}

				if (meshes)
				{
					var _meshFilters = ListPool<MeshFilter>.New();
					root.GetComponentsInChildren(_meshFilters);

					foreach (var meshFilter in _meshFilters)
					{
						var mesh = meshFilter.sharedMesh;

						if (mesh == null)
						{
							continue;
						}

						var meshBounds = mesh.bounds;

						meshBounds.SetMinMax
						(
							root.transform.TransformPoint(meshBounds.min),
							root.transform.TransformPoint(meshBounds.max)
						);

						if (first)
						{
							bounds = meshBounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(meshBounds);
						}
					}

					_meshFilters.Free();
				}

				if (graphics)
				{
					var _graphics = ListPool<Graphic>.New();
					root.GetComponentsInChildren(_graphics);

					foreach (var graphic in _graphics)
					{
						if (!graphic.enabled)
						{
							continue;
						}

						var graphicCorners = ArrayPool<Vector3>.New(4);
						graphic.rectTransform.GetWorldCorners(graphicCorners);
						var graphicsBounds = BoundsFromCorners(graphicCorners);
						graphicCorners.Free();

						if (first)
						{
							bounds = graphicsBounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(graphicsBounds);
						}
					}

					_graphics.Free();
				}

				if (colliders && first)
				{
					var _colliders = ListPool<Collider>.New();
					root.GetComponentsInChildren(_colliders);

					foreach (var collider in _colliders)
					{
						if (!collider.enabled)
						{
							continue;
						}

						if (first)
						{
							bounds = collider.bounds;
							first = false;
						}
						else
						{
							bounds.Encapsulate(collider.bounds);
						}
					}

					_colliders.Free();
				}
			}

			return !first;
		}
	}
}
