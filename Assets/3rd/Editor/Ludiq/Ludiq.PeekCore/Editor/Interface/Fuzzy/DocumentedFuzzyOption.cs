using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class DocumentedFuzzyOption<T> : FuzzyOption<T>
	{
		public IFuzzyOptionDocumentation documentation { get; protected set; }

		public bool zoom { get; protected set; }

		private bool ZoomIcon(FuzzyOptionNode node) => zoom && node.icon != null && (!string.IsNullOrWhiteSpace(documentation.summary) || !string.IsNullOrWhiteSpace(documentation.remarks));

		public bool showType { get; protected set; }

		public override bool hasFooter => documentation != null;
		
		public override float GetFooterHeight(FuzzyOptionNode node, float width)
		{
			if (ZoomIcon(node))
			{
				width -= Styles.zoomSize + Styles.zoomSpacing;
			}

			var height = 0f;

			width -= 2;

			if (documentation.summary != null)
			{
				height += GetSummaryHeight(width);
			}

			foreach (var parameter in documentation.parameters)
			{
				height += GetParameterHeight(parameter, width);
			}

			if (documentation.returns != null)
			{
				height += GetReturnsHeight(width);
			}

			if (documentation.remarks != null)
			{
				height += GetRemarksHeight(width);
			}
			
			if (ZoomIcon(node))
			{
				return Mathf.Max(Styles.zoomSize + 2 * Styles.zoomSpacing, height);
			}

			return height;
		}

		public override void OnFooterGUI(FuzzyOptionNode node, Rect position)
		{
			if (ZoomIcon(node))
			{
				var zoomPosition = new Rect
				(
					position.x + Styles.zoomSpacing,
					position.y + Styles.zoomSpacing,
					Styles.zoomSize,
					Styles.zoomSize
				);

				position.x += Styles.zoomSize + Styles.zoomSpacing;
				position.width -= Styles.zoomSize + Styles.zoomSpacing;

				GUI.DrawTexture(zoomPosition, Icon()[IconSize.Medium]);
			}

			var y = position.y;

			if (documentation.summary != null)
			{
				var summaryPosition = new Rect
				(
					position.x,
					y,
					position.width,
					GetSummaryHeight(position.width)
				);

				OnSummaryGUI(summaryPosition);

				y = summaryPosition.yMax;
			}

			if (documentation.parameters.Any())
			{
				var parameterPosition = new Rect
				(
					position.x,
					y,
					position.width,
					0
				);

				foreach (var parameter in documentation.parameters)
				{
					parameterPosition.height = GetParameterHeight(parameter, position.width);

					OnParameterGUI(parameterPosition, parameter);

					parameterPosition.y += parameterPosition.height;

					y = parameterPosition.y;
				}
			}

			if (documentation.returns != null)
			{
				var returnsPosition = new Rect
				(
					position.x,
					y,
					position.width,
					GetReturnsHeight(position.width)
				);

				OnReturnsGUI(returnsPosition);

				y = returnsPosition.yMax;
			}

			if (documentation.remarks != null)
			{
				var remarksPosition = new Rect
				(
					position.x,
					y,
					position.width,
					GetRemarksHeight(position.width)
				);

				OnRemarksGUI(remarksPosition);

				y = remarksPosition.yMax;
			}
		}

		private float GetSummaryHeight(float width)
		{
			return Styles.summary.CalcHeight(new GUIContent(documentation.summary), width);
		}

		private void OnSummaryGUI(Rect summaryPosition)
		{
			EditorGUI.LabelField(summaryPosition, documentation.summary, Styles.summary);
		}

		private float GetParameterReturnsHeight(GUIContent label, float width)
		{
			width -= IconSize.Small + Styles.iconSpacing;

			return Styles.parameterSummary.CalcHeight(label, width);
		}

		private void OnParameterReturnsGUI(GUIContent label, Rect position)
		{
			var x = position.x + Styles.parameterSummary.padding.left;
			var width = position.width;

			var icon = label.image;

			if (icon != null)
			{
				var iconPosition = new Rect
				(
					x,
					position.y - 1,
					IconSize.Small,
					IconSize.Small
				);

				x += iconPosition.width + Styles.iconSpacing;
				width -= iconPosition.width + Styles.iconSpacing;

				GUI.DrawTexture(iconPosition, icon);
			}

			var labelPosition = new Rect
			(
				x,
				position.y,
				width,
				position.height
			);

			GUI.Label(labelPosition, label, Styles.parameterSummary);
		}

		private float GetParameterHeight(string parameter, float width)
		{
			return GetParameterReturnsHeight(GetParameterLabel(parameter), width);
		}

		private GUIContent GetParameterLabel(string parameter)
		{
			var label = new GUIContent();

			label.text = $"<b>{documentation.GetParameterName(parameter)}: </b>{documentation.GetParameterSummary(parameter)}";
			
			var type = documentation.GetParameterType(parameter);

			if (showType && type != null)
			{
				label.text += $" ({type})";
			}

			label.image = documentation.GetParameterIcon(parameter)?[IconSize.Small];

			return label;
		}

		private GUIContent GetReturnsLabel()
		{
			var label = new GUIContent();

			label.text = $"<b>Returns: </b>{documentation.returns}";
			
			if (showType && documentation.returnType != null)
			{
				label.text += $" ({documentation.returnType})";
			}

			label.image = documentation.returnIcon?[IconSize.Small];

			return label;
		}

		private void OnParameterGUI(Rect parameterPosition, string parameter)
		{
			OnParameterReturnsGUI(GetParameterLabel(parameter), parameterPosition);
		}

		private float GetReturnsHeight(float width)
		{
			return GetParameterReturnsHeight(GetReturnsLabel(), width);
		}

		private void OnReturnsGUI(Rect returnsPosition)
		{
			OnParameterReturnsGUI(GetReturnsLabel(), returnsPosition);
		}

		private float GetRemarksHeight(float width)
		{
			return Styles.remarks.CalcHeight(new GUIContent(documentation.remarks), width);
		}

		private void OnRemarksGUI(Rect remarksPosition)
		{
			GUI.Label(remarksPosition, documentation.remarks, Styles.remarks);
		}

		protected DocumentedFuzzyOption(FuzzyOptionMode mode) : base(mode) { }

		public static class Styles
		{
			static Styles()
			{
				summary = new GUIStyle(EditorStyles.label);
				summary.padding = new RectOffset(7, 7, 7, 7);
				summary.wordWrap = true;
				summary.richText = true;

				parameterSummary = new GUIStyle(EditorStyles.label);
				parameterSummary.padding = new RectOffset(7, 7, 0, 7);
				parameterSummary.wordWrap = true;
				parameterSummary.richText = true;

				remarks = new GUIStyle(EditorStyles.label);
				remarks.padding = new RectOffset(7, 7, 7, 7);
				remarks.wordWrap = true;
				remarks.richText = true;
				remarks.fontSize = 10;
				remarks.normal.textColor = ColorPalette.unityForegroundDim;
				//remarks.fontStyle = FontStyle.Italic;
			}

			public static readonly GUIStyle summary;
			public static readonly GUIStyle parameterSummary;
			public static readonly GUIStyle remarks;
			public static readonly float iconSpacing = 0;
			public static readonly float zoomSize = 32;
			public static readonly float zoomSpacing = 8;
		}
	}
}