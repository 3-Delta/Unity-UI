#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class UGUIGraphicDepth : MonoBehaviour {
    private Graphic[] graphics;
    [SerializeField] private string[] names;

    private void Awake() {
        hideFlags = HideFlags.DontSaveInBuild;

        graphics = GetComponentsInChildren<Graphic>(true);
        int length = graphics.Length;
        names = new String[length];
        for (int i = 0; i < length; i++) {
            names[i] = $"{graphics[i].name}";
        }
    }

    private void Update() {
        int length = graphics.Length;
        for (int i = 0; i < length; i++) {
            if (!graphics[i].IsDestroyed() && graphics[i].enabled) {
                graphics[i].name = $"{names[i]}->{graphics[i].depth}";
            }
        }
    }
}
#endif