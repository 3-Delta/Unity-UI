using System.Collections.Generic;
using UnityEditor;
using RootEditor = UnityEditor.Editor;

namespace Ludiq.PeekCore
{
	public abstract class PersistentRootEditorWrapper : RootEditor
	{
		private static readonly Dictionary<SerializedObject, PersistentRootEditor> editors = new Dictionary<SerializedObject, PersistentRootEditor>();

		protected abstract PersistentRootEditor CreateEditor(SerializedObject serializedObject, PersistentRootEditorWrapper wrapper);
		
		public sealed override void OnInspectorGUI()
		{
			if (serializedObject.isEditingMultipleObjects)
			{
				EditorGUILayout.HelpBox("Multi-object editing is not supported.", MessageType.Info);
				return;
			}
			
			if (!editors.TryGetValue(serializedObject, out var editor))
			{
				editor = CreateEditor(serializedObject, this);
				editors.Add(serializedObject, editor);
			}

			editor.OnGUI();
		}

		static PersistentRootEditorWrapper()
		{
			EditorApplicationUtility.onSelectionChange += ClearCache;
		}
		private static void ClearCache()
		{
			try
			{
				foreach (var individualDrawer in editors.Values)
				{
					individualDrawer.Dispose();
				}
			}
			finally
			{
				editors.Clear();
			}
		}
	}
}