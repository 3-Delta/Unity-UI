using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[assembly: RegisterPluginModuleType(typeof(PluginConfiguration), true)]

namespace Ludiq.PeekCore
{
	public class PluginConfiguration : IPluginModule, IEnumerable<PluginConfigurationItemAccessor>
	{
		protected PluginConfiguration(Plugin plugin)
		{
			this.plugin = plugin;

			this.accessor = Accessor.Root(this);
		}

		public virtual void Initialize()
		{
			savedVersion = plugin.manifest.version;
			Load();
		}

		public virtual void LateInitialize() { }

		public Plugin plugin { get; }

		public Accessor accessor { get; }

		public Editor editor { get; private set; }
		
		public virtual string label => plugin.manifest.name;

		public virtual string header => plugin.manifest.name;

		
		#region Lifecycle

		private void Load()
		{
			LoadEditorPrefs();
			LoadProjectSettings();
		}

		public void Reset()
		{
			foreach (var item in allItems)
			{
				item.Reset();
			}
		}

		public void Save()
		{
			foreach (var item in allItems)
			{
				item.Save();
			}
		}

		public void Save(string memberName)
		{
			GetAccessor(memberName).Save();
		}

		#endregion



		#region Drawing
		
		public SettingsProvider CreateSettingsProvider(SettingsScope scope)
		{
			return new SettingsProvider(ScopeRoot(scope) + "/Ludiq/" + label, scope)
			{
				label = label,
				guiHandler = query => OnGUI(scope, query),
				footerBarGuiHandler = () => OnFooterGUI(scope),
				keywords = this[scope].Where(i => i.visible).Select(Haystack),
			};
		}
		
		public static string ScopeRoot(SettingsScope scope)
		{
			switch (scope)
			{
				case SettingsScope.User: return "Preferences";
				case SettingsScope.Project: return "Project";
				default: throw scope.Unexpected();
			}
		}

		public static string ScopeName(SettingsScope scope)
		{
			switch (scope)
			{
				case SettingsScope.User: return "Preferences";
				case SettingsScope.Project: return "Project Settings";
				default: throw scope.Unexpected();
			}
		}

		private static class Styles
		{
			static Styles()
			{
				background = new GUIStyle();
				background.padding = new RectOffset(10, 10, 0, 0);
			}

			public static readonly GUIStyle background;
		}

		private string Haystack(PluginConfigurationItemAccessor item)
		{
			return item.label.text;
		}

		public void OnGUI(SettingsScope scope, string query)
		{
			if (!PluginContainer.initialized)
			{
				return;
			}

			if (editor == null)
			{
				editor = accessor.CreateInitializedEditor();
			}

			LudiqGUI.BeginVertical(Styles.background);
			
			EditorGUI.BeginChangeCheck();

			using (Inspector.expandTooltip.Override(true))
			{
				foreach (var item in this[scope].Where(i => i.visible))
				{
					EditorGUI.BeginChangeCheck();

					LudiqGUI.Space(2);

					if (!string.IsNullOrEmpty(query))
					{
						var haystack = Haystack(item);

						var matchesSearch = SearchUtility.Matches(query, haystack);

						using (LudiqGUI.color.Override(matchesSearch ? Color.white : Color.white.WithAlphaMultiplied(0.5f)))
						using (LudiqGUIUtility.LabelHighlight(query))
						{
							editor.ChildInspector(item).DrawFieldLayout();
						}
					}
					else
					{
						editor.ChildInspector(item).DrawFieldLayout();
					}

					LudiqGUI.Space(2);

					if (EditorGUI.EndChangeCheck())
					{
						OnItemChange(item);
					}
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				Save();
				InternalEditorUtility.RepaintAllViews();
			}

			LudiqGUI.EndVertical();
		}

		protected virtual void OnItemChange(PluginConfigurationItemAccessor item)
		{

		}

		public void OnFooterGUI(SettingsScope scope)
		{
			if (!PluginContainer.initialized)
			{
				return;
			} 

			LudiqGUI.BeginVertical(Styles.background);
			
			LudiqGUI.Space(8);

			if (GUILayout.Button("Reset to Defaults"))
			{
				if (EditorUtility.DisplayDialog("Reset to Defaults", $"Are you sure you want to reset your {ScopeName(scope).ToLower()} to default?", "Reset", "Cancel"))
				{
					foreach (var item in this[scope])
					{
						item.Reset();
						OnItemChange(item);
						item.Save();
					}
					
					InternalEditorUtility.RepaintAllViews();
				}
			}
			
			LudiqGUI.Space(8);

			LudiqGUI.EndVertical();
		}

		#endregion 



		#region All Items

		private IEnumerable<PluginConfigurationItemAccessor> allItems => LinqUtility.Concat<PluginConfigurationItemAccessor>(editorPrefs, projectSettings);

		public IEnumerator<PluginConfigurationItemAccessor> GetEnumerator()
		{
			return allItems.OrderBy(i => i.member.MetadataToken).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		} 

		public PluginConfigurationItemAccessor GetAccessor(string memberName)
		{
			return allItems.First(item => item.member.Name == memberName);
		}

		public IEnumerable<PluginConfigurationItemAccessor> this[SettingsScope scope]
		{
			get
			{
				switch (scope)
				{
					case SettingsScope.User: return editorPrefs;
					case SettingsScope.Project: return projectSettings;
					default: throw scope.Unexpected();
				}
			}
		}

		#endregion



		#region Editor Prefs

		internal List<EditorPrefAccessor> editorPrefs;

		private void LoadEditorPrefs()
		{
			editorPrefs = new List<EditorPrefAccessor>();
			
			foreach (var memberInfo in GetType().GetMembers().Where(f => f.HasAttribute<EditorPrefAttribute>()).OrderBy(m => m.MetadataToken))
			{
				editorPrefs.Add(accessor.EditorPref(this, memberInfo));
			}
		}

		#endregion



		#region Project Settings

		internal List<ProjectSettingAccessor> projectSettings;

		private string projectSettingsStoragePath => plugin.paths.projectSettings;
		
		internal DictionaryAsset projectSettingsAsset { get; private set; }

		private void LoadProjectSettings()
		{
			AssetUtility.TryLoad(projectSettingsStoragePath, out DictionaryAsset _projectSettingsAsset);

			projectSettingsAsset = _projectSettingsAsset;

			projectSettings = new List<ProjectSettingAccessor>();

			foreach (var memberInfo in GetType().GetMembers().Where(f => f.HasAttribute<ProjectSettingAttribute>()).OrderBy(m => m.MetadataToken))
			{
				projectSettings.Add(accessor.ProjectSetting(this, memberInfo));
			}
		}

		public void SaveProjectSettingsAsset()
		{
			if (projectSettingsAsset == null)
			{
				LoadProjectSettings();
			}

			EditorUtility.SetDirty(projectSettingsAsset);
		}

		#endregion



		#region Items

		/// <summary>
		/// Whether the plugin was properly setup.
		/// </summary>
		[ProjectSetting(visibleCondition = nameof(developerMode), resettable = false)]
		public bool projectSetupCompleted { get; internal set; }

		/// <summary>
		/// Whether the plugin was properly setup.
		/// </summary>
		[EditorPref(visibleCondition = nameof(developerMode), resettable = false)]
		public bool editorSetupCompleted { get; internal set; }

		/// <summary>
		/// The last version to which the plugin successfully upgraded.
		/// </summary>
		[ProjectSetting(visibleCondition = nameof(developerMode), resettable = false)]
		public SemanticVersion savedVersion { get; internal set; }

		protected bool developerMode => LudiqCore.Configuration.developerMode;

		#endregion



#region Menu

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Delete All Project Settings", priority = LudiqProduct.InternalToolsMenuPriority + 401)]
#endif
		public static void DeleteAllProjectSettings()
		{
			foreach (var plugin in PluginContainer.plugins)
			{
				AssetDatabase.DeleteAsset(PathUtility.FromProject(plugin.configuration.projectSettingsStoragePath));
			}
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Delete All Editor Prefs", priority = LudiqProduct.InternalToolsMenuPriority + 402)]
#endif
		public static void DeleteAllEditorPrefs()
		{
			foreach (var plugin in PluginContainer.plugins)
			{
				foreach (var editorPref in plugin.configuration.editorPrefs)
				{
					EditorPrefs.DeleteKey(editorPref.namespacedKey);
				}
			}
		}
		
#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Delete All Player Prefs", priority = LudiqProduct.InternalToolsMenuPriority + 403)]
#endif
		public static void DeleteAllPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

#endregion
	}
}
