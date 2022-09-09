using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESceneNode {
    ActorTotal, // player/npc/npc总节点
    Player, // player节点
    Npc, // npc节点
    Bullet, // 子弹节点

    SDK, // sdk节点
    App, // app入口节点

    EventSystem, // 事件系统节点
    UI, // ui节点
    Audio, // 音效节点
    Timeline, // timeline节点
    ScreenTouch, // 挂载<ScreenTouch>的节点
    PlayerInput, // 键鼠/touch输入
    SafeAreaAdjuster, // 安全区调整器
}

[DisallowMultipleComponent]
public class SceneNodeAssigner : MonoBehaviour {
    public enum ERegisterType {
        AwakeOnDestroy,
        EnableDisable,
    }

    public ERegisterType registerType = ERegisterType.AwakeOnDestroy;
    public ESceneNode nodeType = ESceneNode.ActorTotal;

    private void Awake() {
        if (registerType == ERegisterType.AwakeOnDestroy) {
            SceneNodeService.Replace(nodeType, transform);
        }
    }

    private void OnEnable() {
        if (registerType == ERegisterType.EnableDisable) {
            SceneNodeService.Replace(nodeType, transform);
        }
    }

    private void OnDisable() {
        if (registerType == ERegisterType.EnableDisable) {
            SceneNodeService.Replace(nodeType, null);
        }
    }

    private void OnDestroy() {
        if (registerType == ERegisterType.AwakeOnDestroy) {
            SceneNodeService.Replace(nodeType, null);
        }
    }
}
