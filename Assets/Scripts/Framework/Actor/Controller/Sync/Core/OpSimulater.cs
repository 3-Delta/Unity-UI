using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

// https://zhuanlan.zhihu.com/p/416805924
// https://zhuanlan.zhihu.com/p/50440945
// 操作指令执行器
[DisallowMultipleComponent]
public abstract class OpSimulater : MonoBehaviour {
    // 控制目标
    public Transform target;
    public RemoteInputReceiver remoteReceiver;
    public InputInvoker inputInvoker;

    private void FixedUpdate() {
        OnPreSimulate();
        OnSimulate();
        OnPostSimulate();
    }

    protected virtual OpCmd GetCmd() {
        return new OpCmd();
    }

    protected virtual void OnPreSimulate() { }

    protected virtual void OnSimulate() { }

    protected virtual void OnPostSimulate() { }
}
