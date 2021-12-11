using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class CameraFrustum : MonoBehaviour {
    public new Camera camera = null;

#if UNITY_EDITOR
    private void DrawGizmos() {
        if (camera != null) {
            UnityEditor.CameraEditorUtils.DrawFrustumGizmo(camera);

            //Matrix4x4 lastMatrix = Gizmos.matrix;
            //Color lastColor = Gizmos.color;

            //Gizmos.matrix = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, camera.transform.lossyScale);
            //Gizmos.color = Color.yellow;

            //if(camera.orthographic)
            //{
            //    float gap = camera.farClipPlane - camera.nearClipPlane;
            //    Vector3 size = new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, gap);
            //    Gizmos.DrawWireCube(Vector3.zero + new Vector3(0, 0, camera.farClipPlane / 2), size);
            //}
            //else
            //{
            //    Gizmos.DrawFrustum(camera.transform.position, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
            //}

            //Gizmos.matrix = lastMatrix;
            //Gizmos.color = lastColor;
        }
    }
#endif
}
