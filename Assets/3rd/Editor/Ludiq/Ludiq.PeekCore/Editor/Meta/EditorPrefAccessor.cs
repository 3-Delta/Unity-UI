using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class EditorPrefAccessor : PluginConfigurationItemAccessor
	{
		public EditorPrefAccessor(PluginConfiguration configuration, MemberInfo member, Accessor parent) : base(configuration, member, parent) { }

		public string namespacedKey => $"{configuration.plugin.id}.{key}.Odin";

		public override bool exists => EditorPrefs.HasKey(namespacedKey);

		public override void Load()
		{
			try
			{
				if (definedType == typeof(string))
				{
					value = EditorPrefs.GetString(namespacedKey, (string)defaultValue);
				}
				else if (definedType == typeof(int))
				{
					value = EditorPrefs.GetInt(namespacedKey, (int)defaultValue);
				}
				else if (definedType == typeof(float))
				{
					value = EditorPrefs.GetFloat(namespacedKey, (float)defaultValue);
				}
				else if (definedType == typeof(bool))
				{
					value = EditorPrefs.GetBool(namespacedKey, (bool)defaultValue);
				}
				else
				{
					var bytes = Convert.FromBase64String(EditorPrefs.GetString(namespacedKey));
					value = bytes.ToOdinData().OdinDeserialize<object>();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to deserialize editor pref '{configuration.plugin.id}.{key}', reverting to default.\n{ex}");
				value = defaultValue;
				Save();
			}

			if (!definedType.IsAssignableFrom(valueType))
			{
				Debug.LogWarning($"Failed to cast editor pref '{configuration.plugin.id}.{key}' as '{definedType.CSharpName()}', reverting to default.");
				value = defaultValue;
				Save();
			}
		}

		public override void Save()
		{
			if (definedType == typeof(string))
			{
				EditorPrefs.SetString(namespacedKey, (string)value);
			}
			else if (definedType == typeof(int))
			{
				EditorPrefs.SetInt(namespacedKey, (int)value);
			}
			else if (definedType == typeof(float))
			{
				EditorPrefs.SetFloat(namespacedKey, (float)value);
			}
			else if (definedType == typeof(bool))
			{
				EditorPrefs.SetBool(namespacedKey, (bool)value);
			}
			else
			{
				var bytes = value.OdinSerialize().SerializedBytes;
				EditorPrefs.SetString(namespacedKey, Convert.ToBase64String(bytes));
			}
		}
	}
}