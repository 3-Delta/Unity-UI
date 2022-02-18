using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Toggle))]
public class ToggleListener : MonoBehaviour {
    public enum EToggleType {
        HideShow,
    }

    public Toggle _toggle;

    public Toggle toggle {
        get {
            if (_toggle == null) {
                _toggle = GetComponent<Toggle>();
            }

            return _toggle;
        }
    }

    public EToggleType tgType = EToggleType.HideShow;
    public ActiverGroup group;

    private void Awake() {
        toggle.onValueChanged.AddListener(_OnValueChanged);
    }

    private void _OnValueChanged(bool active) {
        switch (tgType) {
            case EToggleType.HideShow:
                HideShow(active);
                break;
        }
    }

    private void HideShow(bool active) {
        group.SetActive(active);
    }
}
