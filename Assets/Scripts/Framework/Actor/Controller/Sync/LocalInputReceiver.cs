using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

// 接受键鼠/遥感的输入
// 将来从某个统一的地方接收输入，便于统一管理输入的开启,关闭
// npc/player等挂载
// ref https://code.aliyun.com/kaclok/UnityPhysics.git
[DisallowMultipleComponent]
public class LocalInputReceiver : ActorController {
    public bool blockClick = false;
    public bool blockCtrl = false;
    
    protected void OnEnable() {
        if (!blockClick) {
            mediator.playerInput.onClick += this._OnClick;
        }

        if (!blockCtrl) {
            mediator.playerInput.onCtrl += this._OnCtrl;
        }
    }

    protected void OnDisable() {
        mediator.playerInput.onClick -= this._OnClick;
        mediator.playerInput.onCtrl -= this._OnCtrl;
    }

    private void _OnClick(Vector2 touchPosition, PointerEventData eventData) {
        if (!blockClick) {
            _RaycastHit(touchPosition);
        }
    }

    // 人物移动，设计，跳跃
    private void _OnCtrl(OpInput input) {
        if (!blockCtrl) {
            // 移动
        }
    }

    private RaycastHit[] _hits = new RaycastHit[5];
    private void _RaycastHit(Vector2 touchPosition, float maxDistance = 1000f) {
        var ray = CameraService.CurrentCamera3d.ScreenPointToRay(touchPosition);
        int length = Physics.RaycastNonAlloc(ray, this._hits, maxDistance, Physics.AllLayers);
        for (int i = 0; i < length; ++i) {
            mediator.pathFinder.MoveToTarget(_hits[i].point, null);
        }
    }
}
