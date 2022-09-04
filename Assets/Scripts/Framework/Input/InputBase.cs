using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 输入
[DisallowMultipleComponent]
public abstract class InputBase : MonoBehaviour {
    public abstract void OnKeyCode();
    public abstract void OnTouchUp();
    public abstract void OnTouchDown();
    
    public abstract void OnDragStart();
    public abstract void OnDragging();
    public abstract void OnDragEnd();
}
