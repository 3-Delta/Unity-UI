using System;
using System.Collections.Generic;
using Ludiq.PeekCore;
using UnityEngine;

[assembly: MapToPlugin(typeof(LudiqCoreConfiguration), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	public sealed class LudiqCoreConfiguration : PluginConfiguration
	{
		private LudiqCoreConfiguration(LudiqCore plugin) : base(plugin) { }



		#region Editor Prefs

		private bool _humanNaming = true;

		private LanguageIconsSkin _languageIconsSkin = LanguageIconsSkin.VisualStudioMonochrome;

		public event Action namingSchemeChanged;

		public override string header => "Core";

		/// <summary>
		/// Whether programming names should be converted into a more human-readable format.
		/// </summary>
		[EditorPref(visible = true, resettable = true)]
		public bool humanNaming
		{
			get => _humanNaming;
			set
			{
				_humanNaming = value;
				namingSchemeChanged?.Invoke();
			}
		}

		/// <summary>
		/// Whether the Unity object fields should use the fuzzy finder instead
		/// of the default object picker window.
		/// </summary>
		[EditorPref]
		public bool fuzzyObjectPicker { get; set; } = true;

		/// <summary>
		/// The maximum amount of search results to display.
		/// </summary>
		[EditorPref(visible = true, resettable = true)]
		public int maxSearchResults { get; set; } = 50;

		/// <summary>
		/// Whether inherited below should be grouped at the bottom of the options list.
		/// </summary>
		[EditorPref]
		public bool groupInheritedMembers { get; set; } = true;

		/// <summary>
		/// Whether the fuzzy finder should display options that are obsolete.
		/// </summary>
		[EditorPref]
		public bool obsoleteOptions { get; set; } = false;

		/// <summary>
		/// The skin to use for language related (C# / VB) icons.
		/// </summary>
		[EditorPref]
		public LanguageIconsSkin LanguageIconsSkin
		{
			get => _languageIconsSkin;
			set
			{
				_languageIconsSkin = value;
				Icons.Language.skin = value;
			}
		}

		/// <summary>
		/// Whether the height of the fuzzy finder should be limited to the
		/// main editor window height. This is meant to fix Y offset issues on OSX,
		/// but will cut the fuzzy finder if this window is not maximized to the screen size.
		/// </summary>
		[EditorPref(visibleCondition = nameof(isEditorOSX))]
		public bool limitFuzzyFinderHeight { get; set; } = true;

		private bool _developerMode = false;

		/// <summary>
		/// Enables additional options and logging for debugging purposes.
		/// </summary>
		[EditorPref(resettable = false)]
		public new bool developerMode
		{
			get => _developerMode;
			set
			{
				if (value == developerMode)
				{
					return;
				}

				_developerMode = value;

				DefineUtility.ToggleDefine("LUDIQ_DEVELOPER", value);
			}
		}

		[EditorPref(visibleCondition = nameof(developerMode))]
		public bool developerEditorMenu { get; set; } = false;

		/// <summary>
		/// Whether the log should track accessor state.
		/// </summary>
		[EditorPref(visibleCondition = nameof(developerMode))]
		public bool trackAccessorState { get; set; } = false;

		/// <summary>
		/// Whether additional helpers should be shown in the inspector for debugging and profiling.
		/// </summary>
		[EditorPref(visibleCondition = nameof(developerMode))]
		public bool debugInspectorGUI { get; set; } = false;

		// Needs to be proptected to avoid stripping
		private bool isEditorOSX => Application.platform == RuntimePlatform.OSXEditor;

		#endregion



		#region Project Settings

		/// <summary>
		/// Whether the project was updated from Bolt 1.
		/// </summary>
		[ProjectSetting]
		public bool legacyProject { get; set; } = false;

		[ProjectSetting(visible = false, resettable = false)]
		public HashSet<Member> favoriteMembers { get; set; } = new HashSet<Member>();

		#endregion
	}
}