using UnityEngine;

namespace Ludiq.PeekCore
{
	public struct InspectorBlock
	{
		public InspectorBlock(Accessor accessor, Rect position, bool bolded)
		{
			this.accessor = accessor;
			this.position = position;
			this.bolded = false;
		}

		public Accessor accessor { get; }
		public Rect position { get; }
		public bool bolded { get; }
	}
}