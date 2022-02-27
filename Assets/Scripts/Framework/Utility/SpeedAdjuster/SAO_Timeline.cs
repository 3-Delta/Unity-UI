using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SAO_Timeline : SpeedAdjustObserver {
    public PlayableDirector playableDirector;

    protected override void ParseComponent() {
        playableDirector = GetComponent<PlayableDirector>();
    }
    
    protected override void Restore() {
        originalSpeed = (float)playableDirector.playableGraph.GetRootPlayable(0).GetSpeed();
    }
    protected override void Recovery() {
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed((double)originalSpeed);
    }
    
    protected override void DoAdjust() {
        float speed = originalSpeed * speedMultiple;
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(speed);
    }
}
