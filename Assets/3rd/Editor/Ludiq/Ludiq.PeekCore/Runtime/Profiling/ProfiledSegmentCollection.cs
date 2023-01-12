using System.Collections.ObjectModel;

namespace Ludiq.PeekCore
{
	public class ProfiledSegmentCollection : KeyedCollection<string, ProfiledSegment>
	{
		protected override string GetKeyForItem(ProfiledSegment item)
		{
			return item.name;
		}
	}
}