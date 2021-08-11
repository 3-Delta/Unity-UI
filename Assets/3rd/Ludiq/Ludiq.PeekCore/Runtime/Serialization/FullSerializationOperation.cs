using System.Collections.Generic;
using Ludiq.PeekCore.FullSerializer;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public class FullSerializationOperation
	{
		public FullSerializationOperation()
		{
			objectReferences = new List<UnityObject>();
			serializer = new fsSerializer();
			serializer.AddConverter(new UnityObjectConverter());
			serializer.AddConverter(new RayConverter());
			serializer.AddConverter(new Ray2DConverter());
			serializer.AddConverter(new NamespaceConverter());
			serializer.AddConverter(new LooseAssemblyNameConverter());
			serializer.Context.Set(objectReferences);
		}
		
		public fsSerializer serializer { get; private set; }
		public List<UnityObject> objectReferences { get; private set; }

		public void Reset()
		{
			objectReferences.Clear();
		}
	}
}