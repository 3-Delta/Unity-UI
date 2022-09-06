using System;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class DragRaycaster : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
    public Action<GameObject, Vector2> onDrag;
    public Action<GameObject> onDragStart;
    public Action<GameObject> onDragEnd;

    public Action<GameObject, GameObject> onDragOver;
    public Action<GameObject, GameObject> onDragOut;

    // https://blog.csdn.net/qiangqiang_0420/article/details/51375856
    public void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        onDrag?.Invoke(eventData.pointerDrag, eventData.delta);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        onDragStart?.Invoke(eventData.pointerDrag);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        onDragEnd?.Invoke(eventData.pointerDrag);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.dragging) {
            onDragOver?.Invoke(eventData.pointerEnter, eventData.pointerDrag);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.dragging) {
            onDragOut?.Invoke(eventData.pointerEnter, eventData.pointerDrag);
        }
    }
}
