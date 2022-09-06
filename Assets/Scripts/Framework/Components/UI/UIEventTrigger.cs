using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIEventTrigger : EventTriggerBase {
    protected IList<ScrollRect> scrollRects;

    private void Awake() {
        if (scrollRects == null || scrollRects.Count <= 0) {
            scrollRects = GetComponentsInParent<ScrollRect>();
        }
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

    // https://blog.csdn.net/qiangqiang_0420/article/details/51375856
    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnDrag(eventData);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnBeginDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnEndDrag(eventData);
        }
    }

    public override void OnScroll(PointerEventData eventData) {
        base.OnScroll(eventData);
        for (int i = 0, length = scrollRects.Count; i < length; i++) {
            scrollRects[i]?.OnScroll(eventData);
        }
    }
}