using UnityEngine;
using UnityEngine.EventSystems;

// 点击vd之后居中
[DisallowMultipleComponent]
public class CenterOnClick : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        CenterOnChild center = this.transform.GetComponentInParent<CenterOnChild>();
        if (center != null && center.enabled) {
            center.CenterOn(this.transform);
        }
    }
}
