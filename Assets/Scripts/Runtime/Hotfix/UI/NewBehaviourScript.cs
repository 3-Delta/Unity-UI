using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    private void Start() {
        UIMgr.Init();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            UIMgr.Open(EUIType.UILogin, null);
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            UIMgr.Close(EUIType.UILogin);
        }
    }
}
