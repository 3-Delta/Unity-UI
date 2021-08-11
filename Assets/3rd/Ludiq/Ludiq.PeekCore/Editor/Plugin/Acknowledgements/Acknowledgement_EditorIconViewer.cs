using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Acknowledgement_EditorIconViewer), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Acknowledgement_EditorIconViewer : PluginAcknowledgement
	{
		public Acknowledgement_EditorIconViewer(Plugin plugin) : base(plugin) { }

		public override string title => "Editor Icon Viewer";
		public override string author => "Zach Aikman";
	}
}