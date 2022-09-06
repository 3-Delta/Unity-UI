using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 触摸屏输入
// 依赖RayCaster
// 需要在主界面UI的层级下面
[DisallowMultipleComponent]
public class TouchRaycasterMoveInput : MoveInput, IDragHandler {
    protected PointerEventData _eventData;
    
    public override void GatherInput(ref OpInput input) {
        if (_eventData != null) {
            input.ctrl = ECtrlKey.Nil;
            if (_eventData.delta.x > 0f) {
                input.ctrl |= ECtrlKey.Right;
            }
            else if (_eventData.delta.x < 0f) {
                input.ctrl |= ECtrlKey.Left;
            }

            if (_eventData.delta.y > 0f) {
                input.ctrl |= ECtrlKey.Forward;
            }
            else if (_eventData.delta.y < 0f) {
                input.ctrl |= ECtrlKey.Backward;
            }
        }
    }
    
    public void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        _eventData = eventData;
    }
}