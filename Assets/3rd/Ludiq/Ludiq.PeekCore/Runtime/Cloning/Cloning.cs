using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class Cloning
	{
		static Cloning()
		{
			cloners.Add(arrayCloner);
			cloners.Add(dictionaryCloner);
			cloners.Add(enumerableCloner);
			cloners.Add(listCloner);
			cloners.Add(animationCurveCloner);
			cloners.Add(gradientCloner);
		}

		// Cloning has to be really fast, and skippable takes a while.
		private static readonly Dictionary<Type, bool> skippable = new Dictionary<Type, bool>();

		public static HashSet<ICloner> cloners { get; } = new HashSet<ICloner>();

		public static ArrayCloner arrayCloner { get; } = new ArrayCloner();
		public static DictionaryCloner dictionaryCloner { get; } = new DictionaryCloner();
		public static EnumerableCloner enumerableCloner { get; } = new EnumerableCloner();
		public static ListCloner listCloner { get; } = new ListCloner();
		public static AnimationCurveCloner animationCurveCloner { get; } = new AnimationCurveCloner();
		public static GradientCloner gradientCloner { get; } = new GradientCloner();

		public static FieldsCloner fieldsCloner { get; } = new FieldsCloner();
		public static SerializationPolicyCloner serializationPolicyCloner { get; } = new SerializationPolicyCloner();
		
		private static ICloner GetCloner(object original, Type type, ICloner fallbackCloner)
		{
			if (original is ISpecifiesCloner cloneableVia)
			{
				return cloneableVia.cloner;
			}

			foreach (var cloner in cloners)
			{
				if (cloner.Handles(type))
				{
					return cloner;
				}
			}

			Ensure.That(nameof(fallbackCloner)).IsNotNull(fallbackCloner);

			return fallbackCloner;
		}

		private static bool Skippable(Type type)
		{
			bool result;

			if (!skippable.TryGetValue(type, out result))
			{
				result = type.IsValueType || // Value types are copied on assignment, so no cloning is necessary
				         type == typeof(string) || // Strings have copy on write semantics as well, but aren't value types
				         typeof(Type).IsAssignableFrom(type) || // Types are guaranteed to be singletons. Using inheritance because MonoType/RuntimeType extend Type
				         typeof(UnityObject).IsAssignableFrom(type); // Unity objects act as pure references

				skippable.Add(type, result);
			}

			return result;
		}

		internal static object Clone(CloningContext context, object original)
		{
			object clone = null;
			CloneInto(context, ref clone, original);
			return clone;
		}

		internal static void CloneInto(CloningContext context, ref object clone, object original)
		{
			if (original == null)
			{
				clone = null;
				return;
			}

			var type = original.GetType();

			if (Skippable(type))
			{
				clone = original;
				return;
			}

			if (context.clones.TryGetValue(original, out var existingClone))
			{
				clone = existingClone;
				return;
			}

			var cloner = GetCloner(original, type, context.fallbackCloner);

			if (clone == null)
			{
				clone = cloner.ConstructClone(type, original);
			}

			context.clones.Add(original, clone);
			cloner.BeforeClone(type, original);
			cloner.FillClone(type, ref clone, original, context);
			cloner.AfterClone(type, clone);
			context.clones[original] = clone; // In case the reference changed, for example in arrays
		}


		#region Clone Overloads

		public static object Clone(this object original, ICloner fallbackCloner, bool tryPreserveInstances)
		{
			using (var context = CloningContext.New(fallbackCloner, tryPreserveInstances))
			{
				return Clone(context, original);
			}
		}

		public static object Clone(this object original, ICloner fallbackCloner, bool tryPreserveInstances, out Dictionary<object, object> clones)
		{
			using (var context = CloningContext.New(fallbackCloner, tryPreserveInstances))
			{
				var clone = Clone(context, original);
				clones = new Dictionary<object, object>(context.clones);
				return clone;
			}
		}
		
		public static T Clone<T>(this T original, ICloner fallbackCloner, bool tryPreserveInstances)
		{
			return (T)Clone((object)original, fallbackCloner, tryPreserveInstances);
		}

		public static T Clone<T>(this T original, ICloner fallbackCloner, bool tryPreserveInstances, out Dictionary<object, object> clones)
		{
			return (T)Clone((object)original, fallbackCloner, tryPreserveInstances, out clones);
		}
		
		public static object CloneViaSerializationPolicy(this object original)
		{
			return original.Clone(serializationPolicyCloner, true);
		}

		public static object CloneViaSerializationPolicy(this object original, out Dictionary<object, object> clones)
		{
			return original.Clone(serializationPolicyCloner, true, out clones);
		}

		public static T CloneViaSerializationPolicy<T>(this T original)
		{
			return (T)CloneViaSerializationPolicy((object)original);
		}

		public static T CloneViaSerializationPolicy<T>(this T original, out Dictionary<object, object> clones)
		{
			return (T)CloneViaSerializationPolicy((object)original, out clones);
		}

		#endregion



		#region CloneInto Overloads

		public static void CloneInto(this object original, ref object clone, ICloner fallbackCloner, bool tryPreserveInstances)
		{
			using (var context = CloningContext.New(fallbackCloner, tryPreserveInstances))
			{
				CloneInto(context, ref clone, original);
			}
		}

		public static void CloneInto(this object original, ref object clone, ICloner fallbackCloner, bool tryPreserveInstances, out Dictionary<object, object> clones)
		{
			using (var context = CloningContext.New(fallbackCloner, tryPreserveInstances))
			{
				CloneInto(context, ref clone, original);
				clones = new Dictionary<object, object>(context.clones);
			}
		}
		
		public static void CloneInto<T>(this T original, ref object clone, ICloner fallbackCloner, bool tryPreserveInstances)
		{
			CloneInto((object)original, ref clone, fallbackCloner, tryPreserveInstances);
		}

		public static void CloneInto<T>(this T original, ref object clone, ICloner fallbackCloner, bool tryPreserveInstances, out Dictionary<object, object> clones)
		{
			CloneInto((object)original, ref clone, fallbackCloner, tryPreserveInstances, out clones);
		}
		
		public static void CloneIntoViaSerializationPolicy(this object original, ref object clone)
		{
			original.CloneInto(ref clone, serializationPolicyCloner, true);
		}

		public static void CloneIntoViaSerializationPolicy(this object original, ref object clone, out Dictionary<object, object> clones)
		{
			 original.CloneInto(ref clone, serializationPolicyCloner, true, out clones);
		}

		public static void CloneIntoViaSerializationPolicy<T>(this T original, ref object clone)
		{
			CloneIntoViaSerializationPolicy((object)original, ref clone);
		}

		public static void CloneIntoViaSerializationPolicy<T>(this T original, ref object clone, out Dictionary<object, object> clones)
		{
			CloneIntoViaSerializationPolicy((object)original, ref clone, out clones);
		}

		#endregion

		public static object CloneViaMarshalling(this object value)
		{
			var type = value.GetType();

			if (!type.IsValueType)
			{
				throw new ArgumentException($"{nameof(value)} must be a value type");
			}

			var ptr = IntPtr.Zero;

			try
			{
				ptr = Marshal.AllocHGlobal(Marshal.SizeOf(type));
				Marshal.StructureToPtr(value, ptr, false);
				return Marshal.PtrToStructure(ptr, type);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
		}
	}
}