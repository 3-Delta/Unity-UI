using UnityEngine;

#if UNITY_EDITOR
[DisallowMultipleComponent]
public class NWGuarder : MonoBehaviour {
    [SerializeField] private bool isConnecting;
    [SerializeField] private bool isConnected;

    private void Awake() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

    private void Update() {
        isConnected = NWMgr.Instance.transfer.IsConnected;
        isConnecting = NWMgr.Instance.transfer.IsConnecting;
    }
}
#endif
