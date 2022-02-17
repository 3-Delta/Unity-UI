using UnityEngine;
using UnityEngine.UI;

// todo 目前处理静态节点，动态加载的节点后续处理
[DisallowMultipleComponent]
[RequireComponent(typeof(Mask))]
public class MaskStencilInverter : MonoBehaviour {
    [SerializeField] private Graphic[] graphics = new Graphic[0];

    private Graphic _selfGraphic;

    public Graphic SelfGraphic {
        get {
            if (_selfGraphic == null) {
                _selfGraphic = GetComponent<Graphic>();
            }

            return _selfGraphic;
        }
    }

    private void Awake() {
        ReCollect();
    }

    // 动态挂载使用
    public void ReCollect() {
        graphics = GetComponentsInChildren<Graphic>();
    }

    // 外部调用
    public void Set(bool invert) {
        UnityEngine.Rendering.CompareFunction comp = invert ? UnityEngine.Rendering.CompareFunction.NotEqual : UnityEngine.Rendering.CompareFunction.Equal;
        foreach (var item in graphics) {
            if (SelfGraphic != item) {
                item.materialForRendering.SetInt("_StencilComp", (int) comp);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        ReCollect();
    }
#endif
}
