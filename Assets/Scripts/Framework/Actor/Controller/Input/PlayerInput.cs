using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour {
    public ClickRaycaster clickRaycaster;
    
    public KeyboardCtrlInput keyboardInput;
    public TouchCtrlInput touchInput;
    public HandleCtrlInput handleInput;
    
    private OpInput _opInput;
    
    // 点击地面驱动人物移动 开关
    public bool enableClickScreen = true;
    // 人物移动摇杆 开关
    public bool enableCtrl = true;
    
    // 供外部监听
    public Action<Vector2, PointerEventData> onClick;
    public Action<Vector2, PointerEventData> onTouchDown;
    public Action<Vector2, PointerEventData> onTouchUp;

    public Action<Vector2, PointerEventData> onBeginDrag;
    public Action<Vector2, PointerEventData> onDrag;
    public Action<Vector2, PointerEventData> onEndDrag;
    
    public Action<OpInput> onCtrl;
    
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
        this.clickRaycaster.onClick += _OnClick;
        this.clickRaycaster.onTouchDown += _OnTouchDown;
        this.clickRaycaster.onTouchUp += _OnTouchUp;

        this.touchInput.onBeginDrag += _OnBeginDrag;
        this.touchInput.onDrag += _OnDrag;
        this.touchInput.onEndDrag += _OnEndDrag;
    }

    private void OnDisable() {
        this.clickRaycaster.onClick -= _OnClick;
        this.clickRaycaster.onTouchDown -= _OnTouchDown;
        this.clickRaycaster.onTouchUp -= _OnTouchUp;
        
        this.touchInput.onBeginDrag -= _OnBeginDrag;
        this.touchInput.onDrag -= _OnDrag;
        this.touchInput.onEndDrag -= _OnEndDrag;
    }
    
    private void _OnClick(Vector2 touchPosition, PointerEventData eventData) {
        if (enableClickScreen) {
            onClick?.Invoke(touchPosition, eventData);
        }
    }
    
    private void _OnTouchDown(Vector2 touchPosition, PointerEventData eventData) {
        if (enableClickScreen) {
            onTouchDown?.Invoke(touchPosition, eventData);
        }
    }
    
    private void _OnTouchUp(Vector2 touchPosition, PointerEventData eventData) {
        if (enableClickScreen) {
            onTouchUp?.Invoke(touchPosition, eventData);
        }
    }

    private void _OnBeginDrag(Vector2 touchPosition, PointerEventData eventData) {
        if (enableClickScreen) {
            this.onBeginDrag?.Invoke(touchPosition, eventData);
        }
    }
    
    private void _OnDrag(Vector2 delta, PointerEventData eventData) {
        if (enableClickScreen) {
            this.onDrag?.Invoke(delta, eventData);
        }
    }
    
    private void _OnEndDrag(Vector2 touchPosition, PointerEventData eventData) {
        if (enableClickScreen) {
            this.onEndDrag?.Invoke(touchPosition, eventData);
        }
    }

    private void _OnCtrl() {
        if (enableCtrl) {
            onCtrl?.Invoke(_opInput);
        }
    }
}
