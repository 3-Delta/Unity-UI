using System;

namespace Ludiq.PeekCore
{
	public interface IDescriptor
	{
		object target { get; }

		IDescription description { get; }

		void SetDirty();

		void Validate();

		event Action descriptionChanged;
	}
}