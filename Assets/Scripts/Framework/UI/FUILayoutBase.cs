using System;
using UnityEngine;

[Serializable]
public abstract class FUILayoutBase {
    public Transform transform { get; protected set; }

    public GameObject gameObject {
        get {
            if (transform != null) {
                return transform.gameObject;
            }

            return null;
        }
    }

    public void TryBind(Transform transform) {
        this.transform = transform;
    }
}

// 用于实现空对象模式
[Serializable]
public class FEmptyUILayout : FUILayoutBase { }
