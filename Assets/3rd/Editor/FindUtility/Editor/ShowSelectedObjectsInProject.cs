using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ShowSelectedObjectsInProject
{
    [MenuItem("Tools/Misc/Show Selected Objects in Project %#g")]
    public static void ShowSelectedObjectsInLastInteractedProjectBrowser()
    {
        if (Selection.objects == null || Selection.objects.Length <= 1)
        {
            return;
        }

        Type projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
        if (projectBrowserType == null)
        {
            Debug.LogError("Can't find UnityEditor.ProjectBrowser type!");
            return;
        }

        FieldInfo lastProjectBrowser = projectBrowserType.GetField("s_LastInteractedProjectBrowser",
            BindingFlags.Static | BindingFlags.Public);
        if (lastProjectBrowser == null)
        {
            Debug.LogError("Can't find s_LastInteractedProjectBrowser field!");
            return;
        }

        object lastProjectBrowserInstance = lastProjectBrowser.GetValue(null);
        FieldInfo projectBrowserViewMode =
            projectBrowserType.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
        if (projectBrowserViewMode == null)
        {
            Debug.LogError("Can't find m_ViewMode field!");
            return;
        }

        // 0 - one column, 1 - two column
        int viewMode = (int)projectBrowserViewMode.GetValue(lastProjectBrowserInstance);
        if (viewMode != 1)
        {
            return;
        }

        MethodInfo showSelectedObjects = projectBrowserType.GetMethod(
            "ShowSelectedObjectsInLastInteractedProjectBrowser", BindingFlags.NonPublic | BindingFlags.Static);
        if (showSelectedObjects != null)
        {
            showSelectedObjects.Invoke(null, null);
        }
        else
        {
            Debug.LogError("Can't find ShowSelectedObjectsInLastInteractedProjectBrowser method!");
        }
    }
}
