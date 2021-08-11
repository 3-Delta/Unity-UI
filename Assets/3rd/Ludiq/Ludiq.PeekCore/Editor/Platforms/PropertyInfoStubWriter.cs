using System.Reflection;
using Ludiq.PeekCore;

[assembly: RegisterAotStubWriter(typeof(PropertyInfo), typeof(PropertyInfoStubWriter))]

namespace Ludiq.PeekCore
{
	public class PropertyInfoStubWriter : AccessorInfoStubWriter<PropertyInfo>
	{
		public PropertyInfoStubWriter(PropertyInfo propertyInfo) : base(propertyInfo) { }

		protected override IOptimizedAccessor GetOptimizedAccessor(PropertyInfo propertyInfo)
		{
			return propertyInfo.Prewarm();
		}

		protected override bool supportsOptimization => stub.SupportsOptimization();

		public override bool skip => base.skip || stub.IsIndexer();
	}
}