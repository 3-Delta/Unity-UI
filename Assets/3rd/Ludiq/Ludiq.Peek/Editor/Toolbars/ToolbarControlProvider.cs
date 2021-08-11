using System;
using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class ToolbarControlProvider
	{
		private readonly Dictionary<Key, ToolbarControl> controlsByKeys = new Dictionary<Key, ToolbarControl>();

		public ToolbarWindow window { get; }

		public event Action cleaningUp;

		public ToolbarControlProvider(ToolbarWindow window)
		{
			this.window = window;

			AssemblyReloadEvents.beforeAssemblyReload += Cleanup;
			EditorApplication.quitting += Cleanup;
			EditorApplication.update += FreeInvalid;
		}

		private void Cleanup()
		{
			cleaningUp?.Invoke();

			foreach (var stripControl in controlsByKeys.Values)
			{
				stripControl?.CloseAllTools();
			}
		}

		public ToolbarControl GetControl(IToolbar toolbar, object tag = null)
		{
			var key = new Key(toolbar, tag);

			if (!controlsByKeys.TryGetValue(key, out var toolbarControl))
			{
				toolbarControl = new ToolbarControl(toolbar, window);
				controlsByKeys.Add(key, toolbarControl);
			}

			return toolbarControl;
		}
		
		private struct Key
		{
			private readonly IToolbar toolbar;

			private readonly object tag;

			public Key(IToolbar toolbar, object tag = null)
			{
				this.toolbar = toolbar;
				this.tag = tag;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is Key other))
				{
					return false;
				}

				return toolbar == other.toolbar &&
				       tag == other.tag;
			}

			public override int GetHashCode()
			{
				return HashUtility.GetHashCode(toolbar, tag);
			}
		}



		#region Freeing

		private DateTime lastFreeTime;

		private readonly TimeSpan freeInterval = TimeSpan.FromSeconds(30);
		
		private void FreeInvalid()
		{
			if (DateTime.UtcNow > lastFreeTime + freeInterval)
			{
				var toRemove = HashSetPool<Key>.New();

				foreach (var controlByKey in controlsByKeys)
				{
					if (!controlByKey.Value.toolbar.isValid)
					{
						toRemove.Add(controlByKey.Key);
					}
				}

				foreach (var tr in toRemove)
				{
					controlsByKeys.Remove(tr);
				}

				toRemove.Free();

				lastFreeTime = DateTime.UtcNow;
			}
		}

		#endregion
	}
}