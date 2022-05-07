using System;
using UnityEngine;

// 鼠标在屏幕上滑动，target跟随鼠标转向
// 绕y轴旋转
// ref: https://github.com/3-Delta/Buff-In-TopDownShooter /PlayerController.cs和UnitRotate.cs
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class RotateFollowScreenInput : MonoBehaviour {
    public Camera posConverter;
    public Transform target;

    private void Start() {
        if (target == null) {
            target = transform;
        }
    }

    private void GetTouchPos(out Vector2 cursorPos) {
        cursorPos = default;
# if UNITY_EDITOR|| UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        cursorPos = Input.mousePosition;
#else
        if (Input.touchCount > 0) {
            cursorPos = Input.GetTouch(0).position;
        }
#endif
    }

    private Vector2 cursorPos;

    private void Update() {
        GetTouchPos(out cursorPos);
    }

    private void FixedUpdate() {
        if (posConverter && this.target) {
            // 先获得target的屏幕坐标，然后对比鼠标坐标就知道转向了
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(posConverter, transform.position);
            float rotateToAngle = Mathf.Atan2(cursorPos.x - screenPos.x, cursorPos.y - screenPos.y) * 180.00f / Mathf.PI;

            // target.Rotate(new Vector3(0, rotateToAngle, 0));
        }
    }
}
