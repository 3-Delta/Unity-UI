using System.Collections.Generic;
using UnityEngine;

// 由server下发的指令驱动，
// 可能是广播的别人的指令
// 可能是AI指令
[DisallowMultipleComponent]
public class RemoteOpReceiver : MonoBehaviour {
    [SerializeField] public Queue<OpCmd> opQueue = new Queue<OpCmd>(0);

    public virtual void GatherOp(ref OpInput input) { }
}
