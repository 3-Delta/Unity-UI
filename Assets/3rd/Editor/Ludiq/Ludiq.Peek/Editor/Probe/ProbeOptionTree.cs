using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class ProbeOptionTree : FuzzyOptionTree
	{
		private readonly List<(string, List<ProbeOption>)> groups = new List<(string, List<ProbeOption>)>();

		public override bool multithreaded => false;

		public override bool searchable { get; }

		public ProbeOptionTree(IEnumerable<ProbeHit> hits) : base(GUIContent.none)
		{
			Ensure.That(nameof(hits)).IsNotNull(hits);
			
			searchable = hits.Count() > 10;

			var groupedHits = new Dictionary<GameObject, List<ProbeHit>>();

			foreach (var hit in hits)
			{
				if (!groupedHits.TryGetValue(hit.groupGameObject, out var group))
				{
					group = new List<ProbeHit>();
					groupedHits.Add(hit.groupGameObject, group);
				}

				group.Add(hit);
			}

			foreach (var groupedHit in groupedHits)
			{
				var groupLabel = UnityAPI.Await(() => groupedHit.Key.name);
				var groupContents = groupedHit.Value.OrderBy(gh => gh.groupOrder).Select(x => new ProbeOption(x)).ToList();
				groups.Add((groupLabel, groupContents));
			}
		}

		public override IEnumerable<IFuzzyOption> Root()
		{
			foreach (var group in groups)
			{
				yield return Separator(group.Item1);

				foreach (var option in group.Item2)
				{
					yield return option;
				}
			}
		}

		public override IEnumerable<IFuzzyOption> SearchableRoot()
		{
			return Root();
		}
	}
}