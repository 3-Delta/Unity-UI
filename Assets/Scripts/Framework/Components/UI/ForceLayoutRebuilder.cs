using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 强制布局重新刷新
[DisallowMultipleComponent]
public class ForceLayoutRebuilder : MonoBehaviour {
    [Range(0.05f, 99f)] public float delayTime = 0.05f;

    public bool useWhenEnable = false;

    private void OnEnable() {
        if (useWhenEnable) {
            Set();
        }
    }

    private void OnDisable() {
        CancelInvoke(nameof(OnTimeEnd));
    }

    public void Set() {
        CancelInvoke(nameof(OnTimeEnd));
        Invoke(nameof(OnTimeEnd), delayTime);
    }

    private void OnTimeEnd() {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

        // ContentSizeFitter[] ls = gameObject.GetComponentsInChildren<ContentSizeFitter>();
        // for (int i = 0, length = ls.Length; i < length; ++i) {
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(ls[i].transform as RectTransform);
        // }
    }
}
