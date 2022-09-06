using UnityEngine;

[DisallowMultipleComponent]
public class JoyStick : MonoBehaviour {
    protected ClickRaycaster ray;
    
    private void Awake() {
        if (!gameObject.TryGetComponent<ClickRaycaster>(out ray)) {
            ray = gameObject.AddComponent<ClickRaycaster>();
        }
    }

    private void OnEnable() {
        if (ray) {
            ray.onClick += InputMgr.instance.onClick;
        }
    }

    private void OnDisable() {
        if (ray) {
            ray.onClick -= InputMgr.instance.onClick;
        }
    }
}
