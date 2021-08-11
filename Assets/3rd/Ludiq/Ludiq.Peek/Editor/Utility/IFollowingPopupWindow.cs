using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public interface IFollowingPopupWindow
	{
		Rect activatorPosition { set; }
	}
}