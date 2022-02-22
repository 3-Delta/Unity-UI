using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FUICloser : MonoBehaviour {
    public int uiType;
    public Button btnExit;
    
    public Action onClose;

    private void Awake() {
        if (btnExit != null) {
            btnExit.onClick.AddListener(Close);
        }
    }

    private void Close() {
        onClose?.Invoke();
        FUIMgr.Close(uiType);
    }
}
