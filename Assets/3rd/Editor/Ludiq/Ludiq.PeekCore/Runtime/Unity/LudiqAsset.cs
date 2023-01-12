using UnityEngine;
using UnityEngine.Serialization;
using OdinSerializationData = Ludiq.OdinSerializer.SerializationData;

namespace Ludiq.PeekCore
{
	public abstract class LudiqAsset : ScriptableObject, ISerializationCallbackReceiver, ILudiqRootObject
	{
		[FormerlySerializedAs("_data")]
		[SerializeField, DoNotSerialize] // Serialize with Unity, but not with Full Serializer.
		protected FullSerializationData _fullData;
		
		[SerializeField, DoNotSerialize] // Serialize with Unity, but not with Odin Serializer.
		protected OdinSerializationData _odinData;

		[DoNotSerialize]
		protected bool _deserializationFailed;

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			Serialization.OnBeforeSerializeImplementation(this, ref _fullData, ref _odinData, _deserializationFailed);
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			Serialization.OnAfterDeserializeImplementation(this, _fullData, _odinData, ref _deserializationFailed);
		}

		void ILudiqRootObject.OnBeforeSerialize()
		{
			OnBeforeSerialize();
		}
		
		void ILudiqRootObject.OnAfterSerialize()
		{
			OnAfterSerialize();
		}

		void ILudiqRootObject.OnBeforeDeserialize()
		{
			OnBeforeDeserialize();
		}

		void ILudiqRootObject.OnAfterDeserialize()
		{
			OnAfterDeserialize();
		}

		protected virtual void OnBeforeSerialize() { }

		protected virtual void OnAfterSerialize() { }

		protected virtual void OnBeforeDeserialize() { }

		protected virtual void OnAfterDeserialize() { }
		
		public virtual void ShowData()
		{
			Serialization.ShowData(this.ToSafeString(), _fullData, _odinData);
		}

		public override string ToString()
		{
			return this.ToSafeString();
		}
	}
}