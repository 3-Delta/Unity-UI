using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	[Serializable]
	public struct FullSerializationData
	{
		[SerializeField]
		private string _json;

		public string json => _json;

		[SerializeField]
		private UnityObject[] _objectReferences;

		public UnityObject[] objectReferences => _objectReferences;

		public bool IsInitializedByUnity => _json != null && _objectReferences != null;

		public bool ContainsRealData => !string.IsNullOrEmpty(_json) || (_objectReferences != null && _objectReferences.Length > 0);

		public FullSerializationData(string json, IEnumerable<UnityObject> objectReferences)
		{
			_json = json;
			_objectReferences = objectReferences?.ToArray() ?? Empty<UnityObject>.array;
		}

		public FullSerializationData(string json, params UnityObject[] objectReferences) : this(json, ((IEnumerable<UnityObject>)objectReferences)) { }
	}
}