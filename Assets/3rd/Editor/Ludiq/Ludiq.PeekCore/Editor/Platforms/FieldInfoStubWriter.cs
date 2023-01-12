using System.Reflection;
using Ludiq.PeekCore;

[assembly: RegisterAotStubWriter(typeof(FieldInfo), typeof(FieldInfoStubWriter))]

namespace Ludiq.PeekCore
{
	public class FieldInfoStubWriter : AccessorInfoStubWriter<FieldInfo>
	{
		public FieldInfoStubWriter(FieldInfo fieldInfo) : base(fieldInfo) { }

		protected override IOptimizedAccessor GetOptimizedAccessor(FieldInfo fieldInfo)
		{
			return fieldInfo.Prewarm();
		}

		protected override bool supportsOptimization => stub.SupportsOptimization();
	}
}