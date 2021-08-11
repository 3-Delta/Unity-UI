using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludiq.Peek
{
	public static class ComponentCache
	{
		private static Dictionary<GameObject[], List<List<Component>>> sharedComponents = new Dictionary<GameObject[], List<List<Component>>>();
		
		private static Dictionary<GameObject[], List<List<Component>>> inconsistentComponents = new Dictionary<GameObject[], List<List<Component>>>();

		private static Dictionary<GameObject, List<RemovedComponent>> removedComponents = new Dictionary<GameObject, List<RemovedComponent>>();

		static ComponentCache()
		{
			EditorApplication.projectChanged += Clear;
			EditorApplication.hierarchyChanged += Clear;
			EditorApplicationUtility.onEnterEditMode += Clear; // Components get destroyed but assembly doesn't get reloaded
			PrefabUtility.prefabInstanceUpdated += (instance) => Clear();
		}

		private static void FetchSharedComponents(GameObject[] targets, out List<List<Component>> sharedComponents, out List<List<Component>> inconsistentComponents)
		{
			sharedComponents = new List<List<Component>>();
			inconsistentComponents = new List<List<Component>>();

			if (targets.Length == 0 || targets[0] == null)
			{
				return;
			}

			var referenceComponents = ListPool<Component>.New();

			targets[0].GetComponents(referenceComponents);

			// TODO: Figure out why some users have "dictionary not used by pool" when freeing below.
			// Hacky hotfix.
			//var componentCounters = DictionaryPool<Type, int>.New();
			var componentCounters = new Dictionary<Type, int>();

			foreach (var component in referenceComponents)
			{
				if (component == null)
				{
					continue;
				}

				var presentOnAll = true;
				var components = new List<Component>();
				var type = component.GetType();

				if (!componentCounters.ContainsKey(type))
				{
					componentCounters[type] = 0;
				}

				if (!componentCounters.TryGetValue(type, out var componentCounter))
				{
					componentCounter = 0;
				}

				foreach (var target in targets)
				{
					if (target == null)
					{
						continue;
					}

					var targetComponents = ListPool<Component>.New();

					target.GetComponents(type, targetComponents);

					var targetComponent = componentCounter < targetComponents.Count ? targetComponents[componentCounter] : null;

					targetComponents.Free();

					if (targetComponent != null)
					{
						components.Add(targetComponent);
					}
					else
					{
						presentOnAll = false;
						break;
					}
				}

				if (presentOnAll)
				{
					sharedComponents.Add(components);
				}
				else
				{
					inconsistentComponents.Add(components);
				}

				componentCounters[type] = componentCounter + 1;
			}

			referenceComponents.Free();

			//componentCounters.Free();
		}

		private static List<RemovedComponent> FetchRemovedComponents(GameObject target)
		{
			var removedComponents = new List<RemovedComponent>();

			foreach (var removedComponent in PrefabUtility.GetRemovedComponents(target))
			{
				if (removedComponent.assetComponent == null) // Not sure why this happens
				{
					continue;
				}

				removedComponents.Add(removedComponent);
			}

			return removedComponents;
		}

		public static List<List<Component>> GetSharedComponents(GameObject[] targets)
		{
			if (!sharedComponents.TryGetValue(targets, out var result))
			{
				FetchSharedComponents(targets, out var shared, out var inconsistent);
				sharedComponents[targets] = shared;
				inconsistentComponents[targets] = inconsistent;
				result = shared;
			}

			return result;
		}

		public static List<List<Component>> GetInconsistentComponents(GameObject[] targets)
		{
			if (!inconsistentComponents.TryGetValue(targets, out var result))
			{
				FetchSharedComponents(targets, out var shared, out var inconsistent);
				sharedComponents[targets] = shared;
				inconsistentComponents[targets] = inconsistent;
				result = inconsistent;
			}

			return result;
		}

		public static List<RemovedComponent> GetRemovedComponents(GameObject target)
		{
			if (!removedComponents.TryGetValue(target, out var result))
			{
				result = FetchRemovedComponents(target);
				removedComponents.Add(target, result);
			}

			return result;
		}

		public static void Clear()
		{
			sharedComponents.Clear();
			inconsistentComponents.Clear();
			removedComponents.Clear();
		}
	}
}