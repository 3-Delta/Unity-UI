using UnityEngine;

// https://answer.uwa4d.com/question/60acab5d6bb31032f979189d
public class CameraUtility {
    public static void World2UI(GameObject go, Transform worldTransform, Camera c3, Camera c2) {
        if (worldTransform != null) {
            World2UI(go, worldTransform.position, c3, c2);
        }
    }

    public static void World2UI(GameObject go, Vector3 worldPosition, Camera c3, Camera c2) {
        if (c3 != null) {
            Vector3 screenPosition = c3.WorldToScreenPoint(worldPosition);
            // 相机背面的物体
            if (screenPosition.z < 0) {
                screenPosition.x = -screenPosition.x;
                screenPosition.y = -screenPosition.y;
            }
            Screen2UI(go, screenPosition, c2);
        }
    }

    public static void Screen2UI(GameObject go, Vector3 screenPosition, Camera c2) {
        if (go != null && c2 != null) {
            Vector3 uiWorldPosition = c2.ScreenToWorldPoint(screenPosition);
            go.transform.position = new Vector3(uiWorldPosition.x, uiWorldPosition.y, 0f);
        }
    }

    // https://blog.uwa4d.com/archives/TechSharing_120.html
    // 裁减矩阵判断某个worldPosition是否在相机裁剪区域之内
    // 只是考虑了中心点，没有考虑size的问题
    public static bool IsInView(Camera camera, Transform t) {
        bool ret = false;
        if (t != null) {
            ret = IsInView(camera, t.position);
        }

        return ret;
    }

    public static bool IsInView(Camera camera, Vector3 worldPosition) {
        bool ret = false;
        if (camera != null) {
            // 矩阵将worldPosition转换为裁减空间的数值，因为是点，不是方向，则w == 1

            // 经过测试发现一般情况下：camera.cullingMatrix == (camera.projectionMatrix * camera.worldToCameraMatrix)
            // 某些情况下不同，chatgpt解释
            // vertex shader将定点从local转换到clip空间，然后裁剪空间到屏幕空间的转换是unity自己完成的，shader不需要处理这部分逻辑
            Matrix4x4 matrix = camera.cullingMatrix;
            Vector4 v = worldPosition;
            v.w = 1;
            Vector4 p = matrix * v;
            if (-p.w <= p.x && p.x <= p.w && -p.w <= p.y && p.y <= p.w && -p.w <= p.z && p.z <= p.w) {
                ret = true;
            }
        }

        return ret;
    }

    public static bool IsOutView(Camera camera, Transform t) {
        // https://blog.csdn.net/cyf649669121/article/details/86484168 
        bool isOut = false;
        var pos = camera.WorldToViewportPoint(t.position);
        // 只是以中心点作为基准判定，没有考虑size，可能存在问题
        if (pos.x < 0 || pos.y < 0 || pos.x > 1 || pos.y > 1) {
            isOut = true;
        }

        return isOut;
    }
}
