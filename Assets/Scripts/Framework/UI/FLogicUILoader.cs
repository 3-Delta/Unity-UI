using System;
using UnityEngine;

#if UNITY_EDITOR
// 方便美术快速预览UI效果
// 以及快速关闭UI
[DisallowMultipleComponent]
public class FLogicUILoader : MonoBehaviour {
    public int uiType;
    public Vector3 arg;

    // 快速开启
    [ContextMenu(nameof(Open))]
    public void Open() {
        FUIMgr.Open(uiType, new Tuple<ulong, ulong, ulong, object>((ulong)arg.x, (ulong)arg.y, (ulong)arg.z, null));
    }

    // 快速关闭
    [ContextMenu(nameof(Close))]
    public void Close() {
        FUIMgr.Close(uiType);
    }
    
    // 快速刷新
    [ContextMenu(nameof(Show))]
    public void Show() {
        FUIMgr.Show(uiType);
    }
}

#endif
