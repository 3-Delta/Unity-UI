#if UNITY_EDITOR
using UnityEditor;

// 模仿：GameFramework
public abstract class BaseInspector : Editor {
    private bool isCompiling = false;

    /// <summary>
    /// 绘制事件。
    /// </summary>
    public override void OnInspectorGUI() {
        if (isCompiling && !EditorApplication.isCompiling) {
            isCompiling = false;
            OnCompileComplete();
        }
        else if (!isCompiling && EditorApplication.isCompiling) {
            isCompiling = true;
            OnCompileStart();
        }
    }

    // 编译开始事件。
    protected virtual void OnCompileStart() { }

    // 编译完成事件。
    protected virtual void OnCompileComplete() { }

    protected bool IsPrefabInHierarchy(UnityEngine.Object target) {
        if (target == null) {
            return false;
        }

#if UNITY_2018_3_OR_NEWER
        return PrefabUtility.GetPrefabAssetType(target) != PrefabAssetType.Regular;
#else
        return PrefabUtility.GetPrefabType(target) != PrefabType.Prefab;
#endif
    }
}
#endif
