using System;
using UnityEngine;

[DisallowMultipleComponent]
[Serializable]
public class PlayerActor : HumanoidActor {
    [SerializeField] private PlayerInput _playerInput;

    public PlayerInput playerInput {
        get {
            if (!_playerInput) {
                SceneNodeService.TryGet(ESceneNode.PlayerInput, out _playerInput);
            }

            return _playerInput;
        }
    }
}
