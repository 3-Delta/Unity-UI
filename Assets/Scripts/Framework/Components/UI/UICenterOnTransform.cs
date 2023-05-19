using UnityEngine;

[DisallowMultipleComponent]
public class UICenterOnTransform : MonoBehaviour {
    public UICenterOnChild centerOn;

    public bool useScale = true;
    public float scaleMultiply = 1f;

    public bool usePosition = true;
    public float positionMultiply = 200f;
    public float positionOffset = -100f;

    private void OnEnable() {
        centerOn.onTransform += OnTransform;
    }

    private void OnDisable() {
        centerOn.onTransform -= OnTransform;
    }

    protected virtual void OnTransform(bool hOrV, int index, Transform tr, float toMiddle, Vector3 srCenterOnCentent) {
        if (useScale) {
            float scale = Mathf.Clamp01(1 - toMiddle) * scaleMultiply;
            tr.localScale = new Vector3(scale, scale, scale);
        }

        if (usePosition) {
            var oldPos = tr.localPosition;
            if (hOrV) {
                tr.localPosition = new Vector3(oldPos.x, srCenterOnCentent.y + toMiddle * positionMultiply + positionOffset, oldPos.z);
            }
            else {
                tr.localPosition = new Vector3(srCenterOnCentent.x + toMiddle * positionMultiply + positionOffset, oldPos.y, oldPos.z);
            }
        }
    }
}