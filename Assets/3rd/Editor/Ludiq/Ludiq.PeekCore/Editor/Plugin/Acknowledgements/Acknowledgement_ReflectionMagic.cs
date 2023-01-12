using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Acknowledgement_ReflectionMagic), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Acknowledgement_ReflectionMagic : PluginAcknowledgement
	{
		public Acknowledgement_ReflectionMagic(Plugin plugin) : base(plugin) { }

		public override string title => "Reflection Magic";
		public override string author => "Jos van der Til";
		public override int? copyrightYear => 2018;
		public override string url => "https://github.com/ReflectionMagic/ReflectionMagic";
		public override string licenseName => "Apache";
		public override string licenseText => CommonLicenses.Apache;
	}
}