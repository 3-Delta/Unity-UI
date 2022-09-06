using System;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class LongPressTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public float duration = 0.2f;

    // public Action<GameObject> onClick;
    public Action<GameObject> onStartLongPress;
    public Action<GameObject> onLongPress;
    public Action<GameObject> onEndLongPress;

    private bool _startLongPress;
    private float _pressStartTime;
    private bool _isPressing;
    private PointerEventData _eventData;

    private void Update() {
        if (_isPressing) {
            if (Time.unscaledTime - _pressStartTime >= duration) {
                if (!_startLongPress) {
                    _startLongPress = true;
                    onStartLongPress?.Invoke(_eventData.pointerPress);
                }

                onLongPress?.Invoke(_eventData.pointerPress);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        _startLongPress = false;
        _isPressing = true;
        _eventData = eventData;
        _pressStartTime = Time.unscaledTime;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        if (_startLongPress) {
            onEndLongPress?.Invoke(eventData.pointerPress);
        }
        // else {
        //     onClick?.Invoke(eventData.pointerPress);
        // }

        _startLongPress = false;
        _isPressing = false;
        _eventData = null;
    }
}