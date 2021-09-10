using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 按钮点击的cd控制
[DisallowMultipleComponent]
[RequireComponent(typeof(Selectable))]
public class CDSelectable : MonoBehaviour, IPointerClickHandler {
    public Selectable selectable;
    [Range(0.1f, 5f)] public float cdTime = 0.4f;

    public bool status {
        get { return selectable.enabled; }
        set { selectable.enabled = value; }
    }

    private void Awake() {
        if (selectable == null) {
            selectable = GetComponent<Selectable>();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        OnClicked();
    }

    private void OnClicked() {
        if (status) {
            Invoke(nameof(_CDCtrl), cdTime);
            status = false;
        }
    }

    private void _CDCtrl() {
        status = true;
    }
}
