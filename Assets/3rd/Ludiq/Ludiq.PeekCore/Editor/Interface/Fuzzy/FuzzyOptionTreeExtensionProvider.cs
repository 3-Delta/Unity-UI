using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public class FuzzyOptionTreeExtensionProvider : MultiDecoratorProvider<IFuzzyOptionTree, IFuzzyOptionTree, RegisterFuzzyOptionTreeExtensionAttribute>
	{
		static FuzzyOptionTreeExtensionProvider()
		{
			instance = new FuzzyOptionTreeExtensionProvider();
		}

		public static FuzzyOptionTreeExtensionProvider instance { get; }
	}

	public static class XFuzzyOptionTreeExtensionProvider
	{
		public static IEnumerable<IFuzzyOptionTree> Extensions(this IFuzzyOptionTree optionTree)
		{
			return FuzzyOptionTreeExtensionProvider.instance.GetDecorators(optionTree);
		}
	}
}