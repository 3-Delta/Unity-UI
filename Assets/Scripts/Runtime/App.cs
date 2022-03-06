// using System;
using System.Threading;
using ILRuntime.Runtime.Enviorment;
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
        
        // 未捕获异常
        System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Debug.LogError(e.ExceptionObject.ToString());
        };
        SynchronizationContext.SetSynchronizationContext(UnitySynchronizationContext.UnitySyncContext);
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
