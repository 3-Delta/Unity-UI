using System;
using UnityEngine;
using UnityEngine.AI;

// 寻路到下一个位置
[DisallowMultipleComponent]
public class NavFollowTargetPos : MonoBehaviour {
    public NavMeshAgent agent;

    public Action onMoveToTarget;
    public Action<bool> onStop;
    
    public bool MoveToTargetPos(Vector3 destPos, Action onBegin) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            agent.enabled = true;
            
            onBegin?.Invoke();
            return agent.SetDestination(hit.position);
        }

        return false;
    }

    public bool MoveToNextPos(Vector2 dir, float distance) {
        return true;
    }

    public bool MoveToNextPos(Vector3 destPos) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            agent.enabled = true;
            agent.SetDestination(hit.position);
            return true;
        }

        return false;
    }

    // 直接传送到某个位置，切换地图的时候调用
    public bool Warp(Vector3 destPos) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            agent.enabled = true;
            
            return agent.Warp(hit.position);
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
            onMoveToTarget?.Invoke();
        }

        agent.isStopped = true;
        agent.enabled = false;
    }
}
