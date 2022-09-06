using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class InputMgr : MonoBehaviour {
    public CtrlInput ctrlInput;

    // 点击地面驱动人物移动 开关
    public bool enableClickScreen = true;
    // 人物移动 开关
    public bool enableCtrl = true;
    
    public Action<Vector2> onClick;
    public Action onCtrl;

    public static InputMgr instance { get; private set; } = null;

    private OpInput _opInput;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if (Input.anyKey) {
            if (this.ctrlInput) {
                this.ctrlInput.GatherCtrlInput(ref _opInput.ctrl);
                this.ctrlInput.GatherSkillInput(ref _opInput.skill);
                this.ctrlInput.GatherOtherInput(ref _opInput.other);
            }
        }
    }

    public void OnClick(Vector2 touchPosition) {
        if (enableClickScreen) {
            onClick?.Invoke(touchPosition);
        }
    }
    
    public void OnCtrl() {
        if (enableCtrl) {
            onCtrl?.Invoke();
        }
    }
}
