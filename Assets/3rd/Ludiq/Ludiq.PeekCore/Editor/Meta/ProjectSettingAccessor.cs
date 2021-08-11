using System.Reflection;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class ProjectSettingAccessor : PluginConfigurationItemAccessor
	{
		public ProjectSettingAccessor(PluginConfiguration configuration, MemberInfo member, Accessor parent) : base(configuration, member, parent) { }

		public override bool exists => storage.ContainsKey(key);
		
		private DictionaryAsset storage => configuration.projectSettingsAsset;

		public override void Load()
		{
			if (!definedType.IsAssignableFrom(valueType))
			{
				Debug.LogWarning($"Failed to cast project setting '{configuration.plugin.id}.{key}' as '{definedType.CSharpFullName()}', reverting to default.");
				value = defaultValue;
			}

			value = storage[key];
		}

		public override void Save()
		{
			if (storage.ContainsKey(key))
			{
				storage[key] = value;
			}
			else
			{
				storage.Add(key, value);
			}

			configuration.SaveProjectSettingsAsset();
		}
	}
}