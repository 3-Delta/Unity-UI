using UnityEngine;

#if UNITY_EDITOR
[DisallowMultipleComponent]
public class TimeScaler : MonoBehaviour {
    private void Awake() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

    private void Update() {
        if (Input.anyKeyDown) {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                Time.timeScale = 4f;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                Time.timeScale = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                Time.timeScale = 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                Time.timeScale = 10f;
            }
        }
    }
}
#endif
