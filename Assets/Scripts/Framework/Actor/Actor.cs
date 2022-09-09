using UnityEngine;

public enum EActorType {
    Player,
    Npc,
}

[DisallowMultipleComponent]
public abstract class Actor : MonoBehaviour, IDisposer
{
    public ulong guid;

    [SerializeField] private PlayerInput _playerInput;

    public PlayerInput playerInput {
        get {
            if (!_playerInput) {
                SceneNodeService.TryGet(ESceneNode.PlayerInput, out _playerInput);
            }

            return _playerInput;
        }
    }
    
    public PathFinder pathFinder;
    public AnimChanger animChanger;
    public ModelChanger modelChanger;
    public StatusChanger statusChanger;
    public VisibleChanger visibleChanger;

    // 根据guid查找actorData
    public virtual ActorData actorData { get; }
}
