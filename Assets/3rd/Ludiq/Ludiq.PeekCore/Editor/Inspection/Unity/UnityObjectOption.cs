using Ludiq.PeekCore;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UEditor = UnityEditor.Editor;

[assembly: RegisterFuzzyOption(typeof(UnityObject), typeof(UnityObjectOption))]

namespace Ludiq.PeekCore
{
	public class UnityObjectOption : FuzzyOption<UnityObject>
	{
		private string typeLabel;
		private EditorTexture typeIcon;
		private bool? isSceneBound;

		private static UnityObject editorTarget;
		private static UEditor editor;
		
		public UnityObjectOption(string label, string typeLabel, EditorTexture typeIcon, UnityObject uo, bool? isSceneBound, FuzzyOptionMode mode) : base(mode)
		{
			this.label = label;
			this.typeLabel = typeLabel;
			this.typeIcon = typeIcon;
			this.isSceneBound = isSceneBound;

			value = uo;

			if (!ReferenceEquals(uo, null))
			{
				getIcon = uo.Icon;
			}
			else
			{
				icon = typeIcon;
			}
		}

		public override bool hasFooter => value != null && value.GetComponentInChildren<Renderer>() != null;

		public override float GetFooterHeight(FuzzyOptionNode node, float width)
		{
			return 128;
		}

		public override void OnFooterGUI(FuzzyOptionNode node, Rect position)
		{
			if (editorTarget != value)
			{
				editorTarget = value;
				UEditor.CreateCachedEditor(editorTarget, null, ref editor);
			}

			if (editor != null)
			{
				editor.DrawPreview(position);
			}
		}

		public override string SearchResultLabel(string query)
		{
			var label = base.SearchResultLabel(query);

			string sourceDescriptor = null;

			if (isSceneBound.HasValue)
			{
				if (isSceneBound == true)
				{
					sourceDescriptor = "Scene";
				}
				else
				{
					if (value is Component)
					{
						sourceDescriptor = "Prefab";
					}
					else if (value is GameObject)
					{
						sourceDescriptor = "Prefab";
					}
					else
					{
						sourceDescriptor = "Asset";
					}
				}
			}

			if (sourceDescriptor != null)
			{
				label += $" <color=#{ColorPalette.unityForegroundDim.ToHexString()}>({sourceDescriptor})</color>";
			}

			return label;
		}
	}
}