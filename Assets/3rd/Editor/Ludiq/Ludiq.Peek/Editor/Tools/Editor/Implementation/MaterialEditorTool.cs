using Ludiq.Peek;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: RegisterEditorTool(typeof(Material), typeof(MaterialEditorTool))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class MaterialEditorTool : EditorTool<Material>
	{
		public MaterialEditorTool(Material[] targets) : base(targets) { }
	}
}