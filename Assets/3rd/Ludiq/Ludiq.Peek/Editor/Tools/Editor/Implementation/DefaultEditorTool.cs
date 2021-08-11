using Ludiq.Peek;
using UnityObject = UnityEngine.Object;

[assembly: RegisterEditorTool(typeof(UnityObject), typeof(DefaultEditorTool<>))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class DefaultEditorTool<T> : EditorTool<T> where T : UnityObject
	{
		public DefaultEditorTool(T[] targets) : base(targets) { }
	}
}