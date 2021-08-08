using System;
using System.IO;

using UnityEngine;

/// <summary>
/// 屏幕截图
/// http://www.unity.5helpyou.com/2730.html
/// </summary>
public class SnapShotService {
    // 优点：简单，可以快速地截取某一帧的画面、全屏截图
    // 缺点：不能针对摄像机截图，无法进行局部截图
    public static void CaptureFullScreen(string path) {
        if (!string.IsNullOrEmpty(path)) {
            ScreenCapture.CaptureScreenshot(path, 0);
        }
    }

    // 根据一个Rect类型来截取指定范围的屏幕
    // 左下角为(0,0)
    // 等待渲染帧结束时调用： yield return new WaitForEndOfFrame();
    public static Texture2D CaptureRect(string path, Rect rect) {
        Texture2D result = null;
        if (!string.IsNullOrEmpty(path)) {
            // 初始化Texture2D
            result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            // 读取屏幕像素信息并存储为纹理数据
            result.ReadPixels(rect, 0, 0);
            // 应用
            result.Apply();

            // 将图片信息编码为字节信息
            byte[] bytes = result.EncodeToPNG();
            // 保存
            File.WriteAllBytes(path, bytes);
        }
        return result;
    }

    public static Texture2D CaptureRect(string path) {
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        return CaptureRect(path, rect);
    }

    public static Texture2D CaptureCamera(string path, Camera camera, Rect rect) {
        Texture2D result = null;
        if (!string.IsNullOrEmpty(path) && camera != null) {
            // 初始化RenderTexture
            RenderTexture rt = RenderTexture.GetTemporary((int)rect.width, (int)rect.height, 0);
            // 设置相机的渲染目标
            camera.targetTexture = rt;
            //开始渲染
            camera.Render();

            //激活渲染贴图读取信息
            RenderTexture.active = rt;
            result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            //读取屏幕像素信息并存储为纹理数据
            result.ReadPixels(rect, 0, 0);
            //应用
            result.Apply();

            //释放相机，销毁渲染贴图
            camera.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            rt = null;

            //将图片信息编码为字节信息
            byte[] bytes = result.EncodeToPNG();
            //保存
            File.WriteAllBytes(path, bytes);
        }
        return result;
    }

    public static Texture2D CaptureCamera(Camera camera, string path) {
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        return CaptureCamera(camera, path, rect);
    }

    public static Texture2D CaptureCamera(Camera camera, string path, Rect rect) {
        return CaptureCamera(camera, path, rect);
    }
}
