using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 整个工程的入口
[DisallowMultipleComponent]
public class App : MonoBehaviour {
    public static App instance { get; private set; } = null;

#if UNITY_EDITOR
    public EAssemblyLoadType assemblyLoadType = EAssemblyLoadType.ByNative;
    public EAssetLoadType assetLoadType = EAssetLoadType.FromResources;

    private void OnValidate() {
        AssemblyProxy.assemblyLoadType = assemblyLoadType;
        FBridge.assemblyLoadType = assemblyLoadType;
        AssetService.loadType = assetLoadType;
    }
#endif

    private void Awake() {
        instance = this;
    }

    private void OnDestroy() {
        instance = null;
    }

    private void Start() {
        if (AssemblyProxy.TryInit()) {
            FBridge.HotfixInit();
        }
    }
}
