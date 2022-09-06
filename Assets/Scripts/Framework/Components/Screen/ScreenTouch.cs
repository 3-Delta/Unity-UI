using System;
using UnityEngine;

// 长时间不触屏，会进行节能操作
// 比如：阴影，renderscale，分辨率,亮度，声音，帧率，角色数量，
[DisallowMultipleComponent]
public class ScreenTouch : MonoBehaviour {
    public Action<Vector2> onTouchScreen;

    private void Update() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetMouseButtonDown(0)) {
            this.onTouchScreen?.Invoke(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0) {
            this.onTouchScreen?.Invoke(Input.GetTouch(0).position);
        }
#endif
    }
}
