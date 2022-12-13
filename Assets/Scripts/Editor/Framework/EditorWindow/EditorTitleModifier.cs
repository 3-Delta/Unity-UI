using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// https://github.com/mob-sakai/MainWindowTitleModifierForUnity
// https://github.com/XINCGer/UnityToolchainsTrick/tree/main/Assets/Editor/Examples/Example_20_TitleModifier
#if UNITY_EDITOR
[InitializeOnLoad]
public class EditorTitleModifier {
    // for .NET Standard 2.0
    /*
    static EditorTitleModifier() {
        EditorApplication.updateMainWindowTitle -= ChangeApplicationTitle;
        EditorApplication.updateMainWindowTitle += ChangeApplicationTitle;
        // EditorApplication.UpdateMainWindowTitle();
    }
    
    static void ChangeApplicationTitle(ApplicationTitleDescriptor title) {
        title.title = $"{title.title} --> {Application.dataPath}";
    }
    */

    // for .NET 4.x
    static EditorTitleModifier() {
        Type editor = typeof(EditorApplication);
        EventInfo updateTitle = editor.GetEvent("updateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);

        Type titleDescriptor = editor.Assembly.GetTypes()
            .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor");
        Type delegateType = typeof(Action<>).MakeGenericType(titleDescriptor);
        MethodInfo methodInfo = ((Action<object>)UpdateWindowTitle).Method;
        Delegate del = Delegate.CreateDelegate(delegateType, null, methodInfo);

        var args = new object[] {
            del
        };
        updateTitle.GetRemoveMethod(true).Invoke(null, args);
        updateTitle.GetAddMethod(true).Invoke(null, args);
    }

    static void UpdateWindowTitle(object titleDesc) {
        var fieldInfo = typeof(EditorApplication).Assembly.GetTypes()
            .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor")
            .GetField("title", BindingFlags.Instance | BindingFlags.Public);
        
        var str = fieldInfo.GetValue(titleDesc) as string;
        fieldInfo.SetValue(titleDesc, $"{str} --> {Application.dataPath}");
    }
}
#endif