using UnityEngine;

public enum EActorType {
    Player,
    Npc,
}

[DisallowMultipleComponent]
public abstract class Actor : MonoBehaviour, IDisposer {
    public ActorData actorData { get; protected set; }
    
    public virtual void ResetCsv(uint csvId) {
        this.actorData.Reset(csvId);
    }
}
