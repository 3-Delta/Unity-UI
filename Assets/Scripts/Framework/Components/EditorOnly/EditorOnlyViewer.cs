#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// https://www.xuanyusong.com/archives/4384
[DisallowMultipleComponent]
public class EditorOnlyViewer : MonoBehaviour {
    [SerializeField] private string targetTag = "EditorOnly";

    // 本脚本只在Editor下使用
    private void Reset() {
        this.hideFlags = HideFlags.DontSaveInBuild;
    }

    // 显示特定Tag的所有物件
    // 选中并且Gizmos开启才会执行
    private void OnDrawGizmosSelected() {
        // 查找UnTagged始终为空集合
        var ls = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject go in ls) {
            Handles.Label(go.transform.position, targetTag);
        }
    }
}
#endif
