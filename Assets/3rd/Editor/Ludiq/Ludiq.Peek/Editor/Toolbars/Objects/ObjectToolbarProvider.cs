using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class ObjectToolbarProvider : SingleDecoratorProvider<UnityObject[], IToolbar, RegisterObjectToolbarAttribute>
	{
		private static ObjectToolbarProvider instance { get; }

		private ObjectToolbarProvider() : base() { }

		static ObjectToolbarProvider()
		{
			instance = new ObjectToolbarProvider();
		}

		protected override Type GetDecoratedType(UnityObject[] targets)
		{
			return ArrayTypeUtility.GetCommonType(targets);
		}

		protected override bool cache => true;

		protected override IEqualityComparer<UnityObject[]> decoratedComparer => ArrayEqualityComparer<UnityObject>.Default;

		protected override IToolbar CreateDecorator(Type decoratorType, UnityObject[] targets)
		{
			var strip = decoratorType.Instantiate(false, ArrayTypeUtility.RetypeArray(targets)).CastTo<IToolbar>();
			strip.Initialize();
			return strip;
		}

		public static IToolbar GetToolbar(IEnumerable<UnityObject> targets)
		{
			// Array gets copied on creation anyway so it's safe to pool
			var targetsArray = targets.ToArrayPooled();
			var strip = instance.GetDecorator(targetsArray);
			targetsArray.Free();
			return strip;
		}

		public static IToolbar GetToolbar(params UnityObject[] targets)
		{
			return instance.GetDecorator(targets);
		}
	}
}