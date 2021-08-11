namespace Ludiq.PeekCore
{
	public interface IPluginModule : IPluginAddon
	{
		void Initialize();
		void LateInitialize();
	}
}