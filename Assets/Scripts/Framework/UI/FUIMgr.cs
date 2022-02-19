using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FUIStack {
    protected readonly List<FUIBase> stack = new List<FUIBase>();

    protected virtual FUIBase CreateInstance(FUIEntry cfg) {
        return Activator.CreateInstance(cfg.ui) as FUIBase;
    }

    protected virtual bool GetOrCreate(int uiType, FUIEntry cfg, out FUIBase ui) {
        if (!FUIMgr.uiDict.TryGetValue(uiType, out ui)) {
            if (cfg == null) {
                return false;
            }

            ui = CreateInstance(cfg);
            if (ui == null) {
                return false;
            }

            ui.Init(uiType, cfg);
            FUIMgr.uiDict.Add(uiType, ui);
            stack.Add(ui);
        }

        return true;
    }

    public void Open(int uiType, FUIEntry cfg, Tuple<ulong, ulong, ulong, object> arg) {
        if (GetOrCreate(uiType, cfg, out FUIBase ui)) {
            ui.Open(arg);
        }
    }

    public void Close(int uiType) { }
}

public class FUIMgr {
    public readonly static Dictionary<int, FUIBase> uiDict = new Dictionary<int, FUIBase>();
    public readonly static Dictionary<EUILayer, FUIStack> stacks = new Dictionary<EUILayer, FUIStack>(3);

    public static Transform uiParent { get; private set; }
    // public static Camera camera2d { get; private set; }

    public static void Init() {
        uiParent = new GameObject("UI").transform;
    }
    
    public static void PreLoad(int uiType) { }

    public static void Open(int uiType, Tuple<ulong, ulong, ulong, object> arg) {
        if (FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            if (!stacks.TryGetValue(cfg.layer, out FUIStack stack)) {
                stack = new FUIStack();
                stacks.Add(cfg.layer, stack);
            }

            stack.Open(uiType, cfg, arg);
        }
    }

    public static void Close(int uiType) {
        if (FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            if (!stacks.TryGetValue(cfg.layer, out FUIStack stack)) {
                stack = new FUIStack();
                stacks.Add(cfg.layer, stack);
            }

            stack.Close(uiType);
        }
    }

    public static void Show(int uiType) { }
    public static void Hide(int uiType) { }
}
