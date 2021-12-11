using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIEventTrigger : EventTrigger {
    public Action<GameObject> onClick;
    public Action<GameObject> onDoubleClick;
    public Action<GameObject, bool> onHover;
    public Action<GameObject, bool> onPress;
    public Action<GameObject, float> onScroll;
    public Action<GameObject, Vector2> onDrag;
    public Action<GameObject> onDragStart;
    public Action<GameObject> onDragEnd;
    public Action<GameObject, GameObject> onDragOver;
    public Action<GameObject, GameObject> onDragOut;

    private IList<ScrollRect> scrollRects;

    private void Awake() {
        if (scrollRects == null || scrollRects.Count <= 0) {
            scrollRects = GetComponentsInParent<ScrollRect>();
        }
    }

    public static UIEventTrigger Get(GameObject go) {
        if (!go.TryGetComponent(out UIEventTrigger trigger)) {
            trigger = go.AddComponent<UIEventTrigger>();
        }

        return trigger;
    }

    public static bool IsOverUI() {
        bool isTouch = false;
        if (EventSystem.current != null) {
            if (Application.isEditor) {
                // 只针对于RayCaster==true的UI，对于3d模型无效
                isTouch = EventSystem.current.IsPointerOverGameObject();
            }
            else {
                isTouch = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
        }

        return isTouch;
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
        base.OnPointerClick(eventData);
        if (onClick != null) {
            onClick(gameObject);
        }

        if (eventData.clickCount == 2 && onDoubleClick != null) {
            onDoubleClick(gameObject);
        }
        // ˫��ԭ������ʹ��eventData.clickTime
        // if (Time.time > 0.35f + eventData.clickTime && onDoubleClick != null) { onDoubleClick(gameObject); }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);
        if (onPress != null) {
            onPress(gameObject, true);
        }
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
        if (onPress != null) {
            onPress(gameObject, false);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if (onHover != null) {
            onHover(gameObject, true);
        }

        if (eventData.dragging && onDragOver != null) {
            onDragOver(gameObject, eventData.pointerDrag);
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if (onHover != null) {
            onHover(gameObject, false);
        }

        if (eventData.dragging && onDragOut != null) {
            onDragOut(gameObject, eventData.pointerDrag);
        }
    }

    // ��ScrollRect�����ʹ��EventListener�����ǲ�ʵ��ScrollRect��dragϵ�к����Ļ����ͻᵼ��ʧȥ��ק���ܣ���������ʵ��
    // https://blog.csdn.net/qiangqiang_0420/article/details/51375856
    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnDrag(eventData);
        }

        if (onDrag != null) {
            onDrag(gameObject, eventData.delta);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnBeginDrag(eventData);
        }

        if (onDragStart != null) {
            onDragStart(gameObject);
        }
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnEndDrag(eventData);
        }

        if (onDragEnd != null) {
            onDragEnd(gameObject);
        }
    }

    public override void OnScroll(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnScroll(eventData);
        }

        if (onScroll != null) {
            onScroll(gameObject, Input.GetAxis("Mouse ScrollWheel"));
        }
    }
    #endregion
}
