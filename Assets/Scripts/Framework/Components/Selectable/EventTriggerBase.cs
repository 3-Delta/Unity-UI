using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EventTriggerBase : EventTrigger {
    public Action<GameObject, Vector3> onClick;
    public Action<GameObject, Vector3> onDoubleClick;

    public Action<GameObject, bool, Vector3> onPress;

    public Action<GameObject, Vector2> onDrag;
    public Action<GameObject> onDragStart;
    public Action<GameObject> onDragEnd;

    public Action<GameObject, GameObject> onDragOver;
    public Action<GameObject, GameObject> onDragOut;

    public Action<GameObject, bool> onHover;
    public Action<GameObject, float> onScroll;

    public static UIEventTrigger Get(GameObject go) {
        if (!go.TryGetComponent(out UIEventTrigger trigger)) {
            trigger = go.AddComponent<UIEventTrigger>();
        }

        return trigger;
    }

    public void AddListener(EventTriggerType eType, UnityAction<BaseEventData> action) {
        if (action != null) {
            Entry entry = new Entry();
            entry.eventID = eType;
            entry.callback.AddListener(action);

            triggers = triggers ?? new List<Entry>();
            triggers.Add(entry);
        }
    }

    #region

    public override void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        base.OnPointerClick(eventData);
        onClick?.Invoke(eventData.pointerPress, eventData.position);

        if (eventData.clickCount == 2) {
            onDoubleClick?.Invoke(eventData.pointerPress, eventData.position);
        }
        // 双击不是eventData.clickTime，而是
        // if (Time.time > 0.35f + eventData.clickTime && onDoubleClick != null) { onDoubleClick(gameObject); }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        base.OnPointerDown(eventData);
        onPress?.Invoke(eventData.pointerPress, true, eventData.pressPosition);
    }

    public override void OnPointerUp(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        base.OnPointerUp(eventData);
        onPress?.Invoke(eventData.pointerPress, false, eventData.pressPosition);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        onHover?.Invoke(eventData.pointerEnter, true);

        if (eventData.dragging) {
            onDragOver?.Invoke(eventData.pointerEnter, eventData.pointerDrag);
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        onHover?.Invoke(eventData.pointerEnter, false);

        if (eventData.dragging) {
            onDragOut?.Invoke(eventData.pointerEnter, eventData.pointerDrag);
        }
    }

    // https://blog.csdn.net/qiangqiang_0420/article/details/51375856
    public override void OnDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        base.OnDrag(eventData);
        onDrag?.Invoke(eventData.pointerDrag, eventData.delta);
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        base.OnDrag(eventData);
        onDragStart?.Invoke(eventData.pointerDrag);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        base.OnDrag(eventData);
        onDragEnd?.Invoke(eventData.pointerDrag);
    }

    public override void OnScroll(PointerEventData eventData) {
        base.OnScroll(eventData);
        onScroll?.Invoke(eventData.pointerEnter, Input.GetAxis("Mouse ScrollWheel"));
    }

    #endregion
}