namespace Ludiq.PeekCore
{
	public interface ISearchResult
	{
		object result { get; }
		float relevance { get; }
	}

	public interface ISearchResult<out T> : ISearchResult
	{
		new T result { get; }
	}

	public struct SearchResult<T> : ISearchResult, ISearchResult<T>
	{
		public T result { get; }
		public float relevance { get; }

		object ISearchResult.result => result;

		public SearchResult(T result, float relevance)
		{
			this.result = result;
			this.relevance = relevance;
		}
	}
}