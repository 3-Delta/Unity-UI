using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class FuzzyOption<T> : IFuzzyOption
	{
		protected FuzzyOption(FuzzyOptionMode mode)
		{
			this.mode = mode;
		}

		public T value { get; protected set; }

		object IFuzzyOption.value => value;

		public string label { get; protected set; }

		public virtual string haystack => label;

		protected Func<EditorTexture> getIcon { private get; set; }

		protected EditorTexture icon { private get; set; }

		protected Func<IEnumerable<EditorTexture>> getIcons { get; private set; }

		protected EditorTexture[] icons { get; private set; }

		public virtual EditorTexture Icon()
		{
			return icon ?? getIcon?.Invoke();
		}

		public virtual IEnumerable<EditorTexture> Icons()
		{
			return icons ?? getIcons?.Invoke() ?? Icon()?.Yield() ?? Enumerable.Empty<EditorTexture>();
		}

		public bool dim { get; protected set; }

		public virtual string headerLabel => label;

		public FuzzyOptionMode mode { get; }

		public bool showHeaderIcon { get; protected set; } = true;

		public virtual bool hasFooter { get; protected set; }

		public virtual float GetFooterHeight(FuzzyOptionNode node, float width)
		{
			return 0;
		}

		public virtual void OnFooterGUI(FuzzyOptionNode node, Rect position) { }

		public virtual string SearchResultLabel(string query)
		{
			return SearchUtility.HighlightQuery(haystack, query);
		}

		public virtual void OnPopulate() { }

		public virtual void OnFocusEnter(FuzzyOptionNode node) { }

		public virtual void OnFocusLeave(FuzzyOptionNode node) { }
	}
}