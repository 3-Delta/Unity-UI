using UnityEditor;

namespace Ludiq.PeekCore
{
	[CustomEditor(typeof(LudiqComponent), true)]
	public class LudiqComponentEditor : LudiqRootObjectEditor
	{
		protected override EditorLayout layout => EditorLayout.Component;
	}
}