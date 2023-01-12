using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterEditor(typeof(DictionaryAsset), typeof(DictionaryAssetEditor))]

namespace Ludiq.PeekCore
{
	public sealed class DictionaryAssetEditor : Editor
	{
		public DictionaryAssetEditor(Accessor accessor) : base(accessor) { }

		private Accessor dictionaryAccessor => accessor[nameof(DictionaryAsset.dictionary)];
		
		protected override float GetInnerHeight(float width)
		{
			return ChildInspector(dictionaryAccessor).FieldHeight(width);
		}

		protected override void OnInnerGUI(Rect position)
		{
			ChildInspector(dictionaryAccessor).DrawField(position);
		}
	}
}