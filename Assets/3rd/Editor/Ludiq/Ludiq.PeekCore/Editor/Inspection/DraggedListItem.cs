using System.Collections;

namespace Ludiq.PeekCore
{
	public class DraggedListItem
	{
		public DraggedListItem(AccessorListAdaptor sourceListAdaptor, int index, object item)
		{
			this.sourceListAdaptor = sourceListAdaptor;
			this.index = index;
			this.item = item;
		}

		public readonly AccessorListAdaptor sourceListAdaptor;
		public readonly int index;
		public readonly object item;

		public IList sourceList => (IList)sourceListAdaptor.accessor.value;

		public static readonly string TypeName = typeof(DraggedListItem).FullName;

		public override string ToString()
		{
			return $"{item} ({sourceList}[{index}])";
		}
	}
}