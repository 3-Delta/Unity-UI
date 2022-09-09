using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public class MethodInvoker<T> : MonoBehaviour where T : class {
    public T instance;
    public string methodName;

    [ContextMenu(nameof(InvokeMethod))]
    public virtual void InvokeMethod() {
        if (this.instance != null) {
            var m = GetMethod(this.methodName);
            m?.Invoke(this.instance, null);
        }
    }

    protected MethodInfo GetMethod(string name) {
        var m = typeof(T).GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        return m;
    }
}
