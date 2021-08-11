using UnityEditor;

namespace Ludiq.PeekCore
{
	[CustomEditor(typeof(LudiqAsset), true)]
	public class LudiqAssetEditor : LudiqRootObjectEditor
	{
		protected override EditorLayout layout => EditorLayout.Asset;
	}
}