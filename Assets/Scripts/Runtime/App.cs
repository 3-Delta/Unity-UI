using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 整个工程的入口
[DisallowMultipleComponent]
public class App : MonoBehaviour {
    public static App instance { get; private set; } = null;

#if UNITY_EDITOR
    public EAssetLoadType assetLoadType = EAssetLoadType.FromResources;

    private void OnValidate() {
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
        AssemblyProxy.Init();
        StaticMethod buildMethod = AssemblyProxy.CreateStaticMethod("Bridge", "Init", 0);
        buildMethod.Exec();

        // Bridge.Init();
    }
}
