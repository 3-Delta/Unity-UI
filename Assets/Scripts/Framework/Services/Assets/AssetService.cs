using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAssetLoadType { 
    // 编辑器非AB同步加载
    EditorSync,
    // 编辑器非AB异步加载
    EditorAsync,

    // 编辑器AB同步加载
    EditorABSync,
    // 编辑器AB异步加载
    EditorABAsync,

    // 真机AB同步加载
    DeviceABSync,
    // 真机AB异步加载
    DeviceABAsync,
}

public class AssetService : MonoBehaviour
{
    public static AssetRequest Load(string assetPath) {
        return null;
    }

    public static AssetRequest LoadAsync(string assetPath, Action<AssetRequest> onLoaded) {
        return null;
    }
}
