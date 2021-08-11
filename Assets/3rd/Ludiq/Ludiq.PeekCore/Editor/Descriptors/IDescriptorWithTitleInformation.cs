namespace Ludiq.PeekCore
{
	public interface IDescriptorWithTitleInformation : IDescriptor
	{
		string Title();
		string Summary();
	}
}
