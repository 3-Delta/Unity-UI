using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Rotater : MonoBehaviour {
    // 控制x/y/z轴是否需要旋转，false，0f  true，1f即可
    public Vector3 axis = new Vector3(1f, 1f, 1f);
    public Transform target;
    public float speed = 100f;

    private Vector3 currentEulerAngles;

    private void Awake() {
        if (target == null) {
            target = transform;
        }

        currentEulerAngles = target.transform.localEulerAngles;
    }

    private void Update() {
        currentEulerAngles += Time.deltaTime * speed * axis;
        target.localEulerAngles = currentEulerAngles;
    }
}
