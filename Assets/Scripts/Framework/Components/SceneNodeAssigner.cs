using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum ESceneNode {
    ActorNode,
    PlayerNode,
    NpcNode,
    BulletNode,
    
    EventSystemNode,
    AudioNode,
}

[DisallowMultipleComponent]
public class SceneNodeAssigner : MonoBehaviour {
    public ESceneNode nodeType = ESceneNode.ActorNode;
    
    private void OnEnable() {
        SceneNodeService.Replace(nodeType, transform);
    }

    private void OnDisable() {
        SceneNodeService.Replace(nodeType, null);
    }
}
