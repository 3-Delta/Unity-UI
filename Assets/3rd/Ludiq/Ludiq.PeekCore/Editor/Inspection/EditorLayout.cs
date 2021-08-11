namespace Ludiq.PeekCore
{
	public enum EditorLayout
	{
		// Title, icon and summary are shown as normal fields
		Fields,

		// Title, icon and summary are shown as a header above fields
		Header,

		// Overlays the default Unity asset header
		Asset,

		// Overlays the default Unity component header
		Component,

		// Title and icon and shown inside a foldout. Summary is shown as a normal field.
		Foldout,
	}
}