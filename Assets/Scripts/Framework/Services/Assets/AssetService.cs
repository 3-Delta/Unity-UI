using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum EAssetLoadType {
    FromAssetBundle,
    FromAssetDatabase,
    FromResources,
}

public class AssetService {
    public static EAssetLoadType loadType = EAssetLoadType.FromAssetBundle;

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
