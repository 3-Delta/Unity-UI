using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum ESceneNode {
    ActorTotalNode, // player/npc/npc总节点
    PlayerNode, // player节点
    NpcNode, // npc节点
    BulletNode, // 子弹节点
    
    SDKNode, // sdk节点
    AppNode, // app入口节点
    
    EventSystemNode, // 事件系统节点
    UINode, // ui节点
    AudioNode, // 音效节点
    TimelineNode, // timeline节点
    ScreenTouchNode, // 挂载<ScreenTouch>的节点
}

[DisallowMultipleComponent]
public class SceneNodeAssigner : MonoBehaviour {
    public ESceneNode nodeType = ESceneNode.ActorTotalNode;
    
    private void OnEnable() {
        SceneNodeService.Replace(nodeType, transform);
    }

    private void OnDisable() {
        SceneNodeService.Replace(nodeType, null);
    }
}
