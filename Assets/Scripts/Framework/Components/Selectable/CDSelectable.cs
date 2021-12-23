using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 按钮点击的cd控制
[DisallowMultipleComponent]
[RequireComponent(typeof(Selectable))]
public class CDSelectable : MonoBehaviour, IPointerClickHandler {
    public Selectable selectable;
    [Range(0.1f, 5f)] public float selfCd = 0.4f;
    public Material grayMaterial;
    public CDSelectableRegistry registry;

    private Graphic[] graphics = new Graphic[0];

    public bool status {
        get { return selectable.enabled; }
        set {
            if (selectable.enabled != value) {
                selectable.enabled = value;

                if (grayMaterial != null) {
                    if (!value) {
                        foreach (var graphic in graphics) {
                            graphic.material = grayMaterial;
                        }
                    }
                    else {
                        foreach (var graphic in graphics) {
                            graphic.material = null;
                        }
                    }
                }
            }
        }
    }

    private void Awake() {
        if (selectable == null) {
            selectable = GetComponent<Selectable>();
        }

        if (grayMaterial != null) {
            graphics = GetComponentsInChildren<Graphic>();
        }
    }

    private void OnEnable() {
        if (registry != null) {
            registry.Register(this);
        }
    }

    private void OnDisable() {
        if (registry != null) {
            registry.Unregister(this);
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
            if (registry == null) {
                DisableImmediately(selfCd);
            }
            else {
                registry.OnAnyClicked();
            }
        }
    }

    public void DisableImmediately(float cd) {
        CancelInvoke(nameof(_CDCtrl));
        Invoke(nameof(_CDCtrl), cd);
        status = false; 
    }

    private void _CDCtrl() {
        status = true;
    }
}
