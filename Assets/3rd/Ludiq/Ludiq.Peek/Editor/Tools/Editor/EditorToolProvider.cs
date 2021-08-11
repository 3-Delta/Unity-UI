using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class EditorToolProvider : SingleDecoratorProvider<UnityObject[], IEditorTool, RegisterEditorToolAttribute>
	{
		public static EditorToolProvider instance { get; }

		private EditorToolProvider() : base() { }

		static EditorToolProvider()
		{
			instance = new EditorToolProvider();
		}

		protected override Type GetDecoratedType(UnityObject[] targets)
		{
			return ArrayTypeUtility.GetCommonType(targets);
		}

		protected override bool cache => true;

		protected override IEqualityComparer<UnityObject[]> decoratedComparer => ArrayEqualityComparer<UnityObject>.Default;

		protected override IEditorTool CreateDecorator(Type decoratorType, UnityObject[] targets)
		{
			var editorTool = decoratorType.Instantiate(false, ArrayTypeUtility.RetypeArray(targets)).CastTo<IEditorTool>();
			return editorTool;
		}

		public static IEditorTool GetEditorTool(IEnumerable<UnityObject> targets)
		{
			if (!ArrayTypeUtility.TryGetCommonType(targets, out var commonType))
			{
				return null;
			}

			// Array gets copied on creation anyway so it's safe to pool
			var targetsArray = targets.ToArrayPooled();
			var editorTool = instance.GetDecorator(targetsArray);
			targetsArray.Free();
			return editorTool;
		}

		public static IEditorTool GetEditorTool(params UnityObject[] targets)
		{
			if (!ArrayTypeUtility.TryGetCommonType(targets, out var commonType))
			{
				return null;
			}

			return instance.GetDecorator(targets);
		}
	}
}