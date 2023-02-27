using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FBridge : MonoBehaviour {
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
        _hotfixInit = AssemblyProxy.CreateStaticMethod("Logic.Hotfix.Fixed.HotfixBridge", "Init", 0);

        // 给热更层传递值类型参数，可以使用BeginInvoke的形式，避免装箱
        // _playAudio = AssemblyProxy.CreateStaticMethod("Logic.Hotfix.AudioMgr", "Play", 1);
        // _stopAudio = AssemblyProxy.CreateStaticMethod("Logic.Hotfix.AudioMgr", "Stop", 1);

        _hasInited = true;
    }

    public static void HotfixInit() {
        if (!_hasInited) {
            return;
        }

#if __NATIVE__ // 如果这里编译报错肯定是Logic.Hotfix.dll不存在，或者没有用vs重新编译Logic.Hotfix到Unity中
        // 注意用vs编译Logic.Hotfix会依赖unity工程，如果此时unity工程有编译错误那么就不会编译Logic.Hotfix
        Logic.Hotfix.Fixed.HotfixBridge.Init();
#elif __REFL_RELOAD__ && UNITY_EDITOR
        _hotfixInit?.Exec();
#elif __REFL__
        _hotfixInit?.Exec();
#elif __ILR__
        _hotfixInit?.Exec();
#endif
    }
    
    public static void Dispose() {
        _hotfixInit = null;
        _playAudio = null;
        _stopAudio = null;

        _hasInited = false;
    }
}
