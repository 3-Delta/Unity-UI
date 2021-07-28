using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

// https://github.com/XINCGer/UnityToolchainsTrick/tree/main/Assets/Editor/Examples/Example_20_TitleModifier
#if UNITY_EDITOR
public class TitleModifier {
    private const int Delay = 2000;

    [InitializeOnLoadMethod]
    private static void Init() {
        ModifyTitleAsync();
    }

    private static async void ModifyTitleAsync() {
        await Task.Delay(Delay);
        ModifyTitle();
    }

    private static void ModifyTitle() {
        Type tEditorApplication = typeof(EditorApplication);
        Type tApplicationTitleDescriptor = tEditorApplication.Assembly.GetTypes()
            .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor");

        EventInfo eiUpdateMainWindowTitle =
            tEditorApplication.GetEvent("updateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo miUpdateMainWindowTitle =
            tEditorApplication.GetMethod("UpdateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);

        Type delegateType = typeof(Action<>).MakeGenericType(tApplicationTitleDescriptor);
        MethodInfo methodInfo = ((Action<object>)UpdateWindowTitle).Method;
        Delegate del = Delegate.CreateDelegate(delegateType, null, methodInfo);

        eiUpdateMainWindowTitle.GetAddMethod(true).Invoke(null, new object[] { del });
        miUpdateMainWindowTitle.Invoke(null, new object[0]);
        eiUpdateMainWindowTitle.GetRemoveMethod(true).Invoke(null, new object[] { del });
    }

    static void UpdateWindowTitle(object desc) {
        var fieldInfo = typeof(EditorApplication).Assembly.GetTypes()
            .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor")
            .GetField("title", BindingFlags.Instance | BindingFlags.Public);
        var str = fieldInfo.GetValue(desc) as string;
        fieldInfo.SetValue(desc, str + " --> " + Application.dataPath);
    }
}
#endif
