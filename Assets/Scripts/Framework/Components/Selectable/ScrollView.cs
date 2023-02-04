using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ScrollView : ScrollRect {
    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onDraging;
    public Action<PointerEventData> onEndDrag;

    public bool isDraging { get; private set; } = false;

    public bool isMoving {
        get {
            if (horizontal) {
                return velocity[0] > 0f;
            }
            else if (vertical) {
                return velocity[1] > 0f;
            }

            return velocity != Vector2.zero;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        this.onBeginDrag?.Invoke(eventData);
    }

    public override void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
        isDraging = true;
        this.onDraging?.Invoke(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        isDraging = false;
        this.onEndDrag?.Invoke(eventData);
    }

    protected override void OnDisable() {
        base.OnDisable();
        isDraging = false;
    }
}
