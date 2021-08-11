using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Ludiq.PeekCore;
using Ludiq.OdinSerializer;
using Ludiq.PeekCore.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(EditorSerializationUtility))]

namespace Ludiq.PeekCore
{
	public static class EditorSerializationUtility
	{
		static EditorSerializationUtility()
		{
			/*
			var types = new Type[]
			{
				typeof(int),
				typeof(int[]),
				typeof(int[,]),
				typeof(int[,,]),
				typeof(int[][]),
				typeof(List<int>),
				typeof(List<int[]>),
				typeof(List<int[,]>),
				typeof(List<int[,,]>),
				typeof(List<int[][]>),
				typeof(Dictionary<int, int>),
				typeof(Dictionary<int[], int[]>),
				typeof(Dictionary<int[,], int[,]>),
				typeof(Dictionary<int[,,], int[,,]>),
				typeof(Dictionary<int[][], int[][]>),
			};

			foreach (var type in types)
			{
				Debug.Log($"{type.AssemblyQualifiedName}\n{TypeName.SimplifyFast(type.AssemblyQualifiedName)}");
			}
			*/
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Force Reserialize Everything")]
#endif
		public static void ForceReserializeEverything()
		{
			AssetDatabase.ForceReserializeAssets();
			AssetDatabase.SaveAssets();
		}

		//[MenuItem("Assets/Reserialize")]
		public static void ForceReserializeSelection()
		{
			var paths = Selection.objects.Select(AssetDatabase.GetAssetPath).NotNull();
			AssetDatabase.ForceReserializeAssets(paths);
			AssetDatabase.SaveAssets();
		}

#region Policy Testing

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Test Serialization Policies")]
#endif
		public static void TestSerializationPolicies()
		{
			var typesToTest = Codebase.types.Where(t => t.Namespace == "Ludiq" || t.Namespace == "Bolt" || t.Namespace().Root.Name == "UnityEngine").ToHashSet();
			
			float i = 0;

			var matching = 0;

			foreach (var type in typesToTest)
			{
				ProgressUtility.DisplayProgressBar("Test Serialization Policies", type.CSharpName(), i++ / typesToTest.Count);

				if (TestSerializationPolicies(type))
				{
					matching++;
				}
			}
			
			Debug.Log($"Serialization policy test complete. \nSuccessfully matched: {matching} / {typesToTest.Count}");

			ProgressUtility.ClearProgressBar();
		}

		private static bool TestSerializationPolicies(Type type)
		{
			var fullSerializerMembers = fsMetaType
				.Get(new fsConfig(), type)
				.Properties
				.Select(p => p._memberInfo)
				.OrderBy(m => m.MetadataToken)
				.ToList();

			var odinSerializerMembers = FormatterUtilities.GetSerializableMembers(type, SerializationPolicy.instance)
				.OrderBy(m => m.MetadataToken)
				.ToList();

			var matches = fullSerializerMembers.SequenceEqual(odinSerializerMembers);

			if (!matches)
			{
				Debug.LogWarning($"Serialization Policy mismatch on {type}: \n\nFull Serializer members:\n{fullSerializerMembers.Select(m => m.Name).ToLineSeparatedString()}\n\nOdin Serializer members:\n{odinSerializerMembers.Select(m => m.Name).ToLineSeparatedString()}\n");
			}

			return matches;
		}

#endregion
	}
}