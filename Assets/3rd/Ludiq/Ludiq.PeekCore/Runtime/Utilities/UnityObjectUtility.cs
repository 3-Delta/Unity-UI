using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class UnityObjectUtility
	{
		static UnityObjectUtility()
		{
			cachedPtrAccessor = typeof(UnityObject).GetField("m_CachedPtr", BindingFlags.NonPublic | BindingFlags.Instance).Prewarm();
		}

		private static readonly IOptimizedAccessor cachedPtrAccessor;
	
		public static bool IsDestroyed(this UnityObject target)
		{
			// Checks whether a Unity object is not actually a null reference,
			// but a rather destroyed native instance.

			return !ReferenceEquals(target, null) && target == null;
		}

		public static bool IsUnityNull(this object obj)
		{
			// Checks whether an object is null or Unity pseudo-null
			// without having to cast to UnityEngine.Object manually
			
			return obj == null || ((obj is UnityObject) && ((UnityObject)obj) == null);
		}

		public static bool IsUnityNullSafe(this UnityObject uo)
		{
			// Allowed off the main thread

			if (ReferenceEquals(uo, null))
			{
				return true;
			}

			return (IntPtr)cachedPtrAccessor.GetValue(uo) == IntPtr.Zero;
		}

		public static int GetInstanceIDSafe(this UnityObject uo)
		{
			// Allowed off the main thread

			return uo.GetHashCode();
		}

		public static object UnityNullCoalesce(this object obj, object fallback)
		{
			return !IsUnityNull(obj) ? obj : fallback;
		}

		public static string ToSafeString(this UnityObject uo)
		{
			if (ReferenceEquals(uo, null))
			{
				return "(null)";
			}

			if (!UnityThread.allowsAPI)
			{
				if (uo.IsUnityNullSafe())
				{
					return $"{uo.GetType().Name}#{uo.GetInstanceIDSafe()} (Null)";
				}
				else
				{
					return $"{uo.GetType().Name}#{uo.GetInstanceIDSafe()}";
				}
			}

			if (uo == null)
			{
				return $"#{uo.GetInstanceID()} (Destroyed)";
			}
			
			try
			{
				return $"{uo.name} ({uo.GetType().Name})";
			}
			catch (Exception ex)
			{
				return $"({ex.GetType().Name} in ToString: {ex.Message})";
			}
		}

		public static string ToSafeString(this object obj)
		{
			if (obj == null)
			{
				return "(null)";
			}

			if (obj is UnityObject uo)
			{
				return uo.ToSafeString();
			}

			try
			{
				return obj.ToString();
			}
			catch (Exception ex)
			{
				return $"({ex.GetType().Name} in ToString: {ex.Message})";
			}
		}

		public static T AsUnityNull<T>(this T obj) where T : UnityObject
		{
			// Converts a Unity pseudo-null to a real null, allowing for coalesce operators.
			// e.g.: destroyedObject.AsUnityNull() ?? otherObject

			if (obj == null)
			{
				return null;
			}

			return obj;
		}
		
		// Avoids alloc
		public static bool HasHideFlag(this UnityObject uo, HideFlags flag)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);

			var hideFlags = uo.hideFlags;

			return (hideFlags & flag) == flag;
		}

		public static bool IsHidden(this UnityObject uo)
		{
			return uo.HasHideFlag(HideFlags.HideAndDontSave);
		}

		public static bool TrulyEqual(UnityObject a, UnityObject b)
		{
			// This method is required when checking two references
			// against one another, where one of them might have been destroyed.
			// It is not required when checking against null.

			// This is because Unity does not compare alive state
			// in the CompareBaseObjects method unless one of the two
			// operators is actually the null literal.

			// From the source:
			/*
		      bool lhsIsNull = (object) lhs == null;
		      bool rhsIsNull = (object) rhs == null;
		      if (rhsIsNull && lhsIsNull)
		        return true;
		      if (rhsIsNull)
		        return !Object.IsNativeObjectAlive(lhs);
		      if (lhsIsNull)
		        return !Object.IsNativeObjectAlive(rhs);
		      return lhs.m_InstanceID == rhs.m_InstanceID;
			 */

			// As we can see, Object.IsNativeObjectAlive is not compared
			// across the two objects unless one of the operands is actually null.
			// But it can happen, for example when exiting play mode.
			// If we stored a static reference to a scene object that was destroyed,
			// the reference won't get cleared because assembly reloads don't happen
			// when exiting playmode. But the instance ID of the object will stay
			// the same, because it only gets reserialized. So if we compare our
			// stale reference that was destroyed to a new reference to the object,
			// it will return true, even though one reference is alive and the other isn't.

			if (a != b)
			{
				return false;
			}

			if ((a == null) != (b == null))
			{
				return false;
			}

			return true;
		}

		public static IEnumerable<T> NotUnityNull<T>(this IEnumerable<T> enumerable) where T : UnityObject
		{
			return enumerable.Where(i => i != null);
		}
		
		public static IEnumerable<UnityObject> FindObjectsOfTypeInScene(Type type, bool includeInactive = true)
		{
			return FindObjectsOfTypeInScene(type, SceneManager.GetActiveScene(), includeInactive);
		}

		public static IEnumerable<UnityObject> FindObjectsOfTypeInScene(Type type, Scene scene, bool includeInactive = true)
		{
			if (type != typeof(UnityObject) && !ComponentHolderProtocol.IsComponentHolderType(type))
			{
				yield break;
			}

			if (scene.isLoaded)
			{
				foreach (var rootGameObject in scene.GetRootGameObjects())
				{
					IEnumerable<UnityObject> children;

					if (type == typeof(GameObject) || type == typeof(UnityObject))
					{
						children = rootGameObject.GetComponentsInChildren(typeof(Transform), includeInactive).Select(t => t.gameObject);
					}
					else if (typeof(Component).IsAssignableFrom(type))
					{
						children = rootGameObject.GetComponentsInChildren(type, includeInactive);
					}
					else
					{
						throw new NotSupportedException($"Cannot find scene objects of type {type}.");
					}

					foreach (var child in children)
					{
						yield return child;
					}
				}
			}
		}

		public static IEnumerable<UnityObject> FindObjectsOfTypeInAllScenes(Type type, bool includeInactive = true)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				
				foreach (var sceneObject in FindObjectsOfTypeInScene(type, scene, includeInactive))
				{
					yield return sceneObject;
				}
			}
		}

		public static IEnumerable<T> FindObjectsOfTypeInScene<T>(bool includeInactive = true)
		{
			return FindObjectsOfTypeInScene(typeof(T), includeInactive).Cast<T>();
		}

		public static IEnumerable<T> FindObjectsOfTypeInScene<T>(Scene scene, bool includeInactive = true)
		{
			return FindObjectsOfTypeInScene(typeof(T), scene, includeInactive).Cast<T>();
		}

		public static IEnumerable<T> FindObjectsOfTypeInAllScenes<T>(bool includeInactive = true)
		{
			return FindObjectsOfTypeInAllScenes(typeof(T), includeInactive).Cast<T>();
		}
	}
}