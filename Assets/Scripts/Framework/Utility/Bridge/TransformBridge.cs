using UnityEngine;

// 可以热更调用，也可以框架层内部调用
public class TransformBridge {
    public static Transform CreateTransform(string name, Transform parent) {
        Transform rlt = new GameObject(name).transform;
        rlt.SetParent(parent);
        ResetTransform(rlt);
        return rlt;
    }

    public static void ResetTransform(Transform transform) {
        if (transform != null) {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }

    public static void SetLocalPosition(Transform transform, float x = 0f, float y = 0f, float z = 0f) {
        if (transform != null) {
            transform.localPosition = new Vector3(x, y, z);
        }
    }

    public static void SetLocalPosition(Transform transform, Vector3 pos) {
        if (transform != null) {
            transform.localPosition = pos;
        }
    }

    public static void SetPosition(Transform transform, float x = 0f, float y = 0f, float z = 0f) {
        if (transform != null) {
            transform.position = new Vector3(x, y, z);
        }
    }

    public static void SetPosition(Transform transform, Vector3 pos) {
        if (transform != null) {
            transform.position = pos;
        }
    }

    // 适用于UGUI
    public static void SetLocalPosition(RectTransform transform, float x = 0f, float y = 0f, float z = 0f) {
        if (transform != null) {
            transform.anchoredPosition3D = new Vector3(x, y, z);
        }
    }
}
