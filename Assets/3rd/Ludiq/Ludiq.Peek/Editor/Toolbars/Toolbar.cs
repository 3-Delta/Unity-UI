using System.Collections;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public abstract class Toolbar : IToolbar
	{
		private readonly List<ITool> tools;

		public ITool mainTool { get; protected set; }

		protected Toolbar()
		{
			tools = new List<ITool>();
		}

		public virtual void Initialize() { }

		public virtual void Update()
		{
			tools.Clear();
			UpdateTools(tools);
		}

		public virtual bool isValid => true;

		protected abstract void UpdateTools(IList<ITool> tools);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<ITool> IEnumerable<ITool>.GetEnumerator()
		{
			return GetEnumerator();
		}

		public NoAllocEnumerator<ITool> GetEnumerator()
		{
			return new NoAllocEnumerator<ITool>(tools);
		}

		public int Count => tools.Count;

		public ITool this[int index] => tools[index];
	}
}