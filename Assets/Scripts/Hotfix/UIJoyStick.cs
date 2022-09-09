using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[Serializable]
public class Joy {
    // ref https://zhuanlan.zhihu.com/p/266077243
    // ref https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/
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

    public float r {
        get {
            return this.circle.sizeDelta.x / 2.5f;
        }
    }

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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joyRect, touchPos, eventData.pressEventCamera, out Vector2 localPosition);
            this.circle.localPosition = localPosition;
        }
    }

    public void SetCirclePosition(Vector2 touchPos, PointerEventData eventData) {
        if (joyType == EJoyType.TouchPosition) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joyRect, touchPos, eventData.pressEventCamera, out Vector2 localPosition); 
            this.circle.localPosition = localPosition;
        }
    }

    // 控制Arrow的位置和旋转
    public void CtrlArrow(PointerEventData eventData) {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(circle, eventData.position, eventData.pressEventCamera, out Vector2 localPosition);
        if (localPosition.magnitude > r) {
            localPosition = localPosition.normalized * r;
        }

        // pos
        this.arrow.localPosition = localPosition;
        
        var dir = localPosition / r;
        // angle
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        this.arrow.localEulerAngles = new Vector3(0f, 0f, angle);
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
    }

    private void OnDisable() {
        this.playerInput.onBeginDrag -= this._OnBeginDrag;
        this.playerInput.onDrag -= this._OnDrag;
        this.playerInput.onEndDrag -= this._EndDrag;
    }
    
    private void _OnBeginDrag(Vector2 touchPosition, PointerEventData eventData) {
        this.uiJoy.SetCircleFirstPosition(touchPosition, eventData);
        this.uiJoy?.Active(true);
    }

    private void _OnDrag(Vector2 delta, PointerEventData eventData) {
        this.uiJoy.SetCirclePosition(eventData.position, eventData);
        this.uiJoy?.CtrlArrow(eventData);
    }

    private void _EndDrag(Vector2 touchPosition, PointerEventData eventData) {
        this.uiJoy?.Active(false);
    }
}
