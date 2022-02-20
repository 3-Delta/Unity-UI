using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FUICloser : MonoBehaviour
{
    public int uiType;
    public Button btnExit;
    
    private void Awake() {
        if (btnExit != null) {
            btnExit.onClick.AddListener(Close);
        }
    }
    
    private void Close() {
        FUIMgr.Close(uiType);
    }
}
