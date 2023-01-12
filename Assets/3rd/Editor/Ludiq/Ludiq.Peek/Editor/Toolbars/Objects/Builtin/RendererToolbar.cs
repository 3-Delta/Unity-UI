using System.Collections.Generic;
using Ludiq.Peek;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: RegisterObjectToolbar(typeof(Renderer), typeof(RendererToolbar))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class RendererToolbar : ObjectToolbar<Renderer>
	{
		public RendererToolbar(Renderer[] targets) : base(targets) { }

		protected override void UpdateTools(IList<ITool> tools)
		{
			if (mainTool != null)
			{
				tools.Add(mainTool);
			}

			var materialSets = ListPool<List<Material>>.New();

			foreach (var target in targets)
			{
				var materialSet = ListPool<Material>.New();
				target.GetSharedMaterials(materialSet);
				materialSets.Add(materialSet);
			}

			var sharedMaterials = HashSetPool<Material>.New();

			var isFirst = true;

			foreach (var materialSet in materialSets)
			{
				if (isFirst)
				{
					sharedMaterials.UnionWith(materialSet);
					isFirst = false;
				}
				else
				{
					sharedMaterials.IntersectWith(materialSet);
				}

				materialSet.Free();
			}

			foreach (var material in sharedMaterials)
			{
				if (material == null)
				{
					continue;
				}

				var materialTool = EditorToolProvider.GetEditorTool(material);

				tools.Add(materialTool);
			}

			materialSets.Free();
		}
	}
}