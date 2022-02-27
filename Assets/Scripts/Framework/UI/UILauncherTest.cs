using UnityEngine;

public class UILauncherTest : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            FUIMgr.Open(1, null);
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            FUIMgr.Close(1);
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            FUIMgr.Show(1);
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            FUIMgr.Hide(1);
        }
        else if (Input.GetKeyDown(KeyCode.T)) {
            FUIMgr.Open(1, null);
            FUIMgr.Close(1);
        }
        else if (Input.GetKeyDown(KeyCode.Y)) {
            FUIMgr.Open(1, null);
            FUIMgr.Hide(1);
        }
        else if (Input.GetKeyDown(KeyCode.U)) {
            FUIMgr.Open(1, null);
            FUIMgr.Open(1, null);
        }
    }
}
