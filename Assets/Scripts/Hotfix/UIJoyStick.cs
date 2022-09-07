using System;
using UnityEngine;

// 其实是JoyStick套了层壳
[DisallowMultipleComponent]
public class UIJoyStick : MonoBehaviour {
    [Serializable]
    public class Joy {
        public Transform root;
        public Transform arrow;
        
        public void ActiveJoy(bool toActive) {
            if (root) {
                root.gameObject.SetActive(toActive);
            }
        }

        public void SetRotation() {
            
        }
    }
    
    public Joy joy;
    public JoyStick joyStick;

    private void OnEnable() {
        joyStick.onTouchDown += this._OnTouchDown;
        joyStick.onTouchUp += this._OnTouchUp;
        joyStick.onCtrl += this._OnCtrl;
    }

    private void OnDisable() {
        joyStick.onTouchDown -= this._OnTouchDown;
        joyStick.onTouchUp -= this._OnTouchUp;
        joyStick.onCtrl -= this._OnCtrl;
    }

    // 显隐UI遥感
    

    private void _OnTouchDown(Vector2 pos) {
        joy?.ActiveJoy(true);
    }

    private void _OnTouchUp(Vector2 pos) {
        joy?.ActiveJoy(false);
    }

    private void _OnCtrl(OpInput input) { }
}
