using System;

namespace Ludiq.PeekCore
{
	public class UnityEditorInternalException : Exception
	{
		public UnityEditorInternalException(Exception innerException) :
			base("An error occured while accessing internal Unity Editor functions. This might happen if Unity makes backward-incompatible changes in their newer versions of the editor.", innerException) { }
	}
}