// #define DEBUG_DEPENDENCIES

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Ludiq.OdinSerializer;
using Ludiq.PeekCore.FullSerializer;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;
using OdinSerializationData = Ludiq.OdinSerializer.SerializationData;
using OdinSerializationContext = Ludiq.OdinSerializer.SerializationContext;
using OdinDeserializationContext = Ludiq.OdinSerializer.DeserializationContext;

namespace Ludiq.PeekCore
{
	public static class Serialization
	{
		static Serialization()
		{
			freeFullOperations = new HashSet<FullSerializationOperation>();
			busyFullOperations = new HashSet<FullSerializationOperation>();

			freeOdinSerializationContexts = new HashSet<OdinSerializationContext>();
			busyOdinSerializationContexts = new HashSet<OdinSerializationContext>();

			freeOdinDeserializationContexts = new HashSet<OdinDeserializationContext>();
			busyOdinDeserializationContexts = new HashSet<OdinDeserializationContext>();
		}

		public const string ConstructorWarning = "This parameterless constructor is only made public for serialization. Use another constructor instead.";

		private static readonly object @lock = new object();

		public static bool isUnityDeserializing { get; private set; }

		public static string PrettyPrint(string json)
		{
			return fsJsonPrinter.PrettyJson(fsJsonParser.Parse(json));
		}



		#region Full Serializer

		private static readonly HashSet<FullSerializationOperation> freeFullOperations;

		private static readonly HashSet<FullSerializationOperation> busyFullOperations;

		private static FullSerializationOperation BeginFullOperation()
		{
			lock (@lock)
			{
				if (freeFullOperations.Count == 0)
				{
					freeFullOperations.Add(new FullSerializationOperation());
				}

				var operation = freeFullOperations.First();
				freeFullOperations.Remove(operation);
				busyFullOperations.Add(operation);
				return operation;
			}
		}

		private static void EndFullOperation(FullSerializationOperation operation)
		{
			lock (@lock)
			{
				if (!busyFullOperations.Contains(operation))
				{
					throw new InvalidOperationException("Trying to finish an operation that isn't started.");
				}

				operation.Reset();
				busyFullOperations.Remove(operation);
				freeFullOperations.Add(operation);
			}
		}

		public static FullSerializationData FullSerialize(this object value, bool forceReflected = false)
		{
			try
			{
				var operation = BeginFullOperation();
				var json = FullSerializeJson(operation.serializer, value, forceReflected);
				var objectReferences = operation.objectReferences.ToArray();
				var data = new FullSerializationData(json, objectReferences);
				EndFullOperation(operation);

#if DEBUG_SERIALIZATION
				Debug.Log(data.ToString($"<color=#88FF00>Serialized: <b>{value?.GetType().Name ?? "null"} [{value?.GetHashCode().ToString() ?? "N/A"}]</b></color>"));
#endif

				return data;
			}
			catch (Exception ex)
			{
				throw new SerializationException($"Serialization of '{value?.GetType().ToString() ?? "null"}' failed.", ex);
			}
		}

		public static void FullDeserializeInto(this FullSerializationData data, ref object instance, bool forceReflected = false)
		{
			try
			{
				if (string.IsNullOrEmpty(data.json))
				{
					instance = null;
					return;
				}

				var operation = BeginFullOperation();
				operation.objectReferences.AddRange(data.objectReferences);
				FullDeserializeJson(operation.serializer, data.json, ref instance, forceReflected);
				EndFullOperation(operation);
			}
			catch (Exception ex)
			{
				try
				{
					Debug.LogWarning(data.GetDebugString("Deserialization Failure Data"), instance as UnityObject);
				}
				catch (Exception ex2)
				{
					Debug.LogWarning("Failed to log deserialization failure data:\n" + ex2, instance as UnityObject);
				}

				throw new SerializationException($"Deserialization into '{instance?.GetType().ToString() ?? "null"}' failed.", ex);
			}
		}

		public static object FullDeserialize(this FullSerializationData data, bool forceReflected = false)
		{
			object instance = null;
			FullDeserializeInto(data, ref instance, forceReflected);
			return instance;
		}

		private static string FullSerializeJson(fsSerializer serializer, object instance, bool forceReflected)
		{
			using (ProfilingUtility.SampleBlock("SerializeJson"))
			{
				fsData data;

				fsResult result;

				if (forceReflected)
				{
					result = serializer.TrySerialize(instance.GetType(), typeof(fsReflectedConverter), instance, out data);
				}
				else
				{
					result = serializer.TrySerialize(instance, out data);
				}

				HandleFullResult("Serialization", result, instance as UnityObject);

				return fsJsonPrinter.CompressedJson(data);
			}
		}

		private static void FullDeserializeJson(fsSerializer serializer, string json, ref object instance, bool forceReflected)
		{
			using (ProfilingUtility.SampleBlock("DeserializeJson"))
			{
				var fsData = fsJsonParser.Parse(json);

				fsResult result;

				if (forceReflected)
				{
					result = serializer.TryDeserialize(fsData, instance.GetType(), typeof(fsReflectedConverter), ref instance);
				}
				else
				{
					result = serializer.TryDeserialize(fsData, ref instance);
				}

				HandleFullResult("Deserialization", result, instance as UnityObject);
			}
		}

		private static void HandleFullResult(string label, fsResult result, UnityObject context = null)
		{
			result.AssertSuccess();

			if (result.HasWarnings)
			{
				foreach (var warning in result.RawMessages)
				{
					Debug.LogWarning($"[{label}] {warning}\n", context);
				}
			}
		}

		public static string GetDebugString(this FullSerializationData data, string title = null)
		{
			using (var writer = new StringWriter())
			{
				writer.WriteLine("Full Serializer Data");

				if (!string.IsNullOrEmpty(title))
				{
					writer.WriteLine(title);
				}

				writer.WriteLine();

				writer.WriteLine("Object References: ");

				WriteObjectReferenceDebug(writer, data.objectReferences);

				writer.WriteLine();

				writer.WriteLine("JSON: ");

				if (data.json == null)
				{
					writer.WriteLine("(Null)");
				}
				else if (data.json == string.Empty)
				{
					writer.WriteLine("(Empty)");
				}
				else
				{
					writer.WriteLine(PrettyPrint(data.json));
				}

				return writer.ToString();
			}
		}

		#endregion



		#region Odin Serializer

		private static readonly HashSet<OdinSerializationContext> freeOdinSerializationContexts;

		private static readonly HashSet<OdinSerializationContext> busyOdinSerializationContexts;

		private static readonly HashSet<OdinDeserializationContext> freeOdinDeserializationContexts;

		private static readonly HashSet<OdinDeserializationContext> busyOdinDeserializationContexts;

		private static OdinSerializationContext BeginOdinSerializationContext()
		{
			lock (@lock)
			{
				if (freeOdinSerializationContexts.Count == 0)
				{
					freeOdinSerializationContexts.Add(new OdinSerializationContext());
				}

				var context = freeOdinSerializationContexts.First();
				ConfigureOdinSerializationContext(context);
				freeOdinSerializationContexts.Remove(context);
				busyOdinSerializationContexts.Add(context);
				return context;
			}
		}

		private static void EndOdinSerializationContext(OdinSerializationContext context)
		{
			lock (@lock)
			{
				if (!busyOdinSerializationContexts.Contains(context))
				{
					throw new InvalidOperationException("Trying to finish an operation that isn't started.");
				}

				context.ResetToDefault();
				busyOdinSerializationContexts.Remove(context);
				freeOdinSerializationContexts.Add(context);
			}
		}

		private static OdinDeserializationContext BeginOdinDeserializationContext()
		{
			lock (@lock)
			{
				if (freeOdinDeserializationContexts.Count == 0)
				{
					freeOdinDeserializationContexts.Add(new OdinDeserializationContext());
				}

				var context = freeOdinDeserializationContexts.First();
				ConfigureOdinDeserializationContext(context);
				freeOdinDeserializationContexts.Remove(context);
				busyOdinDeserializationContexts.Add(context);
				return context;
			}
		}

		private static void EndOdinDeserializationContext(OdinDeserializationContext context)
		{
			lock (@lock)
			{
				if (!busyOdinDeserializationContexts.Contains(context))
				{
					throw new InvalidOperationException("Trying to finish an operation that isn't started.");
				}

				context.Reset();
				busyOdinDeserializationContexts.Remove(context);
				freeOdinDeserializationContexts.Add(context);
			}
		}

		private static void ConfigureOdinSerializationContext(OdinSerializationContext context)
		{
			ConfigureOdinConfig(context.Config);
			context.Binder = SerializationTypeBinder.instance;
		}

		private static void ConfigureOdinDeserializationContext(OdinDeserializationContext context)
		{
			ConfigureOdinConfig(context.Config);
			context.Binder = SerializationTypeBinder.instance;
		}

		private static void ConfigureOdinConfig(SerializationConfig config)
		{
			config.SerializationPolicy = SerializationPolicy.instance;
			config.DebugContext.Logger = SerializationLogger.instance;
			config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.ThrowOnErrors;
		}

		public static OdinSerializationData OdinSerialize<T>(this T value, DataFormat format = DataFormat.Binary)
		{
			var data = new OdinSerializationData();

			return OdinSerialize(value, ref data, format);
		}

		public static OdinSerializationData OdinSerialize<T>(this T value, ref OdinSerializationData data, DataFormat format = DataFormat.Binary)
		{
			try
			{
				var context = BeginOdinSerializationContext();

				if (value is UnityObject uo)
				{
					UnitySerializationUtility.SerializeUnityObject(uo, ref data, true, context);
				}
				else
				{
					data.SerializedFormat = format;

					if (format == DataFormat.Binary)
					{
						var bytes = SerializationUtility.SerializeValue(value, format, out var unityObjects, context);
						data.SerializedBytes = bytes;
						data.ReferencedUnityObjects = unityObjects;
					}
					else if (format == DataFormat.JSON)
					{
						var bytes = SerializationUtility.SerializeValue(value, format, out var unityObjects, context);
						data.SerializedBytesString = Convert.ToBase64String(bytes);
						data.ReferencedUnityObjects = unityObjects;
					}
					else if (format == DataFormat.Nodes)
					{
						using (var writer = new SerializationNodeDataWriter(context))
						{
							SerializationUtility.SerializeValue(writer, format, out var unityObjects, context);
							data.SerializationNodes = writer.Nodes;
							data.ReferencedUnityObjects = unityObjects;
						}
					}
					else
					{
						throw new UnexpectedEnumValueException<DataFormat>(format);
					}
				}

				EndOdinSerializationContext(context);

				return data;
			}
			catch (Exception ex)
			{
				throw new SerializationException($"Serialization of '{value?.GetType().ToString() ?? "null"}' failed.", ex);
			}
		}

		public static void OdinDeserializeInto<T>(this OdinSerializationData data, ref T instance)
		{
			try
			{
				var context = BeginOdinDeserializationContext();

				if (instance is UnityObject uo)
				{
					UnitySerializationUtility.DeserializeUnityObject(uo, ref data, context);
				}
				else
				{
					instance = SerializationUtility.DeserializeValue<T>(data.SerializedBytes, data.SerializedFormat, data.ReferencedUnityObjects, context);
				}

				EndOdinDeserializationContext(context);
			}
			catch (ThreadAbortException) { }
			catch (Exception ex)
			{
				throw new SerializationException($"Deserialization into '{instance?.GetType().ToString() ?? "null"}' failed.", ex);
			}
		}

		public static T OdinDeserialize<T>(this OdinSerializationData data)
		{
			var instance = default(T);
			OdinDeserializeInto(data, ref instance);
			return instance;
		}

		public static OdinSerializationData ToOdinData(this byte[] bytes)
		{
			return new OdinSerializationData {SerializedBytes = bytes};
		}

		private static void WriteObjectReferenceDebug(StringWriter writer, IEnumerable<UnityObject> uos)
		{
			if (uos == null)
			{
				writer.WriteLine("(Null)");
			}
			else if (!uos.Any())
			{
				writer.WriteLine("(None)");
			}
			else
			{
				var index = 0;

				foreach (var objectReference in uos)
				{
					if (objectReference == null)
					{
						writer.WriteLine($"{index}: null");
					}
					else if (UnityThread.allowsAPI)
					{
						writer.WriteLine($"{index}: {objectReference.GetType().FullName} [{objectReference.GetHashCode()}] \"{objectReference.name}\"");
					}
					else
					{
						writer.WriteLine($"{index}: {objectReference.GetType().FullName} [{objectReference.GetHashCode()}]");
					}

					index++;
				}
			}
		}

		public static string GetDebugString(this OdinSerializationData data, string title = null)
		{
			using (var writer = new StringWriter())
			{
				writer.WriteLine("Odin Serializer Data");

				if (!string.IsNullOrEmpty(title))
				{
					writer.WriteLine(title);
				}

				writer.WriteLine();

				writer.WriteLine("Object References: ");

				WriteObjectReferenceDebug(writer, data.ReferencedUnityObjects);

				writer.WriteLine();

				if (data.SerializedFormat == DataFormat.Nodes)
				{
					writer.WriteLine("Nodes: ");

					if (data.SerializationNodes == null)
					{
						writer.WriteLine("(Null)");
					}
					else if (data.SerializationNodes.Count == 0)
					{
						writer.WriteLine("(Empty)");
					}
					else
					{
						var indent = 0;

						foreach (var node in data.SerializationNodes)
						{
							switch (node.Entry)
							{
								case EntryType.EndOfNode:
								case EntryType.EndOfArray:
									indent--;
									break;
							}

							writer.Write(new string('\t', Mathf.Max(indent, 0)));

							writer.Write(node.Entry);

							if (!string.IsNullOrEmpty(node.Name))
							{
								writer.Write(": " + node.Name);
							}

							if (!string.IsNullOrEmpty(node.Data))
							{
								writer.Write(" = " + node.Data);
							}

							writer.WriteLine();

							switch (node.Entry)
							{
								case EntryType.StartOfNode:
								case EntryType.StartOfArray:
									indent++;
									break;
							}
						}
					}
				}
				else if (data.SerializedFormat == DataFormat.JSON)
				{
					writer.WriteLine("JSON: ");

					if (data.SerializedBytesString == null)
					{
						writer.WriteLine("(Null)");
					}
					else if (data.SerializedBytesString == string.Empty)
					{
						writer.WriteLine("(Empty)");
					}
					else
					{
						writer.WriteLine(PrettyPrint(Encoding.UTF8.GetString(Convert.FromBase64String(data.SerializedBytesString))));
					}
				}
				else if (data.SerializedFormat == DataFormat.Binary)
				{
					writer.WriteLine("Bytes: ");

					if (data.SerializedBytes == null)
					{
						writer.WriteLine("(Null)");
					}
					else if (data.SerializedBytes.Length == 0)
					{
						writer.WriteLine("(Empty)");
					}
					else
					{
						StringUtility.WordWrap(BitConverter.ToString(data.SerializedBytes).Replace('-', ' '), 16 * 3);
					}
				}

				writer.WriteLine();

				writer.WriteLine("Prefab Modifications Object References: ");

				WriteObjectReferenceDebug(writer, data.PrefabModificationsReferencedUnityObjects);

				writer.WriteLine();

				writer.WriteLine("Prefab Modifications: ");

				if (data.PrefabModifications == null)
				{
					writer.WriteLine("(Null)");
				}
				else if (data.PrefabModifications.Count == 0)
				{
					writer.WriteLine("(Empty)");
				}
				else
				{
					foreach (var node in data.PrefabModifications)
					{
						writer.WriteLine(node);
					}
				}

				return writer.ToString();
			}
		}

		public static bool ContainsRealData(this OdinSerializationData data)
		{
			return data.SerializedBytes != null && data.SerializedBytes.Length > 0 ||
			       data.SerializationNodes != null && data.SerializationNodes.Count > 0 ||
			       !string.IsNullOrEmpty(data.SerializedBytesString);
		}

		#endregion



		#region Mixed

		public static void OnBeforeSerializeImplementation<T>(T uo, ref FullSerializationData fullData, ref OdinSerializationData odinData, bool deserializationFailed) where T : UnityObject, ILudiqRootObject
		{
			// To prevent data loss, we don't serialize something that failed to deserialize in the first place
			if (deserializationFailed)
			{
				Debug.LogWarning($"Skipping serialization of {uo.ToSafeString()} to prevent data loss because it failed to deserialize.\n", uo);
				return;
			}

			// Run the user handler
			try
			{
				uo.OnBeforeSerialize();
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to run OnBeforeSerialize on {uo.ToSafeString()}.\n{ex}", uo);
				return;
			}

			// Check if we are migrating legacy FS data into OS to notify the user
			var migrating = fullData.ContainsRealData && !odinData.ContainsRealData();

			try
			{
				// Serialize the data using OS only from now on
				odinData = uo.OdinSerialize(ref odinData);

				if (migrating)
				{
					// Clear the legacy FS data so that it isn't used as a fallback during deserialization from now on.
					fullData = default;

					Debug.Log($"Migrated legacy serialization data on {uo.ToSafeString()} from Full Serializer to Odin Serializer.\n", uo);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to serialize {uo.ToSafeString()} using Odin Serializer.\n{ex}", uo);
				return;
			}

			// Run the user handler
			try
			{
				uo.OnAfterSerialize();
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to run OnAfterSerialize on {uo.ToSafeString()}.\n{ex}", uo);
				return;
			}
		}

		public static void OnAfterDeserializeImplementation<T>(T uo, FullSerializationData fullData, OdinSerializationData odinData, ref bool deserializationFailed) where T : UnityObject, ILudiqRootObject
		{
			// Set a flag to indicate to our API checker that Unity calls are forbidden
			isUnityDeserializing = true;

			try
			{
				object _this = uo;

				// If we don't reach the complete end of the process for any reason, the failure flag will be set.
				deserializationFailed = true;

				// Run the user callback
				try
				{
					uo.OnBeforeDeserialize();
				}
				catch (Exception ex)
				{
					Debug.LogError($"Failed to run OnBeforeDeserialize on {uo.ToSafeString()}.\n{ex}", uo);
					return;
				}

				// Deserialize with OS data if it is available
				if (odinData.ContainsRealData())
				{
					try
					{
						odinData.OdinDeserializeInto(ref _this);
					}
					catch (Exception ex)
					{
						Debug.LogError($"Failed to deserialize {uo.ToSafeString()} using Odin Serializer.\n{ex}", uo);
						return;
					}
				}
				// In theory, there shouldn't be a case where we have both OS and FS data because we clear the FS data on a successful OS deserialization.
				// Just to be absolutely safe we don't rollback to earlier FS data though, we're being mutually exclusive on deserialization too.
				else
				{
					// Fallback to legacy FS data
					try
					{
						fullData.FullDeserializeInto(ref _this, true);
					}
					catch (Exception ex)
					{
						Debug.LogError($"Failed to deserialize {uo.ToSafeString()} using legacy Full Serializer data.\n{ex}", uo);
						return;
					}
				}

				// Run the user callback
				try
				{
					uo.OnAfterDeserialize();
				}
				catch (Exception ex)
				{
					Debug.LogError($"Failed to run OnAfterDeserialize on {uo.ToSafeString()}.\n{ex}", uo);
					return;
				}

				// We managed to execute everything successfully, clear the failure flag.
				deserializationFailed = false;
			}
			finally
			{
				// Clear our API lock regardless of what happened
				isUnityDeserializing = false;
			}
		}

		public static void ShowData(string title, FullSerializationData fullData, OdinSerializationData odinData)
		{
			var guid = Guid.NewGuid();
			var fileName = string.IsNullOrEmpty(title) ? guid.ToString() : title + "." + guid;

			var fullDataPath = Path.GetTempPath() + fileName + ".full.txt";
			var odinDataPath = Path.GetTempPath() + fileName + ".odin.txt";

			File.WriteAllText(fullDataPath, GetDebugString(fullData));
			File.WriteAllText(odinDataPath, GetDebugString(odinData));

			Process.Start(fullDataPath);
			Process.Start(odinDataPath);
		}

		#endregion



		#region Dependencies

		public static readonly HashSet<ISerializationDepender> awaitingDependers = new HashSet<ISerializationDepender>();

		public static readonly HashSet<ISerializationDependency> availableDependencies = new HashSet<ISerializationDependency>();

		public static void AwaitDependencies(ISerializationDepender depender)
		{
#if DEBUG_DEPENDENCIES
			Debug.Log($"Awaiting {depender.deserializationDependencies.Count()} dependencies on {depender}\n".Colored(new Color(1, 0.5f, 0))+$"{depender.deserializationDependencies.ToLineSeparatedString()}\n");
#endif

			awaitingDependers.Add(depender);

			CheckIfDependenciesMet(depender);
		}

		public static void NotifyDependencyDeserialized(ISerializationDependency dependency)
		{
			availableDependencies.Add(dependency);
			
#if DEBUG_DEPENDENCIES
			Debug.Log($"Dependency {dependency} is now available.\n".Colored(Color.yellow));
#endif

			foreach (var awaitingDepender in awaitingDependers.ToArray())
			{
				if (!awaitingDependers.Contains(awaitingDepender))
				{
					// In case the depender was already handled by a recursive 
					// dependency via OnAfterDependenciesDeserialized,
					// we skip it. This is necessary because we duplicated
					// the set to safely iterate over it with removal.
					// 
					// This should prevent OnAfterDependenciesDeserialized from
					// running twice on any given depender in a single deserialization
					// operation.
					continue;
				}

				CheckIfDependenciesMet(awaitingDepender);
			}
		}

		public static void NotifyDependencyDeserializing(ISerializationDependency dependency)
		{
			if (availableDependencies.Remove(dependency))
			{
#if DEBUG_DEPENDENCIES
				Debug.Log($"Dependency {dependency} is no longer available.\n".Colored(Color.red));
#endif
			}
		}

		private static void CheckIfDependenciesMet(ISerializationDepender depender)
		{
			if (availableDependencies.IsSupersetOf(depender.deserializationDependencies))
			{
				awaitingDependers.Remove(depender);

#if DEBUG_DEPENDENCIES
				Debug.Log($"All dependencies met on {depender}.\n".Colored(Color.cyan));
#endif

				depender.OnAfterDependenciesDeserialized();
			}
		}

		#endregion
	}
}