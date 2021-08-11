using System.Linq;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public class AnnotationDisabler
	{
#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Disable Gizmos", priority = LudiqProduct.InternalToolsMenuPriority + 502)]
#endif
		public static void DisableGizmos()
		{
			foreach (var type in Codebase.types.Where(type => type.HasAttribute<DisableAnnotationAttribute>()))
			{
				var attribute = type.GetAttribute<DisableAnnotationAttribute>();

				var annotation = AnnotationUtility.GetAnnotation(type);

				if (annotation != null)
				{
					annotation.iconEnabled = !attribute.disableIcon;
					annotation.gizmoEnabled = !attribute.disableGizmo;
				}
			}
		}
	}
}