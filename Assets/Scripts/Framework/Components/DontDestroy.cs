using UnityEngine;

[DisallowMultipleComponent]
public class DontDestroy : MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
