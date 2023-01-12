using System;
using System.Collections.Generic;
using System.Reflection;
using Ludiq.OdinSerializer;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class SerializationPolicyCloner : ReflectedCloner
	{
		public override void BeforeClone(Type type, object original)
		{
			(original as ISerializationCallbackReceiver)?.OnBeforeSerialize();
		}

		public override void AfterClone(Type type, object clone)
		{
			(clone as ISerializationCallbackReceiver)?.OnAfterDeserialize();
		}

		protected override IEnumerable<MemberInfo> GetMembers(Type type)
		{
			return FormatterUtilities.GetSerializableMembers(type, SerializationPolicy.instance);
		}
	}
}