using System;

namespace Ludiq.PeekCore
{
	public interface IObservableDisposable : IDisposable
	{
		bool IsDisposed { get; }
	}

	public static class XObservableDisposable
	{
		public static void EnsureNotDisposed(this IObservableDisposable disposable)
		{
			if (disposable.IsDisposed)
			{
				throw new ObjectDisposedException(disposable.ToSafeString());
			}
		}
	}
}