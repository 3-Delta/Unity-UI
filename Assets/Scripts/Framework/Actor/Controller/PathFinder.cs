using System;
using UnityEngine;
using UnityEngine.AI;

// 负责通用的寻路逻辑
[DisallowMultipleComponent]
public class PathFinder : MonoBehaviour {
    public NavToTarget toTarget;
    public NavToNext toNext;
    public NavMeshAgent agent;

    public Action onMoveToTarget;
    public Action<bool> onStop;

    public static bool ValidatePos(Vector3 pos, out NavMeshHit hit, float maxDistance = 10f) {
        bool hasNearestPoint = NavMesh.SamplePosition(pos, out hit, maxDistance, NavMesh.AllAreas);
        return hasNearestPoint;
    }
    
    // 直接传送到某个位置，切换地图的时候调用
    public static bool Warp(NavMeshAgent agent, Vector3 destPos) {
        bool hasNearestPoint = PathFinder.ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            agent.enabled = true;
            
            return agent.Warp(hit.position);
        }

        return false;
    }

    public bool MoveToTarget(Vector3 destPos, Action onBegin) {
        bool hasNearestPoint = ValidatePos(destPos, out NavMeshHit hit);
        if (hasNearestPoint) {
            agent.enabled = true;
            
            onBegin?.Invoke();
            return agent.SetDestination(hit.position);
        }

        return false;
    }

    public bool MoveToNext(Vector2 dir, float distance) {
        return true;
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
