namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class MenuFolderOption : FuzzyOption<string>
	{
		public string path { get; }

		public MenuFolderOption(string path, FuzzyOptionMode mode = FuzzyOptionMode.Branch) : base(mode)
		{
			this.path = path;
			value = path;
			label = path.PartAfterLast('/');
			getIcon = () => LudiqCore.Icons.folder;
		}

		public override string SearchResultLabel(string query)
		{
			var label = base.SearchResultLabel(query);
			
			label += LudiqGUIUtility.DimString($" (in {path.PartBeforeLast('/')})");

			return label;
		}
	}
}