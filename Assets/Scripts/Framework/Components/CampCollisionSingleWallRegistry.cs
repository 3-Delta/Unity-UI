using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CampCollisionSingleWallRegistry : MonoBehaviour {
    private Dictionary<int, List<CampCollisionSingleWall>> walls = new Dictionary<int, List<CampCollisionSingleWall>>();

    public static CampCollisionSingleWallRegistry Instance { get; private set; }

    public void Register(CampCollisionSingleWall wall) {
        if (!walls.TryGetValue(wall.campId, out var ls)) {
            ls = new List<CampCollisionSingleWall>();
            walls.Add(wall.campId, ls);
        }

        if (!ls.Contains(wall)) {
            ls.Add(wall);
        }
    }

    public void UnRegister(CampCollisionSingleWall wall) {
        if (walls.TryGetValue(wall.campId, out var ls)) {
            ls.Remove(wall);
        }
    }

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

#if UNITY_EDITOR
    public int campId = 1;
    public bool canCrossWall = false;

    [ContextMenu(nameof(Set))]
    private void Set() {
        Set(campId, canCrossWall);
    }
#endif

    public void Set(int campId, bool canCrossWall) {
        foreach (var kvp in walls) {
            bool isSelfCamp = kvp.Key == campId;
            foreach (var one in kvp.Value) {
                one.Set(isSelfCamp, canCrossWall);
            }
        }
    }
}
