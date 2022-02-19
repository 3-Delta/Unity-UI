using System;
using UnityEngine;
using UnityEngine.UI;

// 分辨率适配 -- 不考虑安全区域
// https://blog.csdn.net/qq_39185012/article/details/106231691
// Expand或者Shrink都可以， 比如IPad和小米k40的适配就不同
[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasScaler))]
public class ResolutionAdjuster : MonoBehaviour {
    private void Start() {
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(GlobalSetting.ResolutionWidth, GlobalSetting.ResolutionHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }

    // private void Update() {
    //     float curRatio = 2f * Screen.width / Screen.height;
    // }
}
