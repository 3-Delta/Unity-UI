using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class InputMgr : MonoBehaviour {
    public MoveInput moveInput;

    // 点击地面驱动人物移动 开关
    public bool enableClickScreen = true;
    // 人物移动 开关
    public bool enableMove = true;

    public static InputMgr instance { get; private set; } = null;

    private OpInput _opInput;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        if (Input.anyKey) {
            if (moveInput) {
                moveInput.GatherInput(ref _opInput);
            }
        }
    }

    public Action<Vector2> onClick;
    public Action<ECtrlKey> onCtrl;
}
