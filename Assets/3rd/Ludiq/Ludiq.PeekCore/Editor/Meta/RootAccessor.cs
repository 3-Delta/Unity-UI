using System;
using System.Collections.Generic;
using Ludiq.OdinSerializer;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public sealed class RootAccessor : Accessor
	{
		public RootAccessor() : base("Root", null) { }

		public RootAccessor(object value) : this(value, value.GetType()) { }

		public RootAccessor(object value, Type definedType) : base(value, null)
		{
			this.definedType = definedType;
			_rawValue = value;
			serializedObject = value as UnityObject;
			
			if (serializedObject != null && serializedObject.IsConnectedPrefabInstance())
			{
				prefabDefinition = Root(serializedObject.GetPrefabDefinition());
			}
		}

		protected override bool isRoot => true;

		public override UnityObject serializedObject { get; }

		private readonly object _rawValue;

		protected override object rawValue
		{
			get => _rawValue;
			set { }
		}

		public override Attribute[] GetCustomAttributes(bool inherit = true)
		{
			return Empty<Attribute>.array;
		}

		public List<PrefabModification> prefabModifications { get; private set; }

		public override bool supportsPrefabModifications => serializedObject is ISupportsPrefabSerialization;

		public void UpdatePrefabModifications()
		{
			if (!supportsPrefabModifications)
			{
				return;
			}

			var data = ((ISupportsPrefabSerialization)serializedObject).SerializationData;

			prefabModifications = UnitySerializationUtility.PrefabModificationCache.DeserializePrefabModificationsCached(serializedObject, data.PrefabModifications, data.PrefabModificationsReferencedUnityObjects);
		}

		public void ApplyPrefabModifications()
		{
			if (!supportsPrefabModifications)
			{
				return;
			}

			UnitySerializationUtility.RegisterPrefabModificationsChange(serializedObject, prefabModifications);
		}
	}
}