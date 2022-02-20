using UnityEngine;

public class UILauncherTest : MonoBehaviour {
    private void Start() {
        UIMgr.Init();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            UIMgr.Open(EUIType.UIMain, null);
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            UIMgr.Close(EUIType.UIMain);
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            UIMgr.Show(EUIType.UIMain);
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            UIMgr.Hide(EUIType.UIMain);
        }
        else if (Input.GetKeyDown(KeyCode.T)) {
            UIMgr.Open(EUIType.UIMain, null);
            UIMgr.Close(EUIType.UIMain);
        }
        else if (Input.GetKeyDown(KeyCode.Y)) {
            UIMgr.Open(EUIType.UIMain, null);
            UIMgr.Hide(EUIType.UIMain);
        }
        else if (Input.GetKeyDown(KeyCode.U)) {
            UIMgr.Open(EUIType.UIMain, null);
            UIMgr.Open(EUIType.UIMain, null);
        }
    }
}
