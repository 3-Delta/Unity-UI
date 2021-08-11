using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public interface IFuzzyOption
	{
		object value { get; }
		FuzzyOptionMode mode { get; }

		string label { get; }
		string haystack { get; }
		EditorTexture Icon();
		IEnumerable<EditorTexture> Icons();
		bool dim { get; }

		string headerLabel { get; }
		bool showHeaderIcon { get; }

		bool hasFooter { get; }
		float GetFooterHeight(FuzzyOptionNode node, float width);
		void OnFooterGUI(FuzzyOptionNode node, Rect position);

		string SearchResultLabel(string query);

		void OnPopulate();

		void OnFocusEnter(FuzzyOptionNode node);
		void OnFocusLeave(FuzzyOptionNode node);
	}
}