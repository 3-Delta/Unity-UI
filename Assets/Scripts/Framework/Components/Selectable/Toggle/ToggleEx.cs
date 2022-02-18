using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ToggleEx : Selectable, IPointerClickHandler {
    public class ToggleEvent : UnityEvent<bool> { }

    public ToggleEvent onValueChanged = new ToggleEvent();

    public uint id;
    public ToggleRegistry ownerRegistry;
    public ActiverSingleGroup activer;

    protected override void OnEnable() {
        if (this.ownerRegistry != null) {
            this.ownerRegistry.RegisterToggle(this);
        }
    }

    protected override void OnDisable() {
        if (this.ownerRegistry != null) {
            this.ownerRegistry.UnregisterToggle(this);
        }
    }

    public bool isOn { get; private set; } = false;

    // 手动点击触发
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        if (!this.IsActive() || !this.IsInteractable()) {
            return;
        }

        Select();
    }

    public void Select() {
        this.ownerRegistry.SwitchTo(this, this.isOn);
    }

    // Toggle控制 activer 显/隐
    // 以及isOn
    public void OnSelected(bool toSelect) {
        isOn = toSelect;
        this.activer?.SetActive(toSelect);
    }
}
