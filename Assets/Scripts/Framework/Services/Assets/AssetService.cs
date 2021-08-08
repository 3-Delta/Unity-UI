using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum EAssetLoadType {
    Editor,
    Device, // editor下就使用editor下的ab加载
}

public class AssetService {
    public static EAssetLoadType loadType = EAssetLoadType.Device;

    public static RequestAsset Load(string assetPath) {
        RequestAsset request = new RequestAsset();
        request.Load();
        return request;
    }

    public static RequestAsset LoadAsync(string assetPath, Action<RequestAsset> onLoaded) {
        RequestAsset request = new RequestAsset();
        request.LoadAsync(onLoaded);
        return request;
    }

    public static void Unload(RequestAsset request) {
        request?.Unload();
    }
}
