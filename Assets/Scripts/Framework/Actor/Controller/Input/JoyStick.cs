using System;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class JoyStick : MonoBehaviour {
    public ClickRaycaster clickInput;
    
    public KeyboardCtrlInput keyboardInput;
    public TouchCtrlInput touchInput;
    public HandleCtrlInput handleInput;
    
    private OpInput _opInput;
    
    // 点击地面驱动人物移动 开关
    public bool enableClickScreen = true;
    // 人物移动 开关
    public bool enableCtrl = true;
    
    // 供外部监听
    public Action<Vector2> onClick;
    public Action<Vector2> onTouchDown;
    public Action<Vector2> onTouchUp;
    
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
                this.touchInput.GatherAll(ref _opInput);
                keyboardInput.GatherAll(ref _opInput);
                handleInput.GatherAll(ref _opInput);
                
                if (_opInput.HasAny) {
                    _OnCtrl();
                }
            }
        }
    }

    private void OnEnable() {
        clickInput.onClick += _OnClick;
        clickInput.onTouchDown += _OnTouchDown;
        clickInput.onTouchUp += _OnTouchUp;
    }

    private void OnDisable() {
        clickInput.onClick -= _OnClick;
        clickInput.onTouchDown -= _OnTouchDown;
        clickInput.onTouchUp -= _OnTouchUp;
    }
    
    private void _OnClick(Vector2 touchPosition) {
        if (enableClickScreen) {
            onClick?.Invoke(touchPosition);
        }
    }
    
    private void _OnTouchDown(Vector2 touchPosition) {
        if (enableClickScreen) {
            onTouchDown?.Invoke(touchPosition);
        }
    }
    
    private void _OnTouchUp(Vector2 touchPosition) {
        if (enableClickScreen) {
            onTouchUp?.Invoke(touchPosition);
        }
    }
    
    private void _OnCtrl() {
        if (enableCtrl) {
            onCtrl?.Invoke(_opInput);
        }
    }
}
