using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class ReflectedEditor : Editor
	{
		protected ReflectedEditor(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			base.Initialize();

			bindingFlags = MemberAccessor.DefaultBindingFlags;

			accessor.valueTypeChanged += previousType => Reflect();
		}

		public BindingFlags bindingFlags { get; private set; }

		private float _adaptiveWidth;

		protected virtual bool Include(MemberInfo m)
		{
			return m.HasAttribute<InspectableAttribute>() || m.HasAttribute<InspectableIfAttribute>();
		}

		private readonly List<string> inspectedMemberNames = new List<string>();

		protected IEnumerable<Inspector> memberInspectors
		{
			get
			{
				return inspectedMemberNames.Select(name => ChildInspector(accessor.Member(name, bindingFlags), ConfigureMemberInspector));
			}
		}

		protected virtual void ConfigureMemberInspector(Inspector inspector)
		{

		}

		public virtual void Reflect()
		{
			var adaptiveWidthAttribute = accessor.valueType.GetAttribute<InspectorFieldWidthAttribute>();
			_adaptiveWidth = adaptiveWidthAttribute?.width ?? 200;

			inspectedMemberNames.Clear();

			inspectedMemberNames.AddRange(accessor.valueType
			                               .GetMembers(bindingFlags)
			                               .Where(Include)
			                               .Select(mi => mi.ToMember())
			                               .Where(m => m.isAccessor)
			                               .OrderBy(m => m.info.GetAttribute<InspectableAttribute>()?.order ?? int.MaxValue)
			                               .ThenBy(m => m.info.MetadataToken)
			                               .Select(m => m.name));

			SetHeightDirty();
		}

		public virtual bool Display(Accessor memberAccessor)
		{
			if (memberAccessor.TryGetAttribute<InspectableIfAttribute>(out var conditionalAttribute))
			{
				return AttributeUtility.CheckCondition(accessor.valueType, accessor.value, conditionalAttribute.conditionMemberName, false);
			}

			return true;
		}

		public virtual bool ShowInFooter(Accessor memberAccessor)
		{
			return memberAccessor.HasAttribute<InspectorShowInFooterAttribute>();
		}

		protected override float GetInnerHeight(float width)
		{
			var height = 0f;

			var addedSpace = false;

			foreach (var memberInspector in memberInspectors)
			{
				if (!Display(memberInspector.accessor) || ShowInFooter(memberInspector.accessor))
				{
					continue;
				}

				height += GetMemberHeight(memberInspector, width);
				height += EditorGUIUtility.standardVerticalSpacing;

				addedSpace = true;
			}

			if (addedSpace)
			{
				height -= EditorGUIUtility.standardVerticalSpacing;
			}
			
			return height;
		}

		protected override void OnInnerGUI(Rect position)
		{
			var addedSpace = false;

			foreach (var memberInspector in memberInspectors)
			{
				if (!Display(memberInspector.accessor) || ShowInFooter(memberInspector.accessor))
				{
					continue;
				}

				var memberPosition = position.VerticalSection(ref y, GetMemberHeight(memberInspector, position.width));

				OnMemberGUI(memberInspector, memberPosition);

				y += EditorGUIUtility.standardVerticalSpacing;

				addedSpace = true;
			}

			if (addedSpace)
			{
				y -= EditorGUIUtility.standardVerticalSpacing;
			}
		}

		protected override float GetFooterHeight(float width)
		{
			var height = 0f;

			var addedSpace = false;

			foreach (var memberInspector in memberInspectors)
			{
				if (!Display(memberInspector.accessor) || !ShowInFooter(memberInspector.accessor))
				{
					continue;
				}

				height += GetMemberHeight(memberInspector, width);
				height += EditorGUIUtility.standardVerticalSpacing;

				addedSpace = true;
			}

			if (addedSpace)
			{
				height -= EditorGUIUtility.standardVerticalSpacing;
			}

			return height;
		}

		protected override void OnFooterGUI(Rect position)
		{
			var addedSpace = false;

			foreach (var memberInspector in memberInspectors)
			{
				if (!Display(memberInspector.accessor) || !ShowInFooter(memberInspector.accessor))
				{
					continue;
				}

				var memberPosition = position.VerticalSection(ref y, GetMemberHeight(memberInspector, position.width));

				OnMemberGUI(memberInspector, memberPosition);

				y += EditorGUIUtility.standardVerticalSpacing;

				addedSpace = true;
			}

			if (addedSpace)
			{
				y -= EditorGUIUtility.standardVerticalSpacing;
			}
		}

		protected virtual float GetMemberHeight(Inspector memberInspector, float width)
		{
			return memberInspector.FieldHeight(width);
		}

		protected virtual void OnMemberGUI(Inspector memberInspector, Rect memberPosition)
		{
			EditorGUI.BeginChangeCheck();

			memberInspector.DrawField(memberPosition);

			if (EditorGUI.EndChangeCheck())
			{
				OnMemberChange((MemberAccessor)memberInspector.accessor);
			}
		}

		protected virtual void OnMemberChange(MemberAccessor member) { }

		protected override float GetControlWidth()
		{
			return _adaptiveWidth;
		}
	}
}