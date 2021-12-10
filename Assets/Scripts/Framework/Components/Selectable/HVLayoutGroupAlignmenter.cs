using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
public class HVLayoutGroupAlignmenter : MonoBehaviour {
    [ReadOnly] [SerializeField] private HorizontalOrVerticalLayoutGroup layout = null;

    [Range(0, 99)] public int Limit = 3;
    public TextAnchor normalAlignment = TextAnchor.MiddleCenter;
    public TextAnchor overAlignment = TextAnchor.MiddleLeft;

    private void Awake() {
        if (layout == null) {
            layout = GetComponent<HorizontalOrVerticalLayoutGroup>();
        }

        Align();
    }

    public void Align() {
        layout.childAlignment = transform.childCount > Limit ? overAlignment : normalAlignment;
    }
}
