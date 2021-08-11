namespace Ludiq.PeekCore
{
	public class AotStubWriterProvider : SingleDecoratorProvider<object, AotStubWriter, RegisterAotStubWriterAttribute>
	{
		static AotStubWriterProvider()
		{
			instance = new AotStubWriterProvider();
		}

		public static AotStubWriterProvider instance { get; }

		protected override bool cache => true;
	}
}