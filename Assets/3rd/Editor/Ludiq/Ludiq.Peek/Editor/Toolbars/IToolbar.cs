using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public interface IToolbar : IReadOnlyList<ITool>
	{
		ITool mainTool { get; }

		void Initialize();

		void Update();

		bool isValid { get; }

		new NoAllocEnumerator<ITool> GetEnumerator();
	}
}