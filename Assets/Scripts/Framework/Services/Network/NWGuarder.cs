using UnityEngine;

#if UNITY_EDITOR
[DisallowMultipleComponent]
public class NWGuarder : MonoBehaviour {
    [SerializeField] private NWTransfer transfer;

    private void Awake() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

    private void Update() {
        transfer = NWMgr.Instance.transfer;
    }
}
#endif
