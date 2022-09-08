using System;
using UnityEngine;
using UnityEngine.AI;

// 寻路到目标位置
[DisallowMultipleComponent]
public class NavToTarget : MonoBehaviour {
    public NavMeshAgent agent;

    public Action onMoveTo;
    public Action<bool> onStop;

    public bool ReachedTarget {
        get {
            return true;
        }
    }

    public bool MoveTo(Vector3 destPos, Action onBegin) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            agent.enabled = true;
            
            onBegin?.Invoke();
            return agent.SetDestination(hit.position);
        }

        return false;
    }
    
    public void Stop(bool reachTargetPos = false) {
        if (!agent.enabled || agent.isStopped) {
            return;
        }

        bool interrupt = !reachTargetPos;
        onStop?.Invoke(interrupt);
        if (reachTargetPos) {
            this.onMoveTo?.Invoke();
        }

        agent.isStopped = true;
        agent.enabled = false;
    }

    private void Update() {
        if (agent.enabled) {
            if (this.ReachedTarget) {
                Stop(true);
            }
        }
    }
}
