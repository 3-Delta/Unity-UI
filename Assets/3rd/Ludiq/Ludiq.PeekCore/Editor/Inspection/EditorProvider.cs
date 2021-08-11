using System;

namespace Ludiq.PeekCore
{
	public class EditorProvider : SingleDecoratorProvider<Accessor, Editor, RegisterEditorAttribute>
	{
		protected override bool cache => false;
		
		protected override Type GetDecoratedType(Accessor accessor)
		{
			return accessor.definedType;
		}

		protected override Type ResolveDecoratorType(Type decoratedType)
		{
			return ResolveDecoratorTypeByHierarchy(decoratedType) ??
				   AutomaticReflectedEditor(decoratedType) ??
			       typeof(UnknownEditor);
		}

		private Type AutomaticReflectedEditor(Type type)
		{
			if (type.HasAttribute<InspectableAttribute>())
			{
				return typeof(AutomaticReflectedInspector);
			}

			return null;
		}

		public bool HasEditor(Type type)
		{
			return GetDecoratorType(type) != typeof(UnknownEditor);
		}

		static EditorProvider()
		{
			instance = new EditorProvider();
		}

		public static EditorProvider instance { get; private set; }
	}

	public static class XEditorProvider
	{
		public static Editor CreateUninitializedEditor(this Accessor accessor)
		{
			return EditorProvider.instance.GetDecorator(accessor);
		}
		
		public static TEditor CreateUninitializedEditor<TEditor>(this Accessor accessor) where TEditor : Editor
		{
			return EditorProvider.instance.GetDecorator<TEditor>(accessor);
		}

		public static Editor CreateInitializedEditor(this Accessor accessor)
		{
			var editor = CreateUninitializedEditor(accessor);
			editor.Initialize();
			return editor;
		}
		
		public static TEditor CreateInitializedEditor<TEditor>(this Accessor accessor) where TEditor : Editor
		{
			var editor = CreateUninitializedEditor<TEditor>(accessor);
			editor.Initialize();
			return editor;
		}

		public static bool HasEditor(this Type type)
		{
			return EditorProvider.instance.HasEditor(type);
		}

		public static bool HasEditor(this Accessor accessor)
		{
			return EditorProvider.instance.HasEditor(accessor.definedType);
		}
	}
}