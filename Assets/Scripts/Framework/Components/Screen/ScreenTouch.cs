using System;
using UnityEngine;

// 长时间不触屏，会进行节能操作
// 比如：阴影，renderscale，分辨率,亮度，声音，帧率，角色数量，
[DisallowMultipleComponent]
public class ScreenTouch : MonoBehaviour {
    public Action onTouchScreen;

    private void Update() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetMouseButtonDown(0)) {
            this.onTouchScreen?.Invoke();
        }
#else
        if (Input.touchCount > 0) {
            this.onTouchScreen?.Invoke();
        }
#endif
    }
}
