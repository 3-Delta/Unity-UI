using System;
using ILRuntime.CLR.Utils;
using UnityEngine;

// 在红点需要出现的节点上挂载
[DisallowMultipleComponent]
public class RedDotIndexer : MonoBehaviour {
    public uint id;
    // public Func<bool> CanShow;
    // public Action<RedDotIndexer> ShowAction;
    //
    // public void Set() {
    //     
    // }
    //
    // private void TryLoad() {
    //     
    // }
    //
    // public void Listen(bool toListen) {
    //     
    // }

    private RedDotCollector _collector;

    public RedDotCollector Collector {
        get {
            if (this._collector == null) {
                this._collector = this.GetComponentInParent<RedDotCollector>();
            }

            return this._collector;
        }
    }

    private void OnEnable() {
        if (this.Collector != null) {
            this.Collector.Register(this);
        }
    }

    private void OnDisable() {
        if (this.Collector != null) {
            this.Collector.UnRegister(this);
        }
    }
}
