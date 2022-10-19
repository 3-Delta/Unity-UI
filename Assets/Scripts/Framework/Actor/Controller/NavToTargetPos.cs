using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// 寻路到目标位置
[DisallowMultipleComponent]
public class NavToTargetPos : MonoBehaviour {
    [FormerlySerializedAs("agent")] public NavMeshAgent selfAgent;

    public Vector3 targetPosition;
    public float speed;
    
    public Action<Vector3> onReached;
    public Action<Vector3> onInterrupted;

    public bool HasReachedTarget {
        get {
            // http://t.zoukankan.com/sword-magical-blog-p-9665297.html
            // unity源码就是：remainingDistance < dis
            var dis = Mathf.Max(0.001f, this.selfAgent.stoppingDistance);
            return !this.selfAgent.pathPending && this.selfAgent.remainingDistance <= dis;
        }
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(MoveTo))]
    public bool MoveTo() {
        return MoveTo(targetPosition);
    }
#endif

    public bool MoveTo(Vector3 destPos, float speed = 1f, Action onBegin = null, Action<Vector3> onReach = null, Action<Vector3> onInterrupt = null) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            this.selfAgent.enabled = true;
            this.onReached = onReach;
            this.speed = speed;
            this.onInterrupted = onInterrupt;
            
            this.selfAgent.speed = speed;

            onBegin?.Invoke();
            return this.selfAgent.SetDestination(hit.position);
        }

        return false;
    }

    [ContextMenu(nameof(Stop))]
    public void Stop(bool manualStop = false) {
        if (!this.selfAgent.enabled) {
            return;
        }

        bool reached = this.HasReachedTarget;
        if (!manualStop || reached) {
            this.onReached?.Invoke(this.selfAgent.transform.position);
        }
        else {
            this.onInterrupted?.Invoke(this.selfAgent.transform.position);
        }

        this.selfAgent.isStopped = true;
        this.selfAgent.enabled = false;
    }

    private void Update() {
        if (this.selfAgent.enabled) {
            if (this.HasReachedTarget) {
                Stop(false);
            }
        }
    }
}
