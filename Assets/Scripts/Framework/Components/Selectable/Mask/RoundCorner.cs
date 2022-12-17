using UnityEngine;
using UnityEngine.UI;

// https://github.com/kirevdokimov/Unity-UI-Rounded-Corners
[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(MaskableGraphic))]
public class RoundCorner : MonoBehaviour {
    public static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");

    public float radius = 90f;
    public Material material;

    [HideInInspector, SerializeField] private MaskableGraphic graphic;

    private void OnValidate() {
        Validate();
        Refresh();
    }

    private void OnDestroy() {
        Destroy(material);
        this.graphic = null;
        material = null;
    }

    private void OnEnable() {
        Validate();
        Refresh();
    }

    private void OnRectTransformDimensionsChange() {
        if (enabled && material != null) {
            Refresh();
        }
    }

    public void Validate() {
        if (material == null) {
            material = new Material(Shader.Find("UI/Default"));
        }

        if (this.graphic == null) {
            TryGetComponent(out this.graphic);
        }

        if (this.graphic != null) {
            this.graphic.material = material;
        }
    }

    public void Refresh() {
        var rect = ((RectTransform)transform).rect;
        material.SetVector(Props, new Vector4(rect.width, rect.height, radius, 0));
    }

    public new static void Destroy(Object target) {
#if UNITY_EDITOR
		if (Application.isPlaying) {
			Object.Destroy(target);
		} 
        else {
			Object.DestroyImmediate(target);
		}
#else
        Object.Destroy(target);
#endif
    }
}
