using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class AssetFactoryUtility
	{
		public static bool TryCreateAndSave(IFactory factory, IFactoryConfiguration configuration, out ScriptableObject asset)
		{
			Ensure.That(nameof(factory)).IsNotNull(factory);

			var title = $"New {factory.label}";
			var fileName = title;

			var path = EditorUtility.SaveFilePanelInProject(title, fileName, "asset", null);

			if (!string.IsNullOrEmpty(path))
			{
				asset = factory.Create(configuration).CastTo<ScriptableObject>();
				AssetDatabase.CreateAsset(asset, path);
				EditorGUIUtility.PingObject(asset);
				return true;
			}
			else
			{
				asset = null;
				return false;
			}
		}
	}
}
