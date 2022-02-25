using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CampCollisionSingleWall : MonoBehaviour {
    [Serializable]
    public class Wall {
        // 透明墙体，无阻挡
        public Transform red;

        // 真实墙体，有Collider阻挡
        public Transform blue;

        public void ActiveRed(bool toActive) {
            red.gameObject.SetActive(toActive);
            blue.gameObject.SetActive(!toActive);
        }
    }

    public int campId = 0;

    // 阵营阻挡墙体可能不止一个
    public List<Wall> walls = new List<Wall>();

    private void OnEnable() {
        var inst = CampCollisionSingleWallRegistry.Instance;
        if (inst != null) {
            inst.Register(this);
        }
    }

    private void OnDisable() {
        var inst = CampCollisionSingleWallRegistry.Instance;
        if (inst != null) {
            inst.UnRegister(this);
        }
    }

    public void Set(bool isSelfCamp, bool canCrossWall) {
        void Block(List<Wall> walls, bool toActive) {
            foreach (var one in walls) {
                one.ActiveRed(toActive);
            }
        }

        if (isSelfCamp) {
            Block(walls, canCrossWall);
        }
        else {
            Block(walls, false);
        }
    }
}
