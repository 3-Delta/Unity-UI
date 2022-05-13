using System;
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
        bool can = (this.clickCondition == null);
        can |= (clickCondition != null && this.clickCondition.Invoke());
        if (can) {
            this.ownerRegistry.SwitchTo(this, this.isOn);
        }
    }

    // Toggle控制 activer 显/隐
    // 以及isOn
    public void OnSelected(bool toSelect) {
        isOn = toSelect;
        this.activer?.SetActive(toSelect);
    }

#region 点击条件
    // 某些时候selectable不可点击
    private Func<bool> clickCondition;

    // 参数传递null也可以ClearCondition
    public void RegistCondition(Func<bool> func) {
        this.clickCondition = func;
    }

    public void ClearCondition() {
        this.clickCondition = null;
    }
#endregion

}
