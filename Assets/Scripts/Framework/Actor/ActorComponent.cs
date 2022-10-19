using UnityEngine;

// Actor持有者
[DisallowMultipleComponent]
public abstract class ActorComponent<TActor> : MonoBehaviour, IDisposer where TActor : Actor {
    // 中介者
    public TActor mediator;
    // public ValueAssigner<bool> use = new ValueAssigner<bool>(true);

    public virtual void Init(TActor mediator) {
        this.mediator = mediator;
        // this.use.value = true;

        this.OnPreInit();
        this.OnPostInit();
    }

    protected virtual void OnPreInit() { }
    protected virtual void OnPostInit() { }
}
