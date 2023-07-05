using System;
using UnityEngine;

// https://blog.csdn.net/weixin_61738543/article/details/126529650
[DisallowMultipleComponent]
public class DoubleFingerScale : MonoBehaviour {
    public float scaleSpeed = 1f;

    public bool isLimitScale = true;
    public Vector2 minMax = new Vector2(0.8f, 3f);
    public Transform target = null;

    private bool hasInited;
    private float touchDistance;

#if UNITY_EDITOR
[SerializeField]
#endif
    private float scale;
    
    private float baseScale = 1f;
    public Action<Transform, float> onScale;

    private void Start() {
        if (this.target == null) {
            this.target = this.transform;
        }
    }

    private void Update() {
        // 不是双指就关闭
        if (Input.touchCount != 2) {
            this.hasInited = false;
        }

        // 初始化双指逻辑, 也就是第一次双指同时按下的时机
        if (Input.touchCount == 2 && !this.hasInited) {
            // 记录两指点位
            var curTouch1 = Input.GetTouch(0).position;
            var curTouch2 = Input.GetTouch(1).position;
            this.touchDistance = Vector2.Distance(curTouch1, curTouch2);
            this.touchDistance = this.touchDistance <= 0 ? 1f : this.touchDistance;

            this.baseScale = this.target.localScale.x;
            this.hasInited = true;
        }

        // 正常双指逻辑
        if (Input.touchCount == 2) {
            var curTouch1 = Input.GetTouch(0).position;
            var curTouch2 = Input.GetTouch(1).position;
            float curDistance = Vector2.Distance(curTouch1, curTouch2);

            // 两指缩放比例
            this.scale = curDistance / this.touchDistance;
            // 利用scaleSpeed控制缩放速度
            this.scale *= (this.scaleSpeed * this.baseScale);

            // 给缩放比例加限制
            if (isLimitScale) {
                if (scale <= this.minMax.x) {
                    scale = this.minMax.x;
                }
                else if (scale >= this.minMax.y) {
                    scale = this.minMax.y;
                }
            }

            // 缩放目标大小
            onScale?.Invoke(target, scale);

            target.localScale = new Vector3(scale, scale, scale);
        }
    }
}
