using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;
	
	[CustomPropertyDrawer(typeof(UnityObject), true)]
	public class ReferencePropertyDrawer : PropertyDrawer
	{
		private static Event e => Event.current;

		public static EditorWindow lastPopup;

		private static void DefaultField(SerializedProperty property, GUIContent label, Rect fieldPosition)
		{
			try
			{
				UnityEditorDynamic.EditorGUI.DefaultPropertyField(fieldPosition, property, label);
			}
			catch (TargetInvocationException tex)
			{
				if (tex.InnerException is ExitGUIException exitGuiEx)
				{
					throw exitGuiEx;
				}
				else
				{
					throw;
				}
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!PluginContainer.initialized)
			{
				base.OnGUI(position, property, label);
				return;
			}

			if (PeekPlugin.Configuration.enableReferenceInspector && !property.hasMultipleDifferentValues && property.objectReferenceValue != null)
			{
				Rect buttonPosition, fieldPosition;

				if (label != GUIContent.none)
				{
					buttonPosition = new Rect
					(
						position.x + EditorGUIUtility.labelWidth - IconSize.Small - 1,
						position.y,
						IconSize.Small,
						IconSize.Small
					);

					fieldPosition = position;
				}
				else
				{
					buttonPosition = new Rect
					(
						position.xMax - IconSize.Small,
						position.y + 1,
						IconSize.Small,
						IconSize.Small
					);

					fieldPosition = new Rect
					(
						position.x,
						position.y,
						position.width - buttonPosition.width - 2,
						position.height
					);
				}

				DefaultField(property, label, fieldPosition);

				var isActive = PopupWatcher.IsOpenOrJustClosed(lastPopup);
				
				var activatedButton = LudiqGUI.DropdownToggle(buttonPosition, isActive, LudiqGUIUtility.TempContent(PeekPlugin.Icons.propertyDrawer?[IconSize.Small]), GUIStyle.none);
				
				if (activatedButton && !isActive)
				{
					PopupWatcher.Release(lastPopup);
					lastPopup = null;

					var targets = new[] {property.objectReferenceValue};
					var activatorGuiPosition = buttonPosition;
					var activatorScreenPosition = LudiqGUIUtility.GUIToScreenRect(activatorGuiPosition);

					if (e.IsContextMouseButton())
					{
						if (property.objectReferenceValue is GameObject go)
						{
							GameObjectContextMenu.Open(new[] {go}, activatorScreenPosition);
						}
						else
						{
							UnityObjectContextMenu.Open(targets, activatorGuiPosition);
						}
					}
					else
					{
						lastPopup = EditorPopup.Open(targets, activatorScreenPosition);
						PopupWatcher.Watch(lastPopup);
					}
				}
			}
			else
			{
				DefaultField(property, label, position);
			}
		}
	}
}