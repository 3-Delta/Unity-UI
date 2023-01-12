using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class LudiqRootObjectEditor : PersistentRootEditorWrapper
	{
		protected virtual EditorLayout layout => EditorLayout.Fields;

		protected override PersistentRootEditor CreateEditor(SerializedObject serializedObject, PersistentRootEditorWrapper wrapper)
		{
			return new Persistent(serializedObject, wrapper, layout);
		}

		public class Persistent : PersistentRootEditor
		{
			public Persistent(SerializedObject serializedObject, PersistentRootEditorWrapper wrapper, EditorLayout layout) : base(serializedObject, wrapper)
			{
				accessor = Accessor.Root(serializedObject.targetObject);
				editor = accessor.CreateInitializedEditor();
				editor.layout = layout;
			}

			private readonly RootAccessor accessor;

			private readonly Editor editor;

			private bool debugFoldout;

			public override void Dispose()
			{
				editor.Dispose();
			}

			public override void OnGUI()
			{
				if (PluginContainer.anyVersionMismatch)
				{
					LudiqGUI.VersionMismatchShieldLayout();
					return;
				}

				accessor.UpdatePrefabModifications();

				EditorGUI.BeginChangeCheck();

				LudiqGUI.Space(EditorGUIUtility.standardVerticalSpacing);

				editor.DrawControlLayout(21);

				if (editor.isHeightDirty)
				{
					rootEditor.Repaint();
				}

				if (EditorGUI.EndChangeCheck())
				{
					rootEditor.Repaint();
				}

				if (LudiqCore.Configuration.developerMode && LudiqCore.Configuration.developerEditorMenu)
				{
					debugFoldout = EditorGUILayout.Foldout(debugFoldout, "Developer", true);

					if (debugFoldout)
					{
						var target = serializedObject.targetObject;

						if (GUILayout.Button("Show Serialized Data"))
						{
							((ILudiqRootObject)target).ShowData();
						}

						EditorGUI.BeginDisabledGroup(true);
						EditorGUILayout.Toggle("Prefab definition", target.IsPrefabDefinition());
						EditorGUILayout.Toggle("Prefab instance", target.IsPrefabInstance());
						EditorGUILayout.Toggle("Connected prefab instance", target.IsConnectedPrefabInstance());
						EditorGUILayout.Toggle("Disconnected prefab instance", target.IsDisconnectedPrefabInstance());
						EditorGUILayout.Toggle("Scene bound", target.IsSceneBound());
						EditorGUILayout.ObjectField("Prefab definition", target.GetPrefabDefinition(), typeof(Object), true);
						EditorGUI.EndDisabledGroup();
					}
				}
				else
				{
					LudiqGUI.Space(EditorGUIUtility.standardVerticalSpacing);
				}
			}
		}
	}
}