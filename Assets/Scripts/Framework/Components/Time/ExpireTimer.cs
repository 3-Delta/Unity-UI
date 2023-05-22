using System;
using UnityEngine;
using UnityEngine.Serialization;

// 需要在特定时间之前的duration 秒开始显示倒计时
// diff是： 终点时间 - Timer的启动时间
[DisallowMultipleComponent]
public class ExpireTimer : MonoBehaviour {
    public float duration;
    public float diff;
    public Transform cdNode;
    public bool useRealtime = true;

    public Action<float> onCompleted;

#if UNITY_EDITOR
    // 距离开始显示倒计时的剩余时间， 如果已经在倒计时的范围内，则始终为0
    [FormerlySerializedAs("_remainTime")] [SerializeField] private float _remainTimeOfReachToBegin;
#endif

    private MTimer _timer;

    [ContextMenu(nameof(Begin))]
    private void Begin() {
        this.Begin(this.cdNode, this.duration, this.diff, this.onCompleted);
    }

    public void Begin(Transform cdNode, float duration, float diff, Action<float> onCompleted) {
        this.cdNode = cdNode;
        this.duration = duration;
        this.diff = diff;
        this.onCompleted = onCompleted;

        cdNode.gameObject.SetActive(false);
        if (this.diff > 0) {
            void _DoCd(float cd) {
                cdNode.gameObject.SetActive(true);
                this.onCompleted?.Invoke(cd);
            }

            if (diff <= duration) {
                _DoCd(diff);
            }
            else {
                this._timer?.Cancel();
                this._timer = MTimer.CreateOrReuse(ref this._timer, diff - duration, () => { _DoCd(duration); }, MTimer.False,
#if UNITY_EDITOR
                    (delta) => { this._remainTimeOfReachToBegin = this._timer.GetTimeRemaining(); }
#else
                    null
#endif
                    , this.useRealtime, null);
            }
        }
    }

    public void End() {
        this._timer?.Cancel();
    }

    private void OnDestroy() {
        this._timer?.Cancel();
    }
}
