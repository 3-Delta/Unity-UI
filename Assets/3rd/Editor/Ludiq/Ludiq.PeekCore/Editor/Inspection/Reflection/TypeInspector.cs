using System;
using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Type), typeof(TypeInspector))]

namespace Ludiq.PeekCore
{
	public sealed class TypeInspector : Inspector
	{
		private TypeFilter rootTypeFilter;
		
		private TypeTree rootTypeTree = new TypeTree();

		public bool hasHideRootAttribute;

		public bool forceHideRoot { get; set; }

		public Type rootTypeGenericParameter { get; set; }

		private bool hideRoot => forceHideRoot || (hasHideRootAttribute && accessor.value != null);

		public TypeInspector(Accessor accessor) : base(accessor)
		{
			hasHideRootAttribute = accessor.HasAttribute<InspectorTypeHideRootAttribute>();

			rootTypeFilter = accessor.GetAttribute<TypeFilter>() ?? TypeFilter.Any;

			accessor.valueChanged += _ => RefreshRootTypeTree();
		}

		public void RefreshRootTypeTree()
		{
			var type = (Type)accessor.value;

			if (rootTypeGenericParameter != null)
			{
				rootTypeTree.ChangeType(type, rootTypeGenericParameter);
			}
			else
			{
				rootTypeTree.ChangeType(type);
			}
		}

		private Func<IFuzzyOptionTree> GetOptions(TypeTree typeTree)
		{
			if (typeTree == rootTypeTree)
			{
				if (rootTypeFilter == TypeFilter.Any)
				{
					return () => TypeOptionTree.All;
				}
				else
				{
					return () => new TypeOptionTree(Codebase.types, rootTypeFilter);
				}
			}
			else
			{
				return () => new TypeOptionTree(Codebase.types, typeTree.filter);
			}
		}

		protected override float GetControlHeight(float width)
		{
			return LudiqGUI.GetTypeTreeFieldHeight(rootTypeTree, !hideRoot);
		}

		protected override float GetControlWidth()
		{
			return LudiqGUI.GetTypeTreeFieldAdaptiveWidth(rootTypeTree, !hideRoot);
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			LudiqGUI.TypeTreeField(ref position, GUIContent.none, rootTypeTree, !hideRoot, GetOptions);

			if (EditorGUI.EndChangeCheck())
			{
				var newType = rootTypeTree.GetSubstitutedType();
				accessor.RecordUndo();
				accessor.value = newType;
			}
		}
	}
}