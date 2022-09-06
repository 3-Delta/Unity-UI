using System;
using UnityEngine;

// 接受键鼠/遥感的输入
// 将来从某个统一的地方接收输入，便于统一管理输入的开启,关闭
// 监听InputMgr的各种action
// https://code.aliyun.com/kaclok/UnityPhysics.git
[DisallowMultipleComponent]
public class LocalInputReceiver : MonoBehaviour {
    protected void OnEnable() {
        InputMgr.instance.onClick += OnClick;
        InputMgr.instance.onCtrl += OnCtrl;
    }

    protected void OnDisable() {
        InputMgr.instance.onCtrl -= OnCtrl;
    }

    private void OnClick(Vector2 touchPosition) {
        // 发送射线检测
    }

    private void OnCtrl() {
    }
}
