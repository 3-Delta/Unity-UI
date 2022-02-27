using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SAO_Animator : SpeedAdjustObserver 
{
    public Animator animator;

    protected override void ParseComponent() {
        animator = GetComponent<Animator>();
    }
    
    protected override void Restore() {
        originalSpeed = animator.speed;
    }
    protected override void Recovery() {
        animator.speed = originalSpeed;
    }
    
    protected override void DoAdjust() {
        animator.speed = originalSpeed * speedMultiple;
    }
}
