using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public abstract class ObjectToolbar<T> : Toolbar where T : UnityObject
	{
		public T[] targets { get; }

		protected T target
		{
			get
			{
				if (!hasSingleTarget)
				{
					throw new InvalidOperationException("Editor tool is editing multiple targets.");
				}

				return firstTarget;
			}
		}

		protected T firstTarget => targets[0];

		protected virtual bool hasMultipleTargets => targets.Length > 1;

		protected virtual bool hasSingleTarget => targets.Length == 1;

		public override bool isValid
		{
			get
			{
				foreach (var target in targets)
				{
					if (target == null)
					{
						return false;
					}
				}

				return true;
			}
		}

		protected ObjectToolbar(T[] targets) : base()
		{
			this.targets = targets;
		}

		public override void Initialize()
		{
			base.Initialize();

			mainTool = EditorToolProvider.GetEditorTool(targets);
		}

		protected override void UpdateTools(IList<ITool> tools)
		{
			if (mainTool != null)
			{
				tools.Add(mainTool);
			}
		}
	}
}