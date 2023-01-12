using System;
using UnityEditor;
using RootEditor = UnityEditor.Editor;

namespace Ludiq.PeekCore
{
	public abstract class PersistentRootEditor : IDisposable
	{
		protected PersistentRootEditor(SerializedObject serializedObject, RootEditor rootEditor)
		{
			this.serializedObject = serializedObject;
			this.rootEditor = rootEditor;
		}

		public RootEditor rootEditor { get; }

		public SerializedObject serializedObject { get; }

		public abstract void OnGUI();

		public virtual void Dispose() { }
	}
}