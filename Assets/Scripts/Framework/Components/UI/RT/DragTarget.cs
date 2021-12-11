using UnityEngine;
using UnityEngine.UI;

// rt上添加该脚本，支持拖拽模型
[DisallowMultipleComponent]
[RequireComponent(typeof(Graphic))]
public class DragTarget : MonoBehaviour {
    public UIEventTrigger eventTrigger;

    public bool xAxis = true;
    public bool yAxis = false;
    public float speed = 1f;

    public Transform target;

    private void Awake() {
        if (!gameObject.TryGetComponent(out eventTrigger)) {
            eventTrigger = gameObject.AddComponent<UIEventTrigger>();
        }

        eventTrigger.onDrag += OnDrag;
    }

    private void OnDestroy() {
        eventTrigger.onDrag -= OnDrag;
    }

    private void OnDrag(GameObject go, Vector2 delta) {
        if (target != null) {
            var euler = new Vector3(yAxis ? delta.y : 0f, xAxis ? -delta.x : 0f, 0f) * speed;

            // 方案1：
            // 单纯修改角度, 受到 万向锁 的影响
            // target.transform.localEulerAngles += euler;

            // 方案2：
            // 不会受到万向锁的影响, 但是始终绕着local坐标系旋转
            // target.transform.Rotate(euler, Space.Self);

            // 方案3：
            // 不会受到万向锁的影响, 类似unity的模型预览面板一样
            target.transform.Rotate(euler, Space.World);
        }
    }
}
