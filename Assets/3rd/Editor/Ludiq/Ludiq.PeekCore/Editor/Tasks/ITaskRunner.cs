namespace Ludiq.PeekCore
{
	public interface ITaskRunner
	{
		void Run(Task task);
		void Report(Task task);
		bool runsCurrentThread { get; }
	}
}
