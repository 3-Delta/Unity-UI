using UnityEngine;

[DisallowMultipleComponent]
public class TimeScaler : MonoBehaviour {
    private void Start() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

#if UNITY_EDITOR
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
#endif

    public void Set(float target) {
        Time.timeScale = target;
    }
}
