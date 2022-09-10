using UnityEngine;
using UnityEngine.UI;

public class TestActive : MonoBehaviour {
    public Component hookedGameObject;
    public bool enableOrActive = false;

    public Button Show;
    public Button Hide;

    void Start() {
        Show.onClick.RemoveAllListeners();
        Hide.onClick.RemoveAllListeners();

        Show.onClick.AddListener(() => {
            if (!enableOrActive) {
                hookedGameObject.gameObject?.SetActive(true);
            }
            else {
                (hookedGameObject as Behaviour).enabled = true;
            }
        });

        Hide.onClick.AddListener(() => {
            if (!enableOrActive) {
                hookedGameObject.gameObject?.SetActive(false);
            }
            else {
                (hookedGameObject as Behaviour).enabled = false;
            }
        });
    }
}
