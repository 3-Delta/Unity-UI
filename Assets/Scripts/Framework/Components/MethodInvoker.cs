using System.Reflection;
using UnityEngine;

// 其实这里是想参考Odin的[Button]特性的序列化表现，但是水平有限
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
