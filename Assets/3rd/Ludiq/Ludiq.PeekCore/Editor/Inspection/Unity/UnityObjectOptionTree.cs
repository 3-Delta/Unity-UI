using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public class UnityObjectOptionTree : FuzzyOptionTree
	{
		public UnityObjectOptionTree(Type type, Scene? scene, string title = null) : this
		(
			type,
			type.DisplayName(),
			type.Icon(),
			scene,
			true,
			uo => true,
			title ?? type.DisplayName()
		) { }

		public UnityObjectOptionTree
		(
			Type type, 
			string typeLabel,
			EditorTexture typeIcon,
			Scene? scene, 
			bool allowAssetObjects, 
			Func<UnityObject, bool> predicate = null, 
			string title = "Object"
		) : base(new GUIContent(title))
		{
			this.type = type ?? typeof(UnityObject);
			this.typeLabel = typeLabel;
			this.typeIcon = typeIcon;
			this.scene = scene;
			this.allowAssetObjects = allowAssetObjects;
			this.predicate = predicate;
		}

		private readonly string typeLabel;

		private readonly EditorTexture typeIcon;

		private readonly Type type;

		private readonly Scene? scene;

		private readonly bool allowAssetObjects;

		private readonly Func<UnityObject, bool> predicate;

		private Dictionary<UnityObject, string> sceneObjects;

		private Dictionary<UnityObject, string> assetObjects;

		public override void Prewarm()
		{
			base.Prewarm();

			if (scene != null)
			{
				sceneObjects = UnityAPI.Await
				(
					() =>
					UnityObjectUtility
						.FindObjectsOfTypeInScene(type, scene.Value)
						.Where(uo => predicate?.Invoke(uo) ?? true)
						.ToDictionary(uo => uo, uo => uo.name)
				);
			}

			if (allowAssetObjects)
			{
				assetObjects = UnityAPI.Await
				(
					() => AssetUtility.FindAllAssetsOfType(type)
						.Where(uo => predicate?.Invoke(uo) ?? true)
						.ToDictionary(uo => uo, uo => uo.name)
				);
			}
		}

		private UnityObjectOption SceneOption(UnityObject uo)
		{
			return new UnityObjectOption(sceneObjects[uo], typeLabel, typeIcon, uo, true, FuzzyOptionMode.Leaf);
		}

		private UnityObjectOption AssetOption(UnityObject uo)
		{
			if (!assetObjects.TryGetValue(uo, out var label))
			{
				label = UnityAPI.Await(() => uo.name);
			}

			return new UnityObjectOption(label, typeLabel, typeIcon, uo, false, FuzzyOptionMode.Leaf);
		}

		private UnityObjectOption NullOption()
		{
			return new UnityObjectOption("None", typeLabel, typeIcon, null, null, FuzzyOptionMode.Leaf);
		}

		public override IEnumerable<IFuzzyOption> Root()
		{
			if (predicate == null || predicate(null))
			{
				yield return NullOption();
			}

			var showSeparators = scene != null && allowAssetObjects;

			if (scene != null && sceneObjects.Count > 0)
			{
				if (showSeparators)
				{
					yield return Separator("Scene");
				}

				foreach (var sceneObject in sceneObjects.Keys)
				{
					yield return SceneOption(sceneObject);
				}
			}

			if (allowAssetObjects && assetObjects.Count > 0)
			{
				if (showSeparators)
				{
					yield return Separator("Assets");
				}

				if (assetObjects.Count < 25)
				{
					foreach (var assetObject in assetObjects.Keys)
					{
						yield return AssetOption(assetObject);
					}
				}
				else
				{
					foreach (var option in Children("Assets"))
					{
						yield return option;
					}
				}
			}
		}

		private IEnumerable<IFuzzyOption> Children(string pathFromProject)
		{
			var fullPath = Path.Combine(Paths.project, pathFromProject);

			foreach (var directory in Directory.EnumerateDirectories(fullPath))
			{
				yield return new AssetFolderOption(PathUtility.FromProject(directory), FuzzyOptionMode.Branch);
			}

			foreach (var assetChild in UnityAPI.Await(() => LoadedAssetChildren(fullPath).ToArray()))
			{
				yield return assetChild;
			}
		}

		private IEnumerable<IFuzzyOption> LoadedAssetChildren(string fullPath)
		{
			foreach (var file in Directory.EnumerateFiles(fullPath))
			{
				var assetPath = PathUtility.FromProject(file);
				var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);

				if (asset != null && !asset.IsHidden())
				{
					yield return AssetOption(asset);
				}
			}
		}

		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			if (parent is AssetFolderOption folderOption)
			{
				return Children(folderOption.value);
			}
			else
			{
				return Enumerable.Empty<IFuzzyOption>();
			}
		}

		#region Search
		
		public override bool searchable { get; } = true;

		public override IEnumerable<IFuzzyOption> SearchableRoot()
		{
			if (scene != null)
			{
				foreach (var sceneObject in sceneObjects.Keys)
				{
					yield return SceneOption(sceneObject);
				}
			}

			if (allowAssetObjects)
			{
				foreach (var assetObject in assetObjects.Keys)
				{
					yield return AssetOption(assetObject);
				}
			}
		}

		#endregion
	}
}