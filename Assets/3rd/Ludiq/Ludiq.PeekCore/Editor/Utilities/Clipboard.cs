using System;
using Ludiq.OdinSerializer;
using UnityEditor;
using UnityEngine;
using OdinSerializationData = Ludiq.OdinSerializer.SerializationData;

namespace Ludiq.PeekCore
{
	public class Clipboard
	{
		public OdinSerializationData? data { get; private set; }
		public Type dataType { get; private set; }
		public bool containsData => data != null;
		private string lastSystemCopy;

		public Clipboard() { }

		public void Clear()
		{
			data = null;
			dataType = null;
		}

		public void Copy(object value, string systemBufferIdentifier = null)
		{
			Ensure.That(nameof(value)).IsNotNull(value);

			data = value.OdinSerialize();
			dataType = value.GetType();
			
			if (systemBufferIdentifier != null)
			{
				var systemCopy = systemBufferIdentifier + Convert.ToBase64String(data.Value.SerializedBytes);
				EditorGUIUtility.systemCopyBuffer = systemCopy;
				lastSystemCopy = systemCopy;
			}
		}

		public void TryFetchSystemBuffer(string systemBufferIdentifier)
		{
			Ensure.That(nameof(systemBufferIdentifier)).IsNotNullOrEmpty(systemBufferIdentifier);

			if (!EditorGUIUtility.systemCopyBuffer.StartsWith(systemBufferIdentifier))
			{
				return;
			}

			if (EditorGUIUtility.systemCopyBuffer == lastSystemCopy)
			{
				return;
			}

			data = new OdinSerializationData { SerializedBytes = Convert.FromBase64String(EditorGUIUtility.systemCopyBuffer.TrimStart(systemBufferIdentifier)) };
			dataType = null;

			try
			{
				var contents = data.Value.OdinDeserialize<object>();
				dataType = contents.GetType();
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Failed to deserialize data in system clipboard: \n" + ex);
				data = null;
				dataType = null;
			}
		}

		public T Paste<T>()
		{
			return (T)Paste(typeof(T));
		}

		public object Paste()
		{
			if (!containsData)
			{
				throw new InvalidOperationException($"Graph clipboard does not contain data.");
			}

			return data.Value.OdinDeserialize<object>();
		}

		public object Paste(Type type)
		{
			return ConversionUtility.Convert(Paste(), type);
		}
	}
}