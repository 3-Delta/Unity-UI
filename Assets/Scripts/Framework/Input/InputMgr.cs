using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InputMgr : MonoBehaviour {
    private static InputMgr inputMgr = null;

    private void Reset() {
        inputMgr = this;
    }

    public Action<KeyCode> onKeyCode;
    public Action onTouchDown;
    public Action onTouchUp;

    public Action<Vector2> onDragStart;
    public Action<Vector2, Vector2> onDragging;
    public Action<Vector2, Vector2> onDragEnd;
}
