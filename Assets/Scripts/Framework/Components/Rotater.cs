using UnityEngine;

[DisallowMultipleComponent]
public class Rotater : MonoBehaviour {
    [Header("控制x/y/z轴是否需要旋转 false 0 | true 1")]
    public Vector3 axis = new(0f, 0f, 1f);
    public Transform target;
    public float speed = 100f;

    private Vector3 currentEulerAngles;

    private void Awake() {
        if (target == null) {
            target = transform;
        }

        currentEulerAngles = target.transform.localEulerAngles;
    }

    private void Update() {
        currentEulerAngles += Time.deltaTime * speed * axis;
        target.localEulerAngles = currentEulerAngles;
    }
}
