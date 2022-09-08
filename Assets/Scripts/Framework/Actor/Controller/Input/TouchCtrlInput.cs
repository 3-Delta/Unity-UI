using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 触摸屏输入
// 依赖RayCaster
// 需要在主界面UI的层级下面
[DisallowMultipleComponent]
public class TouchCtrlInput : CtrlInput, IBeginDragHandler, IDragHandler, IEndDragHandler {
    protected PointerEventData _eventData;
    
    public Action<Vector2, PointerEventData> onBeginDrag;
    public Action<Vector2, PointerEventData> onDrag;
    public Action<Vector2, PointerEventData> onEndDrag;

    public override void GatherCtrlInput(ref ECtrlKey input) {
        if (_eventData != null) {
            if (_eventData.delta.x > 0f) {
                input |= ECtrlKey.Right;
            }
            else if (_eventData.delta.x < 0f) {
                input |= ECtrlKey.Left;
            }

            if (_eventData.delta.y > 0f) {
                input |= ECtrlKey.Forward;
            }
            else if (_eventData.delta.y < 0f) {
                input |= ECtrlKey.Backward;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        
        onBeginDrag?.Invoke(eventData.position, eventData);
    }
    
    public void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        _eventData = eventData;
        onDrag?.Invoke(eventData.delta, eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        _eventData = null;
        this.onEndDrag?.Invoke(eventData.position, eventData);      
    }
}
