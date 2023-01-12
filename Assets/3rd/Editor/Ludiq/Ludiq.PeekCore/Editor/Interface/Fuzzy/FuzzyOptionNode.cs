using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class FuzzyOptionNode
	{
		public FuzzyOptionNode(IFuzzyOption option)
		{
			Ensure.That(nameof(option)).IsNotNull(option);

			this.option = option;
			children = new List<FuzzyOptionNode>();
			labelText = option.label;
		}

		public FuzzyOptionNode(IFuzzyOption option, string label)
		{
			Ensure.That(nameof(option)).IsNotNull(option);

			this.option = option;
			children = new List<FuzzyOptionNode>();
			labelText = label ?? string.Empty;
		}

		public bool EvaluateHasChildren(IFuzzyOptionTree tree)
		{
			if (!hasChildren.HasValue)
			{
				hasChildren = tree.Children(option).Any();
			}

			return hasChildren.Value;
		}

		#region Data

		public IFuzzyOption option { get; }
		public string labelText { get; private set; }
		public List<FuzzyOptionNode> children { get; }
		public bool? hasChildren { get; set; }
		public bool isPopulated { get; set; }
		public bool isLoading { get; set; } = true;

		#endregion

		#region Interaction

		public Vector2 scroll { get; set; }
		public int selectedIndex { get; set; }

		#endregion

		#region Drawing
		
		public bool isDrawable { get; private set; }
		public bool isPositionable { get; private set; }
		public GUIContent label { get; private set; }
		public bool dim { get; private set; }
		public float width { get; private set; }
		public EditorTexture icon { get; private set; }
		public EditorTexture[] icons { get; private set; }

		public void EnsureDrawable()
		{
			if (!isDrawable)
			{
				PrepareDrawing();
			}
		}

		public void EnsurePositionable()
		{
			if (!isPositionable)
			{
				PreparePositioning();
			}
		}

		private void PreparePositioning()
		{
			width = FuzzyWindow.Styles.optionForeground.CalcSize(new GUIContent(labelText)).x;

			isPositionable = true;
		}

		private void PrepareDrawing()
		{
			icon = option.Icon();
			icons = option.Icons().ToArray();

			label = new GUIContent(labelText);
			dim = option.dim;

			width = 
				(icons.Length * IconSize.Small) + 
				(icons.Length - 1 * FuzzyWindow.Styles.spaceBetweenIcons) + 
				FuzzyWindow.Styles.spaceAfterIcons + 
				FuzzyWindow.Styles.optionForeground.CalcSize(label).x;

			isDrawable = true;
		}

		#endregion
	}
}