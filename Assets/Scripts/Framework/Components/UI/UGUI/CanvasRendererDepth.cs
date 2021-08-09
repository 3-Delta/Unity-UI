#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class UGUIGraphicDepth : MonoBehaviour {
    private CanvasRenderer[] graphics;
    [SerializeField] private List<string> names = new List<string>();

    private void OnEnable() {
        hideFlags = HideFlags.DontSaveInBuild;

        names.Clear();
        graphics = GetComponentsInChildren<CanvasRenderer>();
        int length = graphics.Length;
        for (int i = 0; i < length; i++) {
            names.Add($"{graphics[i].name}");
        }
    }

    private void OnDisable() {
        int length = graphics.Length;
        for (int i = 0; i < length; i++) {
            graphics[i].name = names[i];
        }
    }

    private void Update() {
        int length = graphics.Length;
        for (int i = 0; i < length; i++) {
            if (graphics[i] != null) {
                int v = Convert.ToInt32(graphics[i].hasRectClipping);
                graphics[i].name = $"{graphics[i].absoluteDepth}|{v}|{names[i]}";
            }
        }
    }
}
#endif