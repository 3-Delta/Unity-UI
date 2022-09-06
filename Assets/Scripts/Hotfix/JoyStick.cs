using UnityEngine;

[DisallowMultipleComponent]
public class JoyStick : MonoBehaviour {
    public ClickRaycaster joy;

    public void ActiveJoy(bool toEnable) {
        if (joy) {
            joy.gameObject.SetActive(toEnable);
        }
    }

    private void OnEnable() {
        if (joy) {
            joy.onClick += InputMgr.instance.OnClick;
        }
    }

    private void OnDisable() {
        if (joy) {
            joy.onClick -= InputMgr.instance.OnClick;
        }
    }
}
