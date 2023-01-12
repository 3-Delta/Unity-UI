using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Acknowledgement_TinyJson), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Acknowledgement_TinyJson : PluginAcknowledgement
	{
		public Acknowledgement_TinyJson(Plugin plugin) : base(plugin) { }

		public override string title => "Tiny JSON";
		public override string author => "Alex Parker";
		public override int? copyrightYear => 2018;
		public override string url => "https://github.com/zanders3/json";
		public override string licenseName => "MIT";
		public override string licenseText => CommonLicenses.MIT;
	}
}