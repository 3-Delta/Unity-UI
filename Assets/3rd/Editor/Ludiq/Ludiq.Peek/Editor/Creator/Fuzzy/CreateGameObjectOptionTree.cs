using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class CreateGameObjectOptionTree : FuzzyOptionTree
	{
		private readonly List<CreateGameObjectOption> primitiveOptions;

		private readonly List<MenuFolderOption> primitiveFolderOptions;

		private readonly List<CreateGameObjectOption> assetOptions;

		private readonly List<AssetFolderOption> assetFolderOptions;

		public CreateGameObjectOptionTree(string title = "Create") : base(new GUIContent(title))
		{ 
			primitiveOptions = new List<CreateGameObjectOption>();
			primitiveFolderOptions = new List<MenuFolderOption>();
			assetOptions = new List<CreateGameObjectOption>();
			assetFolderOptions = new List<AssetFolderOption>();
		}

		public override void Prewarm()
		{
			base.Prewarm();

			UnityAPI.Await
			(
				() =>
				{
					if (PeekPlugin.Configuration.createPrimitives)
					{
						var primitivePaths = EditorMainMenu
							.GetSubmenus("GameObject")
							.Where(mi => !(createMenuBlacklist.Contains(mi) || PeekPlugin.Configuration.createMenuBlacklist.Contains(mi)))
							.ToArray();

						var primitiveFolders = HashSetPool<string>.New();

						foreach (var primitivePath in primitivePaths)
						{
							var primitiveOption = CreateGameObjectOption.Primitive(primitivePath);
							var primitiveFolder = primitiveOption.primitiveFolder;
							
							primitiveOptions.Add(primitiveOption);
							
							if (primitiveFolder != "GameObject" && !primitiveFolders.Contains(primitiveFolder))
							{
								primitiveFolderOptions.Add(new MenuFolderOption(primitiveFolder));
								primitiveFolders.Add(primitiveFolder);
							}
						}

						primitiveFolders.Free();
					}

					if (PeekPlugin.Configuration.createPrefabs)
					{
						foreach (var prefabResult in AssetDatabaseUtility.FindAssets("t:prefab").Where(IncludeAsset))
						{
							assetOptions.Add(CreateGameObjectOption.Prefab(prefabResult));
						}
					}

					if (PeekPlugin.Configuration.createModels)
					{
						foreach (var modelResult in AssetDatabaseUtility.FindAssets("t:model").Where(IncludeAsset))
						{
							assetOptions.Add(CreateGameObjectOption.Model(modelResult));
						}
					}

					if (PeekPlugin.Configuration.createSprites)
					{
						foreach (var spriteResult in AssetDatabaseUtility.FindAssets("t:sprite").Where(IncludeAsset))
						{
							assetOptions.Add(CreateGameObjectOption.Sprite(spriteResult));
						}
					}
					
					var assetFolders = HashSetPool<string>.New();

					foreach (var assetOption in assetOptions)
					{
						var assetFolder = assetOption.assetFolder;
						
						while (!string.IsNullOrEmpty(assetFolder))
						{
							if (!assetFolders.Contains(assetFolder))
							{
								assetFolderOptions.Add(new AssetFolderOption(assetFolder));
								assetFolders.Add(assetFolder);
							}
							else
							{
								break;
							}

							assetFolder = PathUtility.NaiveParent(assetFolder);
						}
					}

					assetFolders.Free();
				}
			);
		}

		private bool IncludeAsset(HierarchyPropertyCache asset)
		{
			if (PeekPlugin.Configuration.createFolderBlacklist.Count == 0)
			{
				return true;
			}

			var path = PathUtility.NaiveNormalize(asset.assetPath).Trim().ToLowerInvariant();
			
			foreach (var blacklisted in PeekPlugin.Configuration.createFolderBlacklist)
			{
				var _blacklisted = PathUtility.NaiveNormalize(blacklisted).Trim().ToLowerInvariant();
				
				if (PathUtility.NaiveContains(_blacklisted, path, true))
				{
					return false;
				}
			}

			return true;
		}

		private static readonly HashSet<string> createMenuBlacklist = new HashSet<string>()
		{
			"GameObject/",

			"GameObject/Create Prefab",
			"GameObject/Create Parent",
			"GameObject/Create Sibling",
			"GameObject/Create Child",
			"GameObject/Replace",

			"GameObject/Create Empty Child",
			"GameObject/Center On Children",
			"GameObject/Make Parent",
			"GameObject/Clear Parent",
			"GameObject/Set as first sibling",
			"GameObject/Set as last sibling",
			"GameObject/Move To View",
			"GameObject/Align With View",
			"GameObject/Align View to Selected",
			"GameObject/Toggle Active State",
		};

		public override IEnumerable<IFuzzyOption> Root()
		{
			if (primitiveOptions.Any())
			{
				yield return Separator("Primitives");

				foreach (var option in PrimitiveChildren("GameObject"))
				{
					yield return option;
				}
			}

			if (assetOptions.Any())
			{
				yield return Separator("Assets");

				foreach (var option in AssetChildren("Assets"))
				{
					yield return option;
				}
			}
		}

		private IEnumerable<IFuzzyOption> AssetChildren(string path)
		{
			// TODO: Order by filename

			foreach (var assetFolderOption in assetFolderOptions)
			{
				if (PathUtility.NaiveContains(path, assetFolderOption.path))
				{
					yield return assetFolderOption;
				}
			}

			foreach (var assetOption in assetOptions)
			{
				if (PathUtility.NaiveContains(path, assetOption.assetPath))
				{
					yield return assetOption;
				}
			}
		}

		private IEnumerable<IFuzzyOption> PrimitiveChildren(string path)
		{
			foreach (var primitiveFolderOption in primitiveFolderOptions)
			{
				if (PathUtility.NaiveContains(path, primitiveFolderOption.path))
				{
					yield return primitiveFolderOption;
				}
			}

			foreach (var primitiveOption in primitiveOptions)
			{
				if (PathUtility.NaiveContains(path, primitiveOption.primitivePath))
				{
					yield return primitiveOption;
				}
			}
		}

		public override IEnumerable<IFuzzyOption> Children(IFuzzyOption parent, bool ordered)
		{
			if (parent is AssetFolderOption assetFolderOption)
			{
				return AssetChildren(assetFolderOption.value);
			}
			else if (parent is MenuFolderOption folderOption)
			{
				return PrimitiveChildren(folderOption.value);
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
			return LinqUtility.Concat<IFuzzyOption>(primitiveOptions, assetOptions, primitiveFolderOptions, assetFolderOptions);
		}

		#endregion
	}
}