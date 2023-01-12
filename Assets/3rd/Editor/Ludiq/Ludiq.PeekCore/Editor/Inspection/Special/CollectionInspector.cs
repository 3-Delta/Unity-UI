using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class CollectionInspector : Inspector
	{
		protected CollectionInspector(Accessor accessor) : base(accessor)
		{
			adaptor = CreateAdaptor();
		}

		protected abstract AccessorCollectionAdaptor CreateAdaptor();

		protected AccessorCollectionAdaptor adaptor { get; }

		protected override bool cacheControlHeight => false;

		protected override bool cacheFieldHeight => false;

		protected override float GetControlWidth()
		{
			return adaptor.GetControlWidth();
		}

		protected override float GetControlHeight(float width)
		{
			return adaptor.GetControlHeight(width);
		}

		protected override void OnControlGUI(Rect position)
		{
			adaptor.DrawControl(position);
		}

		protected override float GetFieldHeight(float width)
		{
			return adaptor.GetFieldHeight(width);
		}

		protected override void OnFieldGUI(Rect position)
		{
			adaptor.DrawField(position);
		}
	}
}