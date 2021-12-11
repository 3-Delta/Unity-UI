using System;
using UnityEngine;

// UI动画的开始结束
[DisallowMultipleComponent]
public class UIAnimationTrigger : MonoBehaviour {
    public Action<string> onBegin;
    public Action<string> onEnd;

    // 动画开始的时候调用
    private void OnBegin(string arg) {
        onBegin?.Invoke(arg);
    }

    // 动画结束的时候调用
    private void OnEnd(string arg) {
        onEnd?.Invoke(arg);
    }

    private void OnDestroy() {
        onBegin = null;
        onEnd = null;
    }
}
