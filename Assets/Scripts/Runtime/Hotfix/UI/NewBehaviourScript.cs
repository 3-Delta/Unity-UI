using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    private void Start() {
        UIMgr.Init();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            UIMgr.Open(EUIType.UILogin, null);
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            UIMgr.Close(EUIType.UILogin);
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            UIMgr.Show(EUIType.UILogin);
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            UIMgr.Hide(EUIType.UILogin);
        }
        else if (Input.GetKeyDown(KeyCode.T)) {
            UIMgr.Open(EUIType.UILogin, null);
            UIMgr.Close(EUIType.UILogin);
        }
        else if (Input.GetKeyDown(KeyCode.Y)) {
            UIMgr.Open(EUIType.UILogin, null);
            UIMgr.Hide(EUIType.UILogin);
        }
    }
}
