using UnityEngine;

public enum EActorType {
    Player,
    Npc,
}

[DisallowMultipleComponent]
public abstract class Actor : MonoBehaviour, IDisposer
{
    public ulong guid;
    
    public AnimChanger animChanger;
    public ModelChanger modelChanger;
    public StatusChanger statusChanger;
    public VisibleChanger visibleChanger;

    // 根据guid查找actorData
    public virtual ActorData actorData { get; }
}
