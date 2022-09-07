using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 点击地面驱动人物移动
[DisallowMultipleComponent]
public class ClickRaycaster : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
    public Action<Vector2> onClick;
    public Action<Vector2> onTouchDown;
    public Action<Vector2> onTouchUp;

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        if (!eventData.dragging) {
            // 点击过程中没有拖动
            onClick?.Invoke(eventData.position);
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        if (!eventData.dragging) {
            onTouchDown?.Invoke(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        
        onTouchUp?.Invoke(eventData.position);
    }
}
