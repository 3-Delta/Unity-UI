using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class HierarchyPropertyCache
	{
		public HierarchyPropertyCache(HierarchyProperty source)
		{
			instanceID = source.instanceID;
			name = source.name;
			hasChildren = source.hasChildren;
			depth = source.depth;
			row = source.row;
			colorCode = source.colorCode;
			guid = source.guid;
			icon = source.icon;
			isValid = source.isValid;
			isMainRepresentation = source.isMainRepresentation;
			hasFullPreviewImage = source.hasFullPreviewImage;
			iconDrawStyle = source.iconDrawStyle;
			isFolder = source.isFolder;
		}

		public int instanceID { get; }
		public string name { get; }
		public bool hasChildren { get; }
		public int depth { get; }
		public int row { get; }
		public int colorCode { get; }
		public string guid { get; }
		public Texture2D icon { get; }
		public bool isValid { get; }
		public bool isMainRepresentation { get; }
		public bool hasFullPreviewImage { get; }
		public IconDrawStyle iconDrawStyle { get; }
		public bool isFolder { get; }

		private string _assetPath;

		public string assetPath => _assetPath ?? (_assetPath = AssetDatabase.GUIDToAssetPath(guid));

		public static HierarchyPropertyCache Find(HierarchyType type, int instanceID)
		{
			var property = new HierarchyProperty(type);
			property.Find(instanceID, null);
			return new HierarchyPropertyCache(property);
		}

		public static HierarchyPropertyCache Find(string assetPath)
		{
			var property = new HierarchyProperty(assetPath);
			return new HierarchyPropertyCache(property);
		}
	}
}
