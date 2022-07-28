using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Actor持有者
[DisallowMultipleComponent]
public class ActorHolder : MonoBehaviour, IDisposer {
    // 中介者
    public Actor mediator { get; private set; }
    // public ValueAssigner<bool> use = new ValueAssigner<bool>(true);

    public virtual void Init(Actor mediator) {
        this.mediator = mediator;
        // this.use.value = true;

        this.OnPreInit();
        this.OnPostInit();
    }

    protected virtual void OnPreInit() { }
    protected virtual void OnPostInit() { }
}
