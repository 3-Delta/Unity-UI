using System;
using System.Collections.Generic;
using UnityEngine;

// 参考URP的SerializedDictionary的设计
// 以及 https://assetstore.unity.com/packages/tools/integration/serializabledictionary-90477
// 放弃
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [Serializable]
    public class KVP {
        public TKey key;
        public TValue value;
    }

    [SerializeField] private List<KVP> list = new List<KVP>();

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize() { }
}
