using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 刘海屏适配 -- 安全区域适配
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class SafeAreaAdjuster : MonoBehaviour {
    public Canvas canvas;
    public RectTransform left;
    public RectTransform right;
    public RectTransform up;
    public RectTransform down;

    public static readonly float DotWidth = 20f;
    
    [ContextMenu(nameof(ResetBorder))]
    public void ResetBorder() {
        var scrWidth = Screen.width;
        var scrHeight = Screen.height;
        var halfWidth = scrWidth / 2f;
        var halfHeight = scrHeight / 2f;

        this.left.anchoredPosition = new Vector2(-halfWidth, 0f);
        this.left.sizeDelta = new Vector2(DotWidth, scrHeight);
        
        this.right.anchoredPosition = new Vector2(halfWidth, 0f);
        this.right.sizeDelta = new Vector2(DotWidth, scrHeight);
        
        this.up.anchoredPosition = new Vector2(0f, halfHeight);
        this.up.sizeDelta = new Vector2(scrWidth, DotWidth);
        
        this.down.anchoredPosition = new Vector2(0, -halfHeight);
        this.down.sizeDelta = new Vector2(scrWidth, DotWidth);
    }

    [ContextMenu(nameof(ScreenWH))]
    public Vector2 ScreenWH() {
        var wh = new Vector2(Screen.width, Screen.height);
        Debug.LogError("ScreenWH: " + wh);
        return wh;
    }
}
