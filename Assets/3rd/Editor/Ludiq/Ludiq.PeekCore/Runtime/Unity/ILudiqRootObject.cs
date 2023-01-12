namespace Ludiq.PeekCore
{
	public interface ILudiqRootObject
	{
		void ShowData();

		void OnBeforeSerialize();

		void OnAfterSerialize();

		void OnBeforeDeserialize();

		void OnAfterDeserialize();
	}
}