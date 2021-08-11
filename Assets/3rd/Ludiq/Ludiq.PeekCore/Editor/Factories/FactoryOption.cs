using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public class FactoryOption : DocumentedFuzzyOption<LudiqGUI.PopupFunc>
	{
		protected readonly IFactory factory;

		protected readonly IFactoryConfiguration configuration;

		protected readonly bool showNewIcon;

		public FactoryOption(IFactory factory, IFactoryConfiguration configuration = null, bool showNewIcon = true) : base(FuzzyOptionMode.Leaf)
		{
			Ensure.That(nameof(factory)).IsNotNull(factory);

			value = Func;
			this.factory = factory;
			this.configuration = configuration;
			label = $"New {factory.label}";
			this.showNewIcon = showNewIcon;
			documentation = new LambdaFuzzyOptionDocumentation(factory.description);
			zoom = true;
		}

		public override EditorTexture Icon()
		{
			return factory.icon;
		}

		public override IEnumerable<EditorTexture> Icons()
		{
			if (showNewIcon)
			{
				yield return LudiqCore.Icons.@new;
			}
			
			var factoryIcon = Icon();

			if (factoryIcon != null)
			{
				yield return factoryIcon;
			}
		}

		protected virtual bool Func(out object o)
		{
			o = factory.Create(configuration);
			return true;
		}
	}
}
