using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Acknowledgement_MD4), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Acknowledgement_MD4 : PluginAcknowledgement
	{
		public Acknowledgement_MD4(Plugin plugin) : base(plugin) { }

		public override string title => "MD4 Algorithm";
		public override string author => "BitLush";
		public override string url => "https://bitlush.com/blog/md4-hash-algorithm-in-c-sharp";
	}
}