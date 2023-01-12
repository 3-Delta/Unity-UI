using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityObject = UnityEngine.Object;
#if PROBUILDER_4_OR_NEWER
using HandleUtility = UnityEditor.HandleUtility;
using UnityEngine.ProBuilder;
using UnityEditor.ProBuilder;
#endif

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class Probe
	{
		#region Object Picking

		public const int DefaultLimit = 100;

		public static bool canPickHandles => e != null &&
		                                     (
			                                     e.type == EventType.MouseMove ||
			                                     e.type == EventType.MouseDown ||
			                                     e.type == EventType.MouseUp ||
			                                     e.type == EventType.MouseDrag ||
			                                     e.type == EventType.MouseEnterWindow ||
			                                     e.type == EventType.MouseLeaveWindow
		                                     );

		public static ProbeHit? Pick(ProbeFilter filter, SceneView sceneView, Vector2 guiPosition, out Vector3 point)
		{
			var results = ListPool<ProbeHit>.New();

			try
			{
				PickAllNonAlloc(results, filter, sceneView, guiPosition);

				foreach (var result in results)
				{
					if (result.point.HasValue)
					{
						point = result.point.Value;
						return result;
					}
				}

				point = DefaultPoint(sceneView, guiPosition);
				return null;
			}
			finally
			{
				results.Free();
			}
		}

		public static ProbeHit[] PickAll(ProbeFilter filter, SceneView sceneView, Vector2 guiPosition, int limit = DefaultLimit)
		{
			var results = new List<ProbeHit>();
			PickAllNonAlloc(results, filter, sceneView, guiPosition, limit);
			return results.ToArray();
		}

		public static void PickAllNonAlloc(List<ProbeHit> hits, ProbeFilter filter, SceneView sceneView, Vector2 guiPosition, int limit = DefaultLimit)
		{
			var screenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(guiPosition);
			var ray3D = HandleUtility.GUIPointToWorldRay(guiPosition);
			var worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);
			var layerMask = PeekPlugin.Configuration.probeLayerMask;

			var raycastHits = ArrayPool<RaycastHit>.New(limit);
			var overlapHits = ArrayPool<Collider2D>.New(limit);
			var handleHits = HashSetPool<GameObject>.New();
			var ancestorHits = HashSetPool<ProbeHit>.New();
#if PROBUILDER_4_OR_NEWER
			var proBuilderHits = ListPool<ProbeHit>.New();
#endif

			var gameObjectHits = DictionaryPool<GameObject, ProbeHit>.New();

			try
			{
				// Raycast (3D)
				if (filter.raycast)
				{
					var raycastHitCount = Physics.RaycastNonAlloc(ray3D, raycastHits, Mathf.Infinity, layerMask);

					for (var i = 0; i < raycastHitCount; i++)
					{
						var raycastHit = raycastHits[i];

#if UNITY_2019_2_OR_NEWER
						if (SceneVisibilityManager.instance.IsHidden(raycastHit.transform.gameObject))
						{
							continue;
						}
#endif

						var gameObject = raycastHit.transform.gameObject;

						if (!gameObjectHits.TryGetValue(gameObject, out var hit))
						{
							hit = new ProbeHit(gameObject);
						}

						hit.point = raycastHit.point;
						hit.distance = raycastHit.distance;

						gameObjectHits[gameObject] = hit;
					}
				}

				// Overlap (2D)
				if (filter.overlap)
				{
					var overlapHitCount = Physics2D.OverlapPointNonAlloc(worldPosition, overlapHits, layerMask);

					for (var i = 0; i < overlapHitCount; i++)
					{
						var overlapHit = overlapHits[i];
						
#if UNITY_2019_2_OR_NEWER
						if (SceneVisibilityManager.instance.IsHidden(overlapHit.transform.gameObject))
						{
							continue;
						}
#endif

						var gameObject = overlapHit.transform.gameObject;

						if (!gameObjectHits.TryGetValue(gameObject, out var hit))
						{
							hit = new ProbeHit(gameObject);
						}

						hit.distance = hit.distance ?? Vector3.Distance(overlapHit.transform.position, worldPosition);

						gameObjectHits[gameObject] = hit;
					}
				}

				// Handles (Editor Default)
				if (filter.handles && canPickHandles)
				{
					PickAllHandlesNonAlloc(handleHits, guiPosition, limit);

					foreach (var handleHit in handleHits)
					{
						var gameObject = handleHit;

						if (!gameObjectHits.TryGetValue(gameObject, out var hit))
						{
							hit = new ProbeHit(gameObject);
						}

						hit.distance = hit.distance ?? Vector3.Distance(handleHit.transform.position, worldPosition);

						gameObjectHits[gameObject] = hit;
					}
				}

				// Ancestors
				foreach (var gameObjectHit in gameObjectHits)
				{
					var gameObject = gameObjectHit.Key;
					var hit = gameObjectHit.Value;

					var parent = gameObject.transform.parent;

					int depth = 0;

					while (parent != null)
					{
						var parentGameObject = parent.gameObject;

						var parentHit = new ProbeHit(parentGameObject);
						parentHit.groupGameObject = gameObject;
						parentHit.distance = hit.distance ?? Vector3.Distance(parentHit.transform.position, worldPosition);
						parentHit.groupOrder = 1000 + depth;

						ancestorHits.Add(parentHit);

						parent = parent.parent;
						depth++;
					}
				}

#if PROBUILDER_4_OR_NEWER
				// ProBuilder
				if (filter.proBuilder && ProBuilderEditor.instance != null)
				{
					var proBuilderMeshes = ListPool<ProBuilderMesh>.New();

					try
					{
						foreach (var gameObjectHit in gameObjectHits.Values)
						{
							var proBuilderMesh = gameObjectHit.gameObject.GetComponent<ProBuilderMesh>();

							if (proBuilderMesh != null)
							{
								proBuilderMeshes.Add(proBuilderMesh);
							}
						}

						PickProBuilderElementsNonAlloc(proBuilderHits, proBuilderMeshes, sceneView, guiPosition);
					}
					finally
					{
						proBuilderMeshes.Free();
					}
				}
#endif

				// Prepare final hits
				hits.Clear();

				// Add hits
				foreach (var gameObjectHit in gameObjectHits.Values)
				{
					hits.Add(gameObjectHit);
				}

				foreach (var ancestorHit in ancestorHits)
				{
					hits.Add(ancestorHit);
				}

#if PROBUILDER_4_OR_NEWER
				foreach (var proBuilderHit in proBuilderHits)
				{
					hits.Add(proBuilderHit);
				}
#endif

				// Sort by distance
				hits.Sort(compareHits);
			}
			finally
			{
				raycastHits.Free();
				overlapHits.Free();
				handleHits.Free();
				ancestorHits.Free();

#if PROBUILDER_4_OR_NEWER
				proBuilderHits.Free();
#endif

				gameObjectHits.Free();
			}
		}

		private static void PickAllHandlesNonAlloc(HashSet<GameObject> results, Vector2 position, int limit = DefaultLimit)
		{
			if (!canPickHandles)
			{
				// HandleUtility.PickGameObject is not supported in those contexts
				Debug.LogWarning($"Cannot pick game objects in the current event: {e?.ToString() ?? "null"}");
				return;
			}

			GameObject result = null;

			var count = 0;

			do
			{
				var ignored = results.ToArray();

				result = HandleUtility.PickGameObject(position, false, ignored);

				// Ignored doesn't seem very reliable. Sometimes, an item included
				// in ignored will still be returned. That's a sign we should stop.
				if (results.Contains(result))
				{
					result = null;
				}

				if (result != null)
				{
					results.Add(result);
				}
			} while (result != null && count++ < limit);
		}

		private static readonly Comparison<ProbeHit> compareHits = CompareHits;

		private static int CompareHits(ProbeHit a, ProbeHit b)
		{
			var distanceA = a.distance ?? Mathf.Infinity;
			var distanceB = b.distance ?? Mathf.Infinity;
			return distanceA.CompareTo(distanceB);
		}
		
		private static Vector3 DefaultPoint(SceneView sceneView, Vector2 guiPosition)
		{
			var screenPosition = (Vector3)HandleUtility.GUIPointToScreenPixelCoordinate(guiPosition);
			screenPosition.z = sceneView.cameraDistance;
			return sceneView.camera.ScreenToWorldPoint(screenPosition);
		}

#if PROBUILDER_4_OR_NEWER

		private static void PickProBuilderElementsNonAlloc(List<ProbeHit> hits, List<ProBuilderMesh> meshes, SceneView sceneView, Vector2 guiPosition)
		{
			var screenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(guiPosition);
			var worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

			var pickRadius = PeekPlugin.Configuration.probeProBuilderRadius;

			var pickerOptions = PickerOptions.Default;

			pickerOptions.depthTest = PeekPlugin.Configuration.probeProBuilderDepthTest;

			var pickRect = new Rect
			(
				guiPosition.x - pickRadius,
				guiPosition.y - pickRadius,
				2 * pickRadius,
				2 * pickRadius
			);

			var verticesByMeshes = SelectionPicker.PickVerticesInRect(sceneView.camera, pickRect, meshes, pickerOptions, EditorGUIUtility.pixelsPerPoint);

			var edgesByMeshes = SelectionPicker.PickEdgesInRect(sceneView.camera, pickRect, meshes, pickerOptions, EditorGUIUtility.pixelsPerPoint);

			var facesByMeshes = SelectionPicker.PickFacesInRect(sceneView.camera, pickRect, meshes, pickerOptions, EditorGUIUtility.pixelsPerPoint);

			foreach (var verticesByMesh in verticesByMeshes)
			{
				var mesh = verticesByMesh.Key;
				var gameObject = mesh.gameObject;
				var vertices = verticesByMesh.Value;
				var meshVertices = mesh.GetVertices();
				var sharedVertices = mesh.sharedVertices;

				foreach (var vertexIndex in vertices)
				{
					var hit = new ProbeHit(gameObject);

					var vertex = meshVertices[vertexIndex];
					var sharedVertex = sharedVertices[vertexIndex];
					var sharedVertexIndex = sharedVertex[0];
					hit.point = vertex.position;
					hit.distance = Vector3.Distance(vertex.position, worldPosition);

					hit.label = $"{mesh.name}: Vertex {vertexIndex}";
					hit.icon = PeekProBuilderIntegration.Icons.vertex;
					hit.groupOrder = 1;

					hit.selectHandler = (add) =>
					{
						Selection.activeGameObject = gameObject;

						ProBuilderEditor.selectMode = SelectMode.Vertex;

						Undo.RecordObject(mesh, "Selection Change");

						if (add)
						{
							mesh.SetSelectedVertices(mesh.selectedVertices.Concat(sharedVertex));
						}
						else
						{
							mesh.SetSelectedVertices(sharedVertex);
						}

						PeekProBuilderIntegration.UpdateSelection();
					};

					hit.focusHandler = () => ProBuilderHighlight(new SceneSelection(mesh, sharedVertexIndex));
					hit.lostFocusHandler = ClearProBuilderHighlight;

					hits.Add(hit);
				}
			}

			foreach (var edgesByMesh in edgesByMeshes)
			{
				var mesh = edgesByMesh.Key;
				var gameObject = mesh.gameObject;
				var edges = edgesByMesh.Value;
				var meshVertices = mesh.GetVertices();
				var sharedVertices = mesh.sharedVertices;

				var visited = HashSetPool<(int, int)>.New();

				foreach (var edge in edges)
				{
					var hit = new ProbeHit(gameObject);

					var vertexA = meshVertices[edge.a];
					var vertexB = meshVertices[edge.b];
					var center = (vertexA.position + vertexB.position) / 2;

					var sharedVertexIndexA = -1;
					var sharedVertexIndexB = -1;

					for (var currentSharedIndex = 0; currentSharedIndex < sharedVertices.Count; currentSharedIndex++)
					{
						var sharedVertex = sharedVertices[currentSharedIndex];

						if (sharedVertex.Contains(edge.a))
						{
							sharedVertexIndexA = currentSharedIndex;
						}

						if (sharedVertex.Contains(edge.b))
						{
							sharedVertexIndexB = currentSharedIndex;
						}
					}

					var sharedVertexIndexMin = Mathf.Min(sharedVertexIndexA, sharedVertexIndexB);
					var sharedVertexIndexMax = Mathf.Max(sharedVertexIndexA, sharedVertexIndexB);

					if (visited.Contains((sharedVertexIndexMin, sharedVertexIndexMax)))
					{
						continue;
					}

					hit.point = center;
					hit.distance = Vector3.Distance(center, worldPosition);
					hit.label = $"{mesh.name}: Edge [{sharedVertexIndexMin}, {sharedVertexIndexMax}]";
					hit.icon = PeekProBuilderIntegration.Icons.edge;
					hit.groupOrder = 2;

					hit.selectHandler = (add) =>
					{
						Selection.activeGameObject = gameObject;

						ProBuilderEditor.selectMode = SelectMode.Edge;

						Undo.RecordObject(mesh, "Selection Change");

						if (add)
						{
							mesh.SetSelectedEdges(mesh.selectedEdges.Append(edge));
						}
						else
						{
							mesh.SetSelectedEdges(edge.Yield());
						}

						PeekProBuilderIntegration.UpdateSelection();
					};

					hit.focusHandler = () => ProBuilderHighlight(new SceneSelection(mesh, edge));
					hit.lostFocusHandler = ClearProBuilderHighlight;

					hits.Add(hit);

					visited.Add((sharedVertexIndexMin, sharedVertexIndexMax));
				}

				visited.Free();
			}

			foreach (var facesByMesh in facesByMeshes)
			{
				var mesh = facesByMesh.Key;
				var gameObject = mesh.gameObject;
				var faces = facesByMesh.Value;
				var meshVertices = mesh.GetVertices();
				var meshFaces = mesh.faces;

				foreach (var face in faces)
				{
					var faceIndex = meshFaces.IndexOf(face);

					var hit = new ProbeHit(gameObject);

					var center = Vector3.zero;

					foreach (var vertexIndex in face.distinctIndexes)
					{
						var vertex = meshVertices[vertexIndex];
						center += vertex.position;
					}

					center /= face.distinctIndexes.Count;

					hit.point = center;
					hit.distance = Vector3.Distance(center, worldPosition);

					hit.label = $"{mesh.name}: Face {faceIndex}";
					hit.icon = PeekProBuilderIntegration.Icons.face;
					hit.groupOrder = 3;

					hit.selectHandler = (add) =>
					{
						Selection.activeGameObject = gameObject;

						ProBuilderEditor.selectMode = SelectMode.Face;

						Undo.RecordObject(mesh, "Selection Change");

						if (add)
						{
							mesh.SetSelectedFaces(mesh.GetSelectedFaces().Append(face));
						}
						else
						{
							mesh.SetSelectedFaces(null);
							mesh.SetSelectedFaces(face.Yield());
						}

						PeekProBuilderIntegration.UpdateSelection();
					};

					hit.focusHandler = () => ProBuilderHighlight(new SceneSelection(mesh, face));
					hit.lostFocusHandler = ClearProBuilderHighlight;

					hits.Add(hit);
				}
			}
		}

#endif

		#endregion



		#region Scene View Integration

		private static Event e => Event.current;

		private static Vector2? pressPosition;

		private static GameObject highlight { get; set; }

		public static void Highlight(GameObject selection)
		{
			highlight = selection;
			SceneView.RepaintAll();
		}

		public static void ClearHighlight()
		{
			highlight = null;
			SceneView.RepaintAll();
		}
		
#if PROBUILDER_4_OR_NEWER
		private static SceneSelection proBuilderHighlight { get; set; }

		public static void ProBuilderHighlight(SceneSelection selection)
		{
			highlight = selection.gameObject;
			proBuilderHighlight = selection;
			SceneView.RepaintAll();
		}

		public static void ClearProBuilderHighlight()
		{
			highlight = null;
			proBuilderHighlight = null;
			SceneView.RepaintAll();
		}
#endif

		private static Material highlightMaskMaterial;

		private static Material highlightEffectMaterial;

		private const CameraEvent highlightCameraEvent = CameraEvent.AfterImageEffects;

		private const string highlightCommandBufferName = "Ludiq.Peek.ProbeHighlight";

		private static CommandBuffer GetOrAddHighlightCommandBuffer(SceneView sceneView)
		{
			CommandBuffer highlightCommandBuffer = null;

			foreach (var commandBuffer in sceneView.camera.GetCommandBuffers(highlightCameraEvent))
			{
				if (commandBuffer.name == highlightCommandBufferName)
				{
					highlightCommandBuffer = commandBuffer;
				}
			}

			if (highlightCommandBuffer == null)
			{
				highlightCommandBuffer = new CommandBuffer() {name = highlightCommandBufferName};
				sceneView.camera.AddCommandBuffer(highlightCameraEvent, highlightCommandBuffer);
			}

			return highlightCommandBuffer;
		}

		private static Color GetSceneViewColorPref(string name)
		{
			// TODO: Usually we'd use dynamic, such  as:
			// (Color)UnityEditorDynamic.SceneView.kSceneViewSelectedOutline.Color
			// But it's causing a crash for unknown reasons, so I'm switching to manual reflection
			// If we get a better error log from that, then at least we'd know what's happening
			var sceneView = typeof(SceneView);
			var sceneView_pref = sceneView.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null);
			var sceneView_pref_Color = sceneView_pref.GetType().GetProperty("Color", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(sceneView_pref);
			return sceneView_pref_Color.CastTo<Color>();
		}

		private static void ProgramHighlight(SceneView sceneView)
		{
			if (e.type != EventType.Repaint)
			{
				return;
			}

			if (highlightMaskMaterial == null)
			{
				highlightMaskMaterial = new Material(Shader.Find("Ludiq/Editor/ProbeHighlightMask"));
			}

			if (highlightEffectMaterial == null)
			{
				highlightEffectMaterial = new Material(Shader.Find("Ludiq/Editor/ProbeHighlightEffect"));
				highlightEffectMaterial.SetColor("_Color", (GetSceneViewColorPref("kSceneViewSelectedOutline")).WithAlpha(0.5f));
				highlightEffectMaterial.SetColor("_ChildColor", (GetSceneViewColorPref("kSceneViewSelectedChildrenOutline")).WithAlpha(0.5f));
			}
			
			var graphics = GetOrAddHighlightCommandBuffer(sceneView);

			graphics.Clear();

			if (highlight == null)
			{
				return;
			}

			var maskId = Shader.PropertyToID("_ProbeHighlightMask");
			var resultId = Shader.PropertyToID("_ProbeHighlightResult");
			var roootId = Shader.PropertyToID("_ProbeHighlightRoot");

			graphics.GetTemporaryRT(maskId, -1, -1);
			graphics.SetRenderTarget(maskId, BuiltinRenderTextureType.CameraTarget);
			graphics.ClearRenderTarget(false, true, new Color(0,0,0,0));
			
			var renderers = highlight.GetComponentsInChildren<Renderer>();

			foreach (var renderer in renderers)
			{
				var isRoot = renderer.gameObject == highlight;
				graphics.SetGlobalFloat(roootId, isRoot ? 1 : 0);

				if (renderer is MeshRenderer meshRenderer)
				{
					var mesh = renderer.GetComponent<MeshFilter>().AsUnityNull()?.sharedMesh;

					if (mesh == null)
					{
						continue;
					}

					for (var subMeshIndex = meshRenderer.subMeshStartIndex; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
					{
						graphics.DrawRenderer(renderer, highlightMaskMaterial, subMeshIndex);
					}
				}
				else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
				{
					var mesh = skinnedMeshRenderer.sharedMesh;

					if (mesh == null)
					{
						continue;
					}

					for (var subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
					{
						graphics.DrawRenderer(renderer, highlightMaskMaterial, subMeshIndex);
					}
				}
				else
				{
					graphics.DrawRenderer(renderer, highlightMaskMaterial);
				}
			}
			
			graphics.SetGlobalTexture(maskId, maskId);
			graphics.GetTemporaryRT(resultId, -1, -1);
			graphics.Blit(BuiltinRenderTextureType.CameraTarget, resultId, highlightEffectMaterial);
			graphics.Blit(resultId, BuiltinRenderTextureType.CameraTarget);
		}

		internal static void OnSceneGUI(SceneView sceneView)
		{
			if (!PeekPlugin.Configuration.enableProbe)
			{
				return;
			}
			
			Profiler.BeginSample("Peek." + nameof(Probe));

			try
			{
				ProgramHighlight(sceneView);
				
#if PROBUILDER_4_OR_NEWER
				PeekProBuilderIntegration.DrawHighlight(proBuilderHighlight);
#endif

				var shortcut = PeekPlugin.Configuration.probeShortcut;

				// Make sure not to conflict with right-click pan
				shortcut.mouseShortcut.checkRelease = true;
				shortcut.mouseShortcut.requireStaticRelease = true;

				if (shortcut.Check(e) && !SceneViewIntegration.used)
				{
					var hits = ListPool<ProbeHit>.New();

					try
					{
						PickAllNonAlloc(hits, ProbeFilter.@default, sceneView, e.mousePosition, PeekPlugin.Configuration.probeLimit);

						if (hits.Count > 0)
						{
							var add = e.shift;
							
							var activatorPosition = new Rect(e.mousePosition, Vector2.zero);
							activatorPosition.width = 220;
							activatorPosition = LudiqGUIUtility.GUIToScreenRect(activatorPosition);

							// Note: Had to make FuzzyWindow use OnMouseUp for select here instead
							// of OnMouseDown because otherwise escaping the default RectSelection
							// behaviour with the event order and DefaultControl ID's was... hell.

							LudiqGUI.FuzzyDropdown
							(
								activatorPosition,
								new ProbeOptionTree(hits),
								null,
								(_hit) =>
								{
									add |= e?.shift ?? false;
									var hit = (ProbeHit)_hit;
									hit.Select(add);
								}
							);

							FuzzyWindow.instance.Focus();
							GUIUtility.hotControl = 0; // Escape the default RectSelection control
							e.Use();
						}
					}
					finally
					{
						hits.Free();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			Profiler.EndSample();
		}

		#endregion
	}
}