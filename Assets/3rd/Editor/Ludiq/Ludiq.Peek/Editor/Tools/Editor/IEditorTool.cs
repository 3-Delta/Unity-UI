using System;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public interface IEditorTool : ITool
	{
		Type targetType { get; }
	}
}