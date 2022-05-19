using UnityEngine;

public enum EActorType {
    Player,
    Npc,
}

[DisallowMultipleComponent]
public abstract class Actor : MonoBehaviour, IDisposer {
    public ActorData data { get; protected set; }

    public abstract void GetData(ulong guid);

    public virtual void ResetCsv(uint csvId) {
        this.data.Reset(csvId);
    }
}
