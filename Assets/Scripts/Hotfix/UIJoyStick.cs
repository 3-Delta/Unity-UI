using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[Serializable]
public class Joy {
    // ref https://zhuanlan.zhihu.com/p/266077243
    public enum EJoyType {
        FixedPosition,
        ClickPosition,
        TouchPosition,
    }
    
    // 是否固定在特定位置
    public EJoyType joyType;
    // 特定位置锚点
    public Transform fixedPos;
    public RectTransform range; // 拖拽的位置受限制，需要在range范围之内

    public bool useJoy = true;
    public RectTransform joyRect;
    public RectTransform circle;
    public RectTransform arrow;

    // 显隐UI遥感
    public void Active(bool toActive) {
        if (useJoy && circle) {
            circle.gameObject.SetActive(toActive);
        }
    }
    
    public void SetCircleFirstPosition(Vector2 touchPos, PointerEventData eventData) {
        if (joyType == EJoyType.FixedPosition) {
            this.circle.localPosition = fixedPos.localPosition;
        }
        else {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joyRect, touchPos, eventData.pressEventCamera, out Vector2 localPosition)) {
                this.circle.localPosition = localPosition;
            }
            else {
                // error 透射点在joyRect区域之外
            }
        }
    }

    public void SetCirclePosition(Vector2 touchPos, PointerEventData eventData) {
        if (joyType == EJoyType.TouchPosition) {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joyRect, touchPos, eventData.pressEventCamera, out Vector2 localPosition)) {
                this.circle.localPosition = localPosition;
            }
            else {
                // error 透射点在joyRect区域之外
            }
        }
    }

    // 控制Arrow的位置和旋转
    public void CtrlArrow(in OpInput input) {
        if (input.HasMoveCtrl) {
            // input.ctrl
        }
    }
}

// 其实是JoyStick套了层壳
[DisallowMultipleComponent]
public class UIJoyStick : MonoBehaviour {
    public PlayerInput playerInput;
    public Joy uiJoy;
    
    private void OnEnable() {
        this.playerInput.onBeginDrag += this._OnBeginDrag;
        this.playerInput.onDrag += this._OnDrag;
        this.playerInput.onEndDrag += this._EndDrag;

        this.playerInput.onCtrl += this._OnCtrl;
    }

    private void OnDisable() {
        this.playerInput.onBeginDrag -= this._OnBeginDrag;
        this.playerInput.onDrag -= this._OnDrag;
        this.playerInput.onEndDrag -= this._EndDrag;

        this.playerInput.onCtrl -= this._OnCtrl;
    }
    
    private void _OnBeginDrag(Vector2 touchPosition, PointerEventData eventData) {
        this.uiJoy.SetCircleFirstPosition(touchPosition, eventData);
        this.uiJoy?.Active(true);
    }

    private void _OnDrag(Vector2 delta, PointerEventData eventData) {
        this.uiJoy.SetCirclePosition(eventData.position, eventData);
    }

    private void _EndDrag(Vector2 touchPosition, PointerEventData eventData) {
        this.uiJoy?.Active(false);
    }

    private void _OnCtrl(OpInput input) {
        this.uiJoy?.CtrlArrow(in input);
        // 1. 摇杆旋转
        // 控制人物移动
    }
}
