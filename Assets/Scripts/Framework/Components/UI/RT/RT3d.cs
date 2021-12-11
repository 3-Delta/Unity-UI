using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class RT3d : MonoBehaviour {
    public GameObject modelGo { get; protected set; }

    public new Camera camera { get; protected set; }
    public Transform pos { get; protected set; }

    public RenderTexture rt { get; protected set; }

    #region 上次设置
    protected int width;
    protected int height;
    protected int depthBuffer;
    protected RenderTextureFormat format = RenderTextureFormat.Default;
    #endregion

    public RT3d Parse(GameObject scene) {
        this.modelGo = scene;
        this.camera = scene.transform.Find("Camera").GetComponent<Camera>();
        this.pos = scene.transform.Find("Pos");
        return this;
    }

    private void OnDestroy() {
        Clear();
    }

    public void Clear() {
        ReleaseCameraRT();
        ReleaseRT();

        camera = null;
        pos = null;

        if (modelGo != null) {
            GameObject.Destroy(modelGo);
            modelGo = null;
        }
    }

    // 支持拖拽?
    public void GetRT(RawImage rawImage, bool supportOp, bool useXAxis, bool useYAxis, int width, int height,
        int depthBuffer,
        RenderTextureFormat format,
        float scale) {
        if (rawImage != null) {
            rawImage.texture = GetRT(width, height, depthBuffer, format, scale);

            if (supportOp) {
                if (!rawImage.TryGetComponent<DragTarget>(out DragTarget dt)) {
                    dt = rawImage.gameObject.AddComponent<DragTarget>();
                }

                dt.target = modelGo.transform;
                dt.xAxis = useXAxis;
                dt.yAxis = useYAxis;
            }
        }
    }

    public RenderTexture GetRT(int width, int height, int depthBuffer, RenderTextureFormat format, float scale) {
        width = width > 0 ? width : Screen.width;
        height = height > 0 ? height : Screen.height;

        width = (int)(width * scale);
        height = (int)(height * scale);

        // 和已有的参数不匹配，直接Release
        if (width != this.width || height != this.height || format != this.format || depthBuffer != this.depthBuffer) {
            ReleaseRT();

            this.width = width;
            this.height = height;
            this.format = format;
            this.depthBuffer = depthBuffer;

            rt = RenderTexture.GetTemporary(width, height, depthBuffer, format);
            camera.targetTexture = rt;
        }

        return rt;
    }

    public void ReleaseCameraRT() {
        if (camera != null) {
            camera.targetTexture = null;
        }
    }

    public void ReleaseRT() {
        if (rt != null) {
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }
    }
}
