using System;
using UnityEngine;

public enum EActorType {
    Player,
    Npc,
}

[DisallowMultipleComponent]
public abstract class Actor : MonoBehaviour, IDisposer {
    public ulong guid;
    // 根据guid查找actorData
    public virtual ActorData actorData { get; }

    public ActorMountNode hierichy;

    public Transform GetNode(EActorMountNode node) {
        return this.hierichy.Get(node);
    }
    
    public T GetNode<T>(EActorMountNode node) where T : Component {
        return this.hierichy.Get(node).GetComponent<T>();
    }
}

[DisallowMultipleComponent]
public abstract class HumanoidActor : Actor {
#region 壳上的mono
    public ModelChanger modelChanger;
    public StatusChanger statusChanger;
    public VisibleChanger visibleChanger;
    public PathFinder pathFinder;
#endregion

#region 具体player上的mono
    public AnimChanger animChanger;
#endregion

}
