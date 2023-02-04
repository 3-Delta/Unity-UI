using System;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 模仿NGUI的剧中效果
// 中心点可以自定义
[DisallowMultipleComponent]
public class CenterOnChild : MonoBehaviour {
    // 自定义的中心点
    public Transform center;
    public ScrollView scrollRect;
    public float springStrength = 8f;
    public bool useScaleCurve = true;
    public AnimationCurve scaleCurve;
    
    public Action<Transform, int /*index*/> onCenter;
    public Transform currentCenterGo { get; private set; } = null;

    private List<Transform> _children = new List<Transform>();
    private Vector3[] _corners = new Vector3[4];

    private void OnEnable() {
        this.scrollRect.onEndDrag += _OnEndDrag;
    }
    private void OnDisable() {
        this.scrollRect.onEndDrag -= _OnEndDrag;
    }

    public unsafe Vector3 RealCenter {
        get {
            if (this.center != null) {
                return this.center.position;
            }
            else {
                // scrollRect真实的中心
                this.scrollRect.viewport.GetWorldCorners(_corners);
                Vector3 panelCenter = (_corners[2] + _corners[0]) * 0.5f;
                return panelCenter;
            }
        }
    }

    public void CollectChildren(bool needActive = true) {
        _children.Clear();
        foreach (Transform t in this.scrollRect.transform) {
            if (needActive && t.gameObject.activeInHierarchy) {
                _children.Add(t);
            }
        }
    }

    // 拖拽离手之后开始生效
    private void _OnEndDrag(PointerEventData _) {
        if (enabled) {
            this.Recenter();
        }
    }

    private void LateUpdate() {
        // 处理vd缩放等问题
        if (this.scrollRect.isMoving) {
            var center = RealCenter;
            if (this.scrollRect.horizontal) {
                foreach (var child in this._children) {
                    var dis = child.position - center;
                    var t = scaleCurve.Evaluate(dis.x);
                    child.localScale = new Vector3(t, t, 0);
                }
            }

            if (this.scrollRect.vertical) {
                foreach (var child in this._children) {
                    var dis = child.position - center;
                    var t = scaleCurve.Evaluate(dis.y);
                    child.localScale = new Vector3(t, t, 0);
                }
            }
        }
    }

    public void Recenter() {
        // Offset this value by the momentum
        // 停手之后因为有惯性，所以需要慢慢生效居中效果
        var realCenter = RealCenter;
        Vector3 s = this.scrollRect.velocity * Time.unscaledDeltaTime;
        Vector3 pickingPoint = realCenter - s;

        float min = float.MaxValue;
        Transform closest = null;
        for (int i = 1, length = _children.Count; i < length; ++i) {
            Transform t = _children[i];
            float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);

            if (sqrDist < min) {
                min = sqrDist;
                closest = t;
            }
        }

        _CenterOn(closest, realCenter);
    }

    private void _CenterOn(Transform target, Vector3 centerPos) {
        if (target != null) {
            this.currentCenterGo = target;

            Transform content = this.scrollRect.content;
            // 世界坐标转换到以content为基准的局部坐标
            var cp = content.InverseTransformPoint(target.position);
            var cc = content.InverseTransformPoint(centerPos);
            var localOffset = cp - cc;

            if (!this.scrollRect.horizontal) {
                localOffset.x = 0f;
            }

            if (!this.scrollRect.vertical) {
                localOffset.y = 0f;
            }

            localOffset.z = 0f;

            var targetPos = content.localPosition - localOffset;
            SpringPosition.Begin(content.gameObject, targetPos, springStrength, _OnFinish);
        }
        else {
            this.currentCenterGo = null;
        }
    }

    public void CenterOn(Transform target) {
        _CenterOn(target, this.RealCenter);
    }

    public void CenterOn(int targetIndex) {
        _CenterOn(this._children[targetIndex], this.RealCenter);
    }

    private void _OnFinish(GameObject target) {
        onCenter?.Invoke(target.transform, _children.IndexOf(target.transform));
    }
}
