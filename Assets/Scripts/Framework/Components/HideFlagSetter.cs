using UnityEngine;

[DisallowMultipleComponent]
public class HideFlagSetter : MonoBehaviour {
#if UNITY_EDITOR
    public Component target;
    [FlagsEnumProperty] public HideFlags flags = HideFlags.None;

    private void Reset() {
        // 当前脚本不构建
        this.hideFlags = HideFlags.DontSaveInBuild;
    }

    [ContextMenu("显示")]
    private void Inspect() {
        // 显示目标target的hideFlags
        if (this.target != null) {
            this.flags = this.target.hideFlags;
        }
    }

    [ContextMenu("设置")]
    private void Set() {
        // 设置目标target的hideFlags
        if (this.target != null) {
            this.target.hideFlags = this.flags;
        }
    }
#endif
}
