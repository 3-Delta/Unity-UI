namespace Ludiq.PeekCore
{
	public interface IFactory
	{
		object Create(IFactoryConfiguration configuration);

		bool requiresConfiguration { get; }

		string label { get; }

		string description { get; }

		EditorTexture icon { get; }
	}
}
