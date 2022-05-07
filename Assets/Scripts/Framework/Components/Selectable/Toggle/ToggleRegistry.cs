using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ToggleRegistry : UIBehaviour {
    public Action<uint, ToggleEx, bool> onValueChanged;

    public bool allowSwitchOff = false;

    private List<ToggleEx> toggles = new List<ToggleEx>();

    // 点击toggle调用
    // 传入不合理的Toggle会取消所有高亮
    public void SwitchTo(ToggleEx toggle, bool isOn, bool force = false) {
        if (this.allowSwitchOff) {
            isOn = !isOn;
        }
        else {
            isOn = true;
        }

        foreach (var tg in toggles) {
            bool old = toggle.isOn;
            bool sendMessage = (old == isOn && force) || (old != isOn);

            bool toSelect = false;
            if (tg == toggle) {
                toSelect = isOn;
            }

            if (sendMessage) {
                onValueChanged?.Invoke(tg.id, tg, toSelect);
                tg.onValueChanged?.Invoke(toSelect);
            }
        }
    }

    // 外部调用，可能存在id重复性的问题
    public void SwitchToById(uint id, bool isOn = true, bool force = false) {
        if (this.allowSwitchOff) {
            isOn = !isOn;
        }
        else {
            isOn = true;
        }

        // 通知控制表现和逻辑
        foreach (var toggle in toggles) {
            bool old = toggle.isOn;
            bool sendMessage = (old == isOn && force) || (old != isOn);

            bool toSelect = false;
            if (toggle.id == id) {
                toSelect = isOn;
            }
            else {
                toSelect = false;
            }

            if (sendMessage) {
                onValueChanged?.Invoke(toggle.id, toggle, toSelect);
                toggle.onValueChanged?.Invoke(toSelect);
            }
        }
    }

    private bool CheckValid(ToggleEx toggle) {
        return !(toggle == null || !toggles.Contains(toggle));
    }

    public void RegisterToggle(ToggleEx toggle) {
        if (!toggles.Contains(toggle)) {
            toggles.Add(toggle);
        }
    }

    public void UnregisterToggle(ToggleEx toggle) {
        toggles.Remove(toggle);
    }
}
