namespace Ludiq.PeekCore
{
	public class DictionaryInspector : CollectionInspector
	{
		public DictionaryInspector(Accessor accessor) : base(accessor) { }

		protected override AccessorCollectionAdaptor CreateAdaptor()
		{
			return new AccessorDictionaryAdaptor(accessor, this);
		}
	}
}