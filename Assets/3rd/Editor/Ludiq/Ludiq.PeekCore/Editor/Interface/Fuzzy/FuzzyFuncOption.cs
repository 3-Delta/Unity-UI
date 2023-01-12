using System;

namespace Ludiq.PeekCore
{
	public delegate bool FuzzyFunc<T>(out T t);

	public sealed class FuzzyFuncOption<T> : DocumentedFuzzyOption<LudiqGUI.PopupFunc>
	{
		private readonly FuzzyFunc<T> func;

		public FuzzyFuncOption(FuzzyFunc<T> func, string label, Func<EditorTexture> getIcon = null, string summary = null) : base(FuzzyOptionMode.Leaf)
		{
			Ensure.That(nameof(func)).IsNotNull(func);
			this.func = func;
			value = Convert;
			this.label = label;
			this.getIcon = getIcon;
			documentation = new LambdaFuzzyOptionDocumentation(summary);
		}

		private bool Convert(out object o)
		{
			var result = func(out var t);
			o = t;
			return result;
		}
	}
}
