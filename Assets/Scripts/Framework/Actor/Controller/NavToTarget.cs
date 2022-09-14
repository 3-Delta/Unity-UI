using System;
using UnityEngine;
using UnityEngine.AI;

// 寻路到目标位置
[DisallowMultipleComponent]
public class NavToTarget : MonoBehaviour {
    public NavMeshAgent agent;

    public Vector3 targetPosition;
    public Action<Vector3> onReach;
    public Action<Vector3> onInterrupt;

    public bool HasReachedTarget {
        get {
            // http://t.zoukankan.com/sword-magical-blog-p-9665297.html
            // unity源码就是：remainingDistance < dis
            var dis = Mathf.Max(0.001f, this.agent.stoppingDistance);
            return !this.agent.pathPending && this.agent.remainingDistance <= dis;
        }
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(MoveTo))]
    public bool MoveTo() {
        MoveTo(targetPosition);
    }
#endif

    public bool MoveTo(Vector3 destPos, Action onBegin = null, Action<Vector3> onReach = null, Action<Vector3> onInterrupt = null) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            this.agent.enabled = true;
            this.onReach = onReach;
            this.onInterrupt = onInterrupt;

            onBegin?.Invoke();
            return agent.SetDestination(hit.position);
        }

        return false;
    }

    [ContextMenu(nameof(Stop))]
    public void Stop(bool interrupt = false) {
        if (!agent.enabled || agent.isStopped) {
            return;
        }

        if (!interrupt) {
            this.onReach?.Invoke(this.agent.transform.position);
        }
        else {
            this.onInterrupt?.Invoke(this.agent.transform.position);
        }

        agent.isStopped = true;
        agent.enabled = false;
    }

    private void Update() {
        if (agent.enabled) {
            if (this.HasReachedTarget) {
                Stop(false);
            }
        }
    }
}
