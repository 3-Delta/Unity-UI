using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Acknowledgement_OdinSerializer), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Acknowledgement_OdinSerializer : PluginAcknowledgement
	{
		public Acknowledgement_OdinSerializer(Plugin plugin) : base(plugin) { }

		public override string title => "Odin Serializer";
		public override string author => "Sirenix";
		public override int? copyrightYear => 2018;
		public override string url => "https://github.com/TeamSirenix/odin-serializer";
		public override string licenseName => "Apache";
		public override string licenseText => CommonLicenses.Apache;
	}
}