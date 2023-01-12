using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class EditorUnityObjectUtility
	{
		public static bool newPrefabWorkflow => EditorApplicationUtility.unityVersion >= "2018.3.0";

		static EditorUnityObjectUtility()
		{
			try
			{
				UnityTypeType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.UnityType", true);
				UnityTypeType_FindTypeByNameCaseInsensitive = UnityTypeType.GetMethod("FindTypeByNameCaseInsensitive", BindingFlags.Static | BindingFlags.Public);
				UnityTypeType_persistentTypeID = UnityTypeType.GetProperty("persistentTypeID", BindingFlags.Instance | BindingFlags.Public);

				if (UnityTypeType_FindTypeByNameCaseInsensitive == null)
				{
					throw new MissingMemberException(UnityTypeType.ToString(), "FindTypeByNameCaseInsensitive");
				}

				if (UnityTypeType_persistentTypeID == null)
				{
					throw new MissingMemberException(UnityTypeType.ToString(), "persistentTypeID");
				}

#if !UNITY_2019_1_OR_NEWER
				EditorUtility_IsDirty = typeof(EditorUtility).GetMethod("IsDirty", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(int) }, null);

				if (EditorUtility_IsDirty == null)
				{
					throw new MissingMemberException(typeof(EditorUtility).ToString(), "IsDirty");
				}
#endif
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		public static IEnumerable<Type> GetUnityTypes(UnityObject target)
		{
			Ensure.That(nameof(target)).IsNotNull(target);

			if (target.IsComponentHolder())
			{
				yield return typeof(GameObject);

				foreach (var componentType in target.GetComponents<Component>().NotNull().Select(c => c.GetType()).Distinct())
				{
					yield return componentType;
				}
			}
			else
			{
				yield return target.GetType();
			}
		}

		#region Prefabs

		public static UnityObject GetPrefabDefinition(this UnityObject uo)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);

			return PrefabUtility.GetCorrespondingObjectFromSource(uo);
		}

		private static UnityObject GetPrefabInstance(this UnityObject uo)
		{
			return PrefabUtility.GetPrefabInstanceHandle(uo);
		}

		public static bool IsPrefabInstance(this UnityObject uo)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);

			return PrefabUtility.IsPartOfPrefabInstance(uo);
		}

		public static bool IsPrefabDefinition(this UnityObject uo)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);

			// https://forum.unity.com/threads/editorgui-objectfield-allowsceneobjects-in-isolation-mode.610564/
			return PrefabUtility.IsPartOfPrefabAsset(uo);// || PrefabStageUtility.GetPrefabStage(uo.GameObject()) != null;
		}

		public static bool IsConnectedPrefabInstance(this UnityObject uo)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);

			return PrefabUtility.IsPartOfPrefabInstance(uo) && !PrefabUtility.IsDisconnectedFromPrefabAsset(uo);
		}

		public static bool IsDisconnectedPrefabInstance(this UnityObject uo)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);

			return PrefabUtility.IsPartOfPrefabInstance(uo) && PrefabUtility.IsDisconnectedFromPrefabAsset(uo);
		}

		public static bool IsSceneBound(this UnityObject uo)
		{
			Ensure.That(nameof(uo)).IsNotNull(uo);
			
			return !EditorUtility.IsPersistent(uo);

			// return
			// 	(uo is GameObject go && !IsPrefabDefinition(go)) ||
			// 	(uo is Component component && !IsPrefabDefinition(component.gameObject));
		}

		#endregion


		#region Dirty
		
#if !UNITY_2019_1_OR_NEWER
		private static readonly MethodInfo EditorUtility_IsDirty;
#endif

		public static bool IsDirty(UnityObject uo)
		{
			if (uo.IsSceneBound())
			{
				return SceneManager.GetActiveScene().isDirty;
			}
			else
			{
#if UNITY_2019_1_OR_NEWER
				return EditorUtility.IsDirty(uo);
#else
				try
				{
					return (bool)EditorUtility_IsDirty.InvokeOptimized(null, uo.GetInstanceID());
				}
				catch (Exception ex)
				{
					throw new UnityEditorInternalException(ex);
				}
#endif
			}
		}

		#endregion


		#region Class

		public const int MonoBehaviourClassID = 114;
		private static readonly Type UnityTypeType; // internal sealed class UnityType
		private static readonly PropertyInfo UnityTypeType_persistentTypeID; // public int persistentTypeID { get; private set; }
		private static readonly MethodInfo UnityTypeType_FindTypeByNameCaseInsensitive; // public static extern int StringToClassIDCaseInsensitive(string classString);

		public static int GetClassID(Type type)
		{
			if (typeof(MonoBehaviour).IsAssignableFrom(type) || typeof(ScriptableObject).IsAssignableFrom(type))
			{
				return MonoBehaviourClassID;
			}

			try
			{
				var unityType = UnityTypeType_FindTypeByNameCaseInsensitive.Invoke(null, new object[] { type.Name });

				if (unityType == null)
				{
					throw new Exception($"Could not find UnityType for '{type}'.");
				}

				return (int)UnityTypeType_persistentTypeID.GetValue(unityType, null);
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		public static string GetScriptClass(Type type)
		{
			if (!typeof(MonoBehaviour).IsAssignableFrom(type) && !typeof(ScriptableObject).IsAssignableFrom(type))
			{
				throw new NotSupportedException("Trying to get script class of a non-script type.");
			}

			return type.Name;
		}

		#endregion
	}
}