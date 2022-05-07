using UnityEngine;

// ref: https://github.com/3-Delta/Buff-In-TopDownShooter
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {
    // 镜头要跟随的角色，没有角色镜头就停止跟随了
    public GameObject followTarget;

    //镜头跟随的偏移
    private Vector3 offset;

    private void LateUpdate() {
        if (!this.followTarget) {
            return;
        }
        this.transform.position = this.offset + this.followTarget.transform.position;
    }

    public void SetTarget(GameObject target) {
        this.followTarget = target;
        var position = transform.position;
        var targetPosition = target.transform.position;
        this.offset = new Vector3(position.x - targetPosition.x, 
            -position.z / Mathf.Cos(transform.rotation.eulerAngles.x * Mathf.PI / 180) - targetPosition.y, 
            position.z - targetPosition.z);
    }
}
