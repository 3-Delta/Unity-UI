using UnityEngine;
using UnityEngine.EventSystems;

/*
[DisallowMultipleComponent]
public class UICenterOnClick : MonoBehaviour, IPointerClickHandler {
    public UICenterOnChild centerOn;

    private void Awake() {
        if (centerOn == null) {
            centerOn = transform.GetComponentInParent<UICenterOnChild>();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        if (centerOn != null) {
            centerOn.CenterOn(transform, true);
        }
    }
}
*/
