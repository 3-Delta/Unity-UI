using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 触摸屏输入
// 依赖RayCaster
// 需要在主界面UI的层级下面
[DisallowMultipleComponent]
public class TouchScreenInput : InputBase, IPointerClickHandler, IBeginDragHandler, IDragHandler,
    IEndDragHandler {
    public override void GatherInput(ref OpInput input) {
    }

    protected bool _hasDrag = false;
    
    // 左右鼠标键一起按下，然后抬起右键，这里不会响应
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        
        if (!_hasDrag) {
            
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        _hasDrag = true;
    }

    public void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        
        _hasDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        
        _hasDrag = true;
    }
}