using System;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Member), typeof(MemberInspector))]

namespace Ludiq.PeekCore
{
	public sealed class MemberInspector : Inspector
	{
		public MemberInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			accessor.instantiate = true;

			base.Initialize();

			memberFilter = accessor.GetAttribute<MemberFilter>() ?? MemberFilter.Any;
			memberTypeFilter = accessor.GetAttribute<TypeFilter>() ?? TypeFilter.Any;
			action = MemberAction.None;
		}

		private IFuzzyOptionTree GetOptions()
		{
			return new MemberOptionTree(Codebase.types, memberFilter, memberTypeFilter, action);
		}

		public static class Styles
		{
			static Styles()
			{
				failurePopup = new GUIStyle(EditorStyles.popup);
				failurePopup.normal.textColor = Color.red;
				failurePopup.active.textColor = Color.red;
				failurePopup.hover.textColor = Color.red;
				failurePopup.focused.textColor = Color.red;
			}

			public static readonly GUIStyle failurePopup;
		}

		#region Accessors

		private Accessor nameAccessor => accessor[nameof(Member.name)];

		private Accessor infoAccessor => accessor[nameof(Member.info)];

		private Accessor targetTypeAccessor => accessor[nameof(Member.targetType)];

		#endregion

		#region Settings

		private MemberAction action;

		private ReadOnlyCollection<Type> typeSet;

		private MemberFilter memberFilter;

		private TypeFilter memberTypeFilter;

		private bool expectingBoolean => memberTypeFilter?.ExpectsBoolean ?? false;

		#endregion

		#region Rendering

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var popupLabel = new GUIContent("(Nothing)");
			var popupStyle = EditorStyles.popup;

			if (accessor.value != null)
			{
				popupLabel.text = (string)nameAccessor.value;

				if (LudiqCore.Configuration.humanNaming && popupLabel.text != null)
				{
					popupLabel.text = popupLabel.text.Prettify();
				}

				try
				{
					var member = ((Member)accessor.value);
					popupLabel.image = member.pseudoDeclaringType.Icon()[IconSize.Small];
					popupLabel.text = member.info.DisplayName(action, expectingBoolean);
				}
				catch
				{
					popupStyle = Styles.failurePopup;
				}
			}

			if (popupLabel.image != null)
			{
				popupLabel.text = " " + popupLabel.text;
			}

			var newMemberManipulator = (Member)LudiqGUI.FuzzyPopup
			(
				position,
				GetOptions,
				(Member)accessor.value,
				new GUIContent(popupLabel),
				popupStyle
			);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newMemberManipulator;
			}
		}

		#endregion
	}
}