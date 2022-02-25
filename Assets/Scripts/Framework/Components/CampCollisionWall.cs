using System;
using System.Collections.Generic;
using UnityEngine;

// 目前只支持两个阵营
[DisallowMultipleComponent]
public class CampCollisionWall : MonoBehaviour {
    // 绝对概念的阵营
    public enum ECampType {
        Left,
        Right
    }

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

    // 阵营阻挡墙体可能不止一个
    public List<Wall> leftWalls = new List<Wall>();
    public List<Wall> rightWalls = new List<Wall>();

    public static CampCollisionWall Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        Instance = null;
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(Set))]
    private void Set() {
        Set(targetCamp, canCrossWall);
    }

    public ECampType targetCamp = ECampType.Left;
    public bool canCrossWall = false;
#endif

    public void Set(ECampType targetCamp, bool canCrossWall) {
        void Block(List<Wall> walls, bool toActive) {
            foreach (var one in walls) {
                one.ActiveRed(toActive);
            }
        }

        bool isLeftCamp = targetCamp == ECampType.Left;
        if (isLeftCamp) {
            Block(rightWalls, false);
            if (canCrossWall) {
                Block(leftWalls, true);
            }
            else {
                Block(leftWalls, false);
            }
        }
        else {
            Block(leftWalls, false);
            if (canCrossWall) {
                Block(rightWalls, true);
            }
            else {
                Block(rightWalls, false);
            }
        }
    }
}
