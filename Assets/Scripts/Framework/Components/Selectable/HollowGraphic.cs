using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HollowGraphic : MonoBehaviour {
    public RectTransform rectTransform;

    private void Awake() {
        SetTarget(rectTransform);
    }

    public void SetTarget(RectTransform rectTransform) {
        if (rectTransform != null) {
            this.rectTransform = rectTransform;
            Graphic graphic = rectTransform.gameObject.GetComponent<Graphic>();
            if (graphic != null) {
                graphic.raycastTarget = false;
            }
        }
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
        if (rectTransform == null) {
            return true;
        }
        else {
            return !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);
        }
    }
}
