using System;
using UnityEngine;
using UnityEngine.AI;

// 寻路到目标位置
[DisallowMultipleComponent]
public class NavToTarget : MonoBehaviour {
    public NavMeshAgent agent;

    public Vector3 targetPosition;
    public Action onMoveTo;
    public Action<bool> onStop;
    
    public bool ReachedTarget {
        get {
            // http://t.zoukankan.com/sword-magical-blog-p-9665297.html
            // unity源码就是：remainingDistance < dis
            var dis = Mathf.Max(0.001f, this.agent.stoppingDistance);
            return !this.agent.pathPending && this.agent.remainingDistance <= dis;
        }
    }

    public bool MoveTo(Vector3 destPos, Action onBegin = null, Action onMoveTo = null, Action<bool> onStop = null) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            this.agent.enabled = true;
            this.onMoveTo = onMoveTo;
            this.onStop = onStop;
            
            onBegin?.Invoke();
            return agent.SetDestination(hit.position);
        }

        return false;
    }
    
    public void Stop(bool interrupt = false) {
        if (!agent.enabled || agent.isStopped) {
            return;
        }
        
        onStop?.Invoke(interrupt);
        if (!interrupt) {
            this.onMoveTo?.Invoke();
        }

        agent.isStopped = true;
        agent.enabled = false;
    }

    private void Update() {
        if (agent.enabled) {
            if (this.ReachedTarget) {
                Stop(false);
            }
        }
    }
}
