using System;
using System.Timers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Selectable))]
public class SelectableScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Selectable target;

    [Range(0.02f, 99f)] public float fadeTime = 0.2f;
    [Range(0.5f, 1.5f)] public float scaleRate = 0.85f;

    private float _pressTime = 0f;
    private bool _pressDown = true;

    private bool _isCompleted = true;
    private float _originalScale = 1f;
    private float _currentScale = 1f;

    private void Awake() {
        if (target == null) {
            target = GetComponent<Selectable>();
        }

        _originalScale = target.transform.localScale.x;
    }

    public void OnPointerDown(PointerEventData eventData) {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
         if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

#endif
        Press(true);
    }

    public void OnPointerUp(PointerEventData eventData) {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
         if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

#endif
        Press(false);
    }

    protected void Press(bool press) {
        _pressTime = Time.time;
        _pressDown = press;
        _isCompleted = false;
    }

    private void Update() {
        if (!_isCompleted) {
            float diff = Time.time - _pressTime;
            if (_pressDown) {
                float curScale = target.transform.localScale.x;
                float scale = Mathf.Lerp(curScale, _originalScale * scaleRate, diff / fadeTime);
                target.transform.localScale = new Vector3(scale, scale, scale);
            }
            else {
                float curScale = target.transform.localScale.x;
                float scale = Mathf.Lerp(curScale, _originalScale, diff / fadeTime);
                target.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else {
            if (_pressTime != 0f) {
                if (_pressDown) {
                    float scale = scaleRate * _originalScale;
                    target.transform.localScale = new Vector3(scale, scale, scale);
                }
                else {
                    target.transform.localScale = new Vector3(_originalScale, _originalScale, _originalScale);
                }
            }
        }
    }
}
