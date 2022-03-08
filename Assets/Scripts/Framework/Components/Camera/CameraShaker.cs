using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CameraShaker : MonoBehaviour {
    private Camera _camera;

    public Camera camera {
        get {
            if (_camera == null) {
                _camera = GetComponent<Camera>();
            }

            return _camera;
        }
    }

    public bool resetWhenFinish = true;

    [SerializeField] private Vector3 posBeforeShake;
    // private Tweener shakeTwener;

    private void LateUpdate() {
        if (_shakeOffset != Vector3.zero) {
            var pos = posBeforeShake + _shakeOffset;
            camera.transform.position = pos;
        }
    }

#if UNITY_EDITOR
    [SerializeField] private Vector3 strength;
    [SerializeField] [Range(0.02f, 99f)] private float duration;
    [SerializeField] private float randomness;
    [SerializeField] private int vibrato;

    [ContextMenu(nameof(Begin))]
    public void Begin() {
        Begin(this.duration, this.strength, this.vibrato, this.randomness);
    }

#endif

    public void Begin(float duration, Vector3 strength, int vibrato = 10, float randomness = 90f, bool fadeout = true) {
        // if (shakeTwener != null) {
        //     shakeTwener.Kill(true);
        // }
        //
        // if (camera != null) {
        //     posBeforeShake = camera.transform.position;
        // }
        //
        // shakeTwener = DOTween.Shake(GetShakeOffset, SetShakeOffset, duration, strength, vibrato, randomness, fadeout);
        // shakeTwener.onComplete += this.OnTweenFinish;
        // shakeTwener.onKill += this.OnTweenFinish;
    }

    [ContextMenu(nameof(End))]
    public void End() {
        // if (shakeTwener != null) {
        //     shakeTwener.Kill(true);
        // }
    }

    private void OnTweenFinish() {
        if (resetWhenFinish) {
            camera.transform.position = posBeforeShake;
        }

        // shakeTwener.onComplete -= this.OnTweenFinish;
        // shakeTwener.onKill -= this.OnTweenFinish;
        // shakeTwener = null;
    }

    private Vector3 _shakeOffset;

    private Vector3 GetShakeOffset() {
        return _shakeOffset;
    }

    private void SetShakeOffset(Vector3 offset) {
        _shakeOffset = offset;
    }
}
