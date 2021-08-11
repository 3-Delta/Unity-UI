using System;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class Descriptor<TTarget, TDescription> : Assigner<TTarget, TDescription>, IDescriptorWithTitleInformation, IDisposable
		where TTarget : class
		where TDescription : class, IDescription, new()
	{
		protected Descriptor(TTarget target) : base(target, new TDescription())
		{
			if (target is INotifyChanged notifier)
			{
				notifier.changedInternally += SetDirty;
				notifier.changedExternally += SetDirty;
			}
		}

		public void Dispose()
		{
			if (target is INotifyChanged notifier)
			{
				notifier.changedInternally -= SetDirty;
				notifier.changedExternally -= SetDirty;
			}
		}

		public event Action descriptionChanged;
		
		protected override void OnAssignmentChanged()
		{
			descriptionChanged?.Invoke();
		}

		[Assigns]
		public virtual string Title()
		{
			return target.ToString();
		}

		[Assigns]
		public virtual string Summary()
		{
			return target.GetType().Summary();
		}

		[Assigns]
		[RequiresUnityAPI]
		public virtual EditorTexture Icon()
		{
			return target.GetType().Icon();
		}

		object IDescriptor.target => target;

		public TDescription description => assignee;

		IDescription IDescriptor.description => description;
	}
}
