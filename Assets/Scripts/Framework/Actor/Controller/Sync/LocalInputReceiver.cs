using System;
using UnityEngine;

// 接受键鼠/遥感的输入
// 将来从某个统一的地方接收输入，便于统一管理输入的开启,关闭
// 监听InputMgr的各种action
// https://code.aliyun.com/kaclok/UnityPhysics.git
[DisallowMultipleComponent]
public class LocalInputReceiver : MonoBehaviour {
    protected void OnEnable() {
        PlayerInput.instance.onCtrl += OnCtrl;
    }

    protected void OnDisable() {
        PlayerInput.instance.onCtrl -= OnCtrl;
    }
    
    private void OnCtrl(OpInput input) {
    }

    private RaycastHit[] hits;
    private void RaycastHit(Vector2 touchPosition) {
        var ray = CameraService.CurrentCamera3d.ScreenPointToRay(touchPosition);
        int hitCount = Physics.RaycastNonAlloc(ray, hits);
        if (hitCount > 0) {
            
        }
    }
}
