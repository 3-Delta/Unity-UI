namespace Ludiq.PeekCore
{
	public sealed class DescriptorProvider : SingleDecoratorProvider<object, IDescriptor, RegisterDescriptorAttribute>
	{
		static DescriptorProvider()
		{
			instance = new DescriptorProvider();
		}

		public static DescriptorProvider instance { get; }

		protected override bool cache => true;

		private DescriptorProvider()
		{
			LudiqCore.Configuration.namingSchemeChanged += DescribeAll;
			XmlDocumentation.onLoaded += DescribeAll;
		}



		#region Shortcuts

		public void Describe(object describable)
		{
			GetDecorator(describable).SetDirty();
		}

		public void DescribeAll()
		{
			foreach (var descriptor in decorators.Values)
			{
				descriptor.SetDirty();
			}
		}

		public IDescriptor Descriptor(object target)
		{
			return GetDecorator(target);
		}

		public TDescriptor Descriptor<TDescriptor>(object target) where TDescriptor : IDescriptor
		{
			return GetDecorator<TDescriptor>(target);
		}

		public IDescription Description(object target)
		{
			var descriptor = Descriptor(target);
			descriptor.Validate();
			return descriptor.description;
		}

		public TDescription Description<TDescription>(object target) where TDescription : IDescription
		{
			return Description(target).CastTo<TDescription>();
		}

		#endregion
	}

	public static class XDescriptorProvider
	{
		public static void Describe(this object target)
		{
			DescriptorProvider.instance.Describe(target);
		}

		public static bool HasDescriptor(this object target)
		{
			Ensure.That(nameof(target)).IsNotNull(target);

			return DescriptorProvider.instance.HasDecorator(target.GetType());
		}

		public static IDescriptor Descriptor(this object target)
		{
			return DescriptorProvider.instance.Descriptor(target);
		}

		public static TDescriptor Descriptor<TDescriptor>(this object target) where TDescriptor : IDescriptor
		{
			return DescriptorProvider.instance.Descriptor<TDescriptor>(target);
		}

		public static IDescription Description(this object target)
		{
			return DescriptorProvider.instance.Description(target);
		}

		public static TDescription Description<TDescription>(this object target) where TDescription : IDescription
		{
			return DescriptorProvider.instance.Description<TDescription>(target);
		}
	}
}