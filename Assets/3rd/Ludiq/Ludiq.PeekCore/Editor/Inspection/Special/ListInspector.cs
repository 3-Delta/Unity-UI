namespace Ludiq.PeekCore
{
	public class ListInspector : CollectionInspector
	{
		public ListInspector(Accessor accessor) : base(accessor) { }

		protected override AccessorCollectionAdaptor CreateAdaptor()
		{
			return new AccessorListAdaptor(accessor, this);
		}
	}
}