using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public interface ISerializationDepender : ISerializationCallbackReceiver
	{
		IEnumerable<ISerializationDependency> deserializationDependencies { get; }

		void OnAfterDependenciesDeserialized();
	}
}