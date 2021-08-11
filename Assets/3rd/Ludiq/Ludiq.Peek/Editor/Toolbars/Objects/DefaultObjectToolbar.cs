using Ludiq.Peek;
using UnityObject = UnityEngine.Object;

[assembly: RegisterObjectToolbar(typeof(UnityObject), typeof(DefaultObjectToolbar))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class DefaultObjectToolbar : ObjectToolbar<UnityObject>
	{
		public DefaultObjectToolbar(UnityObject[] targets) : base(targets) { }
	}
}