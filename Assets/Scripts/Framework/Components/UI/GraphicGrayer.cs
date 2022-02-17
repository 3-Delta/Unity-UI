using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GraphicGrayer : MonoBehaviour {
    public Material grayMaterial;

    private Dictionary<Graphic, Material> dict = new Dictionary<Graphic, Material>();

    private void Awake() {
        Collect();
    }

    public void Collect() {
        // 收集原始材质
        dict.Clear();
        var ls = GetComponentsInChildren<Graphic>();
        foreach (var item in ls) {
            dict[item] = item.material;
        }
    }

    public void Gray(bool toGray) {
        if (toGray) {
            foreach (var kvp in dict) {
                kvp.Key.material = grayMaterial;
            }
        }
        else {
            foreach (var kvp in dict) {
                kvp.Key.material = kvp.Value;
            }
        }
    }
}
