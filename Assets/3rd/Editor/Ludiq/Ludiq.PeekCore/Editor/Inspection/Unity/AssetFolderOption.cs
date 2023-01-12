using System.IO;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public class AssetFolderOption : FuzzyOption<string>
	{
		public string name { get; }

		public string path { get; }

		public string folder { get; }

		public AssetFolderOption(string pathFromProject, FuzzyOptionMode mode = FuzzyOptionMode.Branch) : base(mode)
		{
			this.path = pathFromProject;
			this.name = Path.GetFileNameWithoutExtension(path);
			this.folder = Path.GetDirectoryName(path);
			this.value = pathFromProject;
			this.label = Path.GetFileName(pathFromProject);
			getIcon = () => LudiqCore.Icons.folder;
		}

		public override string SearchResultLabel(string query)
		{
			var label = base.SearchResultLabel(query);
			
			label += LudiqGUIUtility.DimString($" (in {PathUtility.NaiveNormalize(folder)})");

			return label;
		}
	}
}