using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: RegisterObjectToolbar(typeof(GameObject), typeof(GameObjectToolbar))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class GameObjectToolbar : ObjectToolbar<GameObject>
	{
		public GameObjectToolbar(GameObject[] targets) : base(targets) { }

		private AddComponentTool addComponentTool;

		private static readonly InconsistentComponentsTool inconsistentComponentsTool = new InconsistentComponentsTool();

		private MergedTool mergedScriptsTool;

		private readonly LazyDictionary<RemovedComponent, RemovedComponentTool> removedComponentTools = new LazyDictionary<RemovedComponent, RemovedComponentTool>(rc => new RemovedComponentTool(rc));

		private bool hasPrefabTarget;

		public override void Initialize()
		{
			base.Initialize();

			if (mainTool is GameObjectEditorTool gameObjectEditorTool)
			{
				gameObjectEditorTool.showText = true;
			}

			var editable = true;

			mergedScriptsTool = new MergedTool
			{
				label = "Scripts...",
				tooltip = "Scripts...",
				icon = PeekPlugin.Icons.script?[IconSize.Small],
				expandable = true
			};

			foreach (var target in targets)
			{
				if (PrefabUtility.IsPartOfPrefabAsset(target))
				{
					hasPrefabTarget = true;
				}

				if (target.HasHideFlag(HideFlags.NotEditable))
				{
					editable = false;
					break;
				}
			}

			if (editable)
			{
				addComponentTool = new AddComponentTool(targets);
			}
		}

		private bool ShouldShowComponent(Component c)
		{
			return c != null &&
			       !c.HasHideFlag(HideFlags.HideInInspector) && 
			       !(c is ParticleSystemRenderer); // Yep, this is also hardcoded in the Unity source... 
		}

		protected override void UpdateTools(IList<ITool> tools)
		{
			if (mainTool != null)
			{
				tools.Add(mainTool);
			}

			if (hasPrefabTarget)
			{
				return;
			}

			mergedScriptsTool.tools.Clear();

			foreach (var components in ComponentCache.GetSharedComponents(targets))
			{
				// Being extra picky about allocation here
				var targets = ArrayPool<UnityObject>.New(components.Count);

				try
				{
					var hide = false;

					for (var i = 0; i < components.Count; i++)
					{
						targets[i] = components[i];
						
						if (!ShouldShowComponent(components[i]))
						{
							hide = true;
							break;
						}
					}

					if (hide)
					{
						continue;
					}

					var componentStrip = ObjectToolbarProvider.GetToolbar(targets);
					
					if (componentStrip.isValid)
					{
						componentStrip.Update();

						foreach (var componentTool in componentStrip)
						{
							if (componentTool is IEditorTool componentEditorTool && typeof(MonoBehaviour).IsAssignableFrom(componentEditorTool.targetType))
							{
								var hasIcon = componentEditorTool.targetType.Icon()?[IconSize.Small] != PeekPlugin.Icons.script?[IconSize.Small];
								var merge = PeekPlugin.Configuration.scriptsMerging.Merge(hasIcon);

								if (merge)
								{
									mergedScriptsTool.tools.Add(componentEditorTool);
								}
								else
								{
									tools.Add(componentEditorTool);
								}
							}
							else
							{
								tools.Add(componentTool);
							}
						}
					}
				}
				finally
				{
					targets.Free();
				}
			}

			if (mergedScriptsTool.tools.Count > 1)
			{
				tools.Add(mergedScriptsTool);
			}
			else if (mergedScriptsTool.tools.Count == 1)
			{
				tools.Add(mergedScriptsTool.tools[0]);
			}

			if (hasSingleTarget && PrefabUtility.IsPartOfPrefabInstance(target))
			{
				foreach (var removedComponent in ComponentCache.GetRemovedComponents(target))
				{
					tools.Add(removedComponentTools[removedComponent]);
				}
			}

			if (ComponentCache.GetInconsistentComponents(targets).Count > 0)
			{
				tools.Add(inconsistentComponentsTool);
			}

			if (addComponentTool != null)
			{
				tools.Add(addComponentTool);
			}
		}
	}
}