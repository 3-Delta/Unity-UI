using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SAO_TimeScale : SpeedAdjustObserver {
    protected override void Restore() {
        originalSpeed = Time.timeScale;
    }
    protected override void Recovery() {
        Time.timeScale = originalSpeed;
    }
    protected override void DoAdjust() {
        Time.timeScale = speedMultiple;
    }
}
