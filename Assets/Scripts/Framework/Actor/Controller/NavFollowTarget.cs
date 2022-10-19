using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// 组队的时候，队员跟随组件
[DisallowMultipleComponent]
public class NavFollowTarget : MonoBehaviour {
    public NavMeshAgent target;

    [FormerlySerializedAs("selfAgent")] public NavMeshAgent selfAgent;
    // 距离target多远停止
    public float speed = 1f;
    public float stopDistance = 0.5f;
    public Action<Vector3> onReached;

    public bool HasTarget {
        get { return this.target != null; }
    }
    
    public bool IsTargetMoving {
        get { return this.target != null && this.target.enabled; }
    }

    public bool HasReachedTarget {
        get {
            if (HasTarget) {
                var diff = this.target.transform.position - this.selfAgent.transform.position;
                return diff.sqrMagnitude <= this.stopDistance * this.stopDistance;
            }

            return true;
        }
    }
    
    public NavFollowTarget SetTarget(NavMeshAgent target, float speed = 1f,  Action<Vector3> onReached = null, float stopDistance = 0.5f) {
        this.target = target;
        this.onReached = onReached;
        this.speed = speed;
        this.stopDistance = stopDistance;

        this.selfAgent.speed = speed;
        return this;
    }

    public NavFollowTarget RemoveTarget() {
        this.target = null;
        return this;
    }
    
    [ContextMenu(nameof(Stop))]
    private void Stop() {
        if (!this.selfAgent.enabled) {
            return;
        }

        this.onReached?.Invoke(this.selfAgent.transform.position);

        this.selfAgent.isStopped = true;
        this.selfAgent.enabled = false;
    }

    private void Update() {
        if (this.IsTargetMoving) {
            var posOffset = this.target.transform.position - this.selfAgent.transform.position;
            var dir = posOffset.normalized;
            // 调整朝向
            this.selfAgent.transform.forward = dir;
            
            if (this.HasReachedTarget) {
                this.Stop();
            }
            else {
                var v = dir * speed;
                this.selfAgent.nextPosition = this.selfAgent.transform.position + Time.deltaTime * v;
            }
        }
    }
}
