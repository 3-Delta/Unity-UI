using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UEditor = UnityEditor.Editor;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class ProbeOption : FuzzyOption<ProbeHit>
	{
		public ProbeOption(ProbeHit hit) : base(FuzzyOptionMode.Leaf)
		{
			Ensure.That(nameof(hit)).IsNotNull(hit);
			
			value = hit;

			label = hit.label ?? UnityAPI.Await(() => hit.gameObject.name);
		}

		public override IEnumerable<EditorTexture> Icons()
		{
			if (value.gameObject != null)
			{
				if (PeekPlugin.Configuration.enablePreviewIcons && 
				    PreviewUtility.TryGetPreview(value.gameObject, out var preview) && 
				    !AssetPreview.IsLoadingAssetPreview(value.gameObject.GetInstanceID()))
				{
					yield return EditorTexture.Single(preview);
				}
				else
				{
					yield return value.gameObject.Icon();
				}
			}

			if (value.icon != null)
			{
				yield return value.icon;
			}
		}

		public override void OnFocusEnter(FuzzyOptionNode node)
		{
			value.OnFocusEnter();
		}

		public override void OnFocusLeave(FuzzyOptionNode node)
		{
			value.OnFocusLeave();
		}
	}
}