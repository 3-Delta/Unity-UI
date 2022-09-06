using System;
using UnityEngine;

// 输入
[DisallowMultipleComponent]
public abstract class MoveInput : MonoBehaviour {
    public virtual void GatherInput(ref OpInput input) {
    }
}
