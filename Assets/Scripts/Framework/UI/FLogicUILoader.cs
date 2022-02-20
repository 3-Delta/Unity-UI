using System;
using UnityEngine;
using UnityEngine.UI;

// 方便美术快速预览UI效果
// 以及快速关闭UI
[DisallowMultipleComponent]
public class FLogicUILoader : MonoBehaviour {
    public int uiType;
    public Button btnExit;
    
    private void Awake() {
        if (btnExit != null) {
            btnExit.onClick.AddListener(Close);
        }
    }

#if UNITY_EDITOR
    public Vector3 arg;
    
    // 快速开启
    [ContextMenu(nameof(Open))]
    public void Open() {
        FUIMgr.Open(uiType, new Tuple<ulong, ulong, ulong, object>((ulong)arg.x, (ulong)arg.y, (ulong)arg.z, null));
    }
#endif

    // 快速关闭
    [ContextMenu(nameof(Close))]
    public void Close() {
        FUIMgr.Close(uiType);
    }
}
