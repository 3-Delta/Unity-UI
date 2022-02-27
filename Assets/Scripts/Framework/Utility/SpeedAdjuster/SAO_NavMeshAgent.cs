using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SAO_NavMeshAgent : SpeedAdjustObserver {
    public NavMeshAgent navMeshAgent;
    
    private float originalAngularSpeed = 1f;
    
    protected override void ParseComponent() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    protected override void Restore() {
        originalSpeed = navMeshAgent.speed;
        originalAngularSpeed = navMeshAgent.angularSpeed;
    }
    protected override void Recovery() {
        navMeshAgent.speed = originalSpeed;
        navMeshAgent.angularSpeed = originalAngularSpeed;
    }
    
    protected override void DoAdjust() {
        navMeshAgent.speed = originalSpeed * speedMultiple;
        navMeshAgent.angularSpeed = originalAngularSpeed * speedMultiple;
    }
}
