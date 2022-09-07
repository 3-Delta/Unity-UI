using System;
using UnityEngine;

[DisallowMultipleComponent]
public class JoyStick : MonoBehaviour {
    public Transform joy;

    public ClickRaycaster clickInput;
    
    public KeyboardCtrlInput keyboardInput;
    public TouchRaycasterCtrlInput touchInput;
    public HandleCtrlInput handleInput;
    
    private OpInput _opInput;
    
    // 点击地面驱动人物移动 开关
    public bool enableClickScreen = true;
    // 人物移动 开关
    public bool enableCtrl = true;
    
    // 供外部监听
    public Action<Vector2> onClick;
    public Action<OpInput> onCtrl;
    
    public static JoyStick instance { get; private set; } = null;
    
    private void Awake() {
        instance = this;
    }
    
    private void Update() {
        _opInput.Reset();
        // if (Input.anyKey) 
        {
            if (enableCtrl) 
            {
                touchInput.GatherAll(ref _opInput);
                keyboardInput.GatherAll(ref _opInput);
                handleInput.GatherAll(ref _opInput);
            }
        }

        if (_opInput.HasAny) {
            _OnCtrl();
        }
    }

    private void OnEnable() {
        clickInput.onClick += _OnClick;
    }

    private void OnDisable() {
        clickInput.onClick -= _OnClick;
    }
    
    // 显隐UI遥感
    public void ActiveJoy(bool toActive) {
        if (joy) {
            joy.gameObject.SetActive(toActive);
        }
    }

    private void _OnClick(Vector2 touchPosition) {
        if (enableClickScreen) {
            onClick?.Invoke(touchPosition);
        }
    }
    
    private void _OnCtrl() {
        if (enableCtrl) {
            onCtrl?.Invoke(_opInput);
        }
    }
}
