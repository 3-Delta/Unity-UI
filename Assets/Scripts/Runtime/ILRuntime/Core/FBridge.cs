using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FBridge : MonoBehaviour {
    public static EAssemblyLoadType assemblyLoadType = EAssemblyLoadType.ByNative;

    private static StaticMethod _hotfixInit;

    private static StaticMethod _playAudio;
    private static StaticMethod _stopAudio;

    private static bool _hasInited = false;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy() {
        Dispose();
    }

    public static void Init() {
        _hotfixInit = AssemblyProxy.CreateStaticMethod("HotfixBridge", "Init", 0);

        // 给热更层传递值类型参数，可以使用BeginInvoke的形式，避免装箱
        // _playAudio = AssemblyProxy.CreateStaticMethod("Logic.Hotfix.AudioMgr", "Play", 1);
        // _stopAudio = AssemblyProxy.CreateStaticMethod("Logic.Hotfix.AudioMgr", "Stop", 1);

        _hasInited = true;
    }

    public static void HotfixInit() {
        if (!_hasInited) {
            return;
        }

        if (assemblyLoadType == EAssemblyLoadType.ByNative) {
            HotfixBridge.Init();
        }
        else {
            _hotfixInit?.Exec();
        }
    }

    public static void Dispose() {
        _hotfixInit = null;
        _playAudio = null;
        _stopAudio = null;

        _hasInited = false;
    }
}
