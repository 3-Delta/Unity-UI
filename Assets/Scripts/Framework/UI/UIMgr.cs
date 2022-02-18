using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStack {
    private List<FUIBase> uiList = new List<FUIBase>();

    protected virtual FUIBase CreateInstance(UIEntry cfg) {
        return Activator.CreateInstance(cfg.script) as FUIBase;
    }

    protected virtual bool GetOrCreate(int uiType, UIEntry cfg, out FUIBase ui) {
        if (!UIMgr.uiDict.TryGetValue(uiType, out ui)) {
            if (cfg == null) {
                return false;
            }

            ui = CreateInstance(cfg);
            if (ui == null) {
                return false;
            }
            
            UIMgr.uiDict.Add(uiType, ui);
            uiList.Add(ui);
            ui.Init(uiType, cfg);
        }

        return true;
    }
    

    public void PreLoad(int uiType) { }
    public void Open(int uiType, UIEntry entry) { }
    public void Close(int uiType) { }
}

public class UIMgr {
    public readonly static Dictionary<int, FUIBase> uiDict = new Dictionary<int, FUIBase>();
    public readonly static Dictionary<EUILayer, UIStack> stacks = new Dictionary<EUILayer, UIStack>(3);

    public static void PreLoad(int uiType) {
        if (FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            stacks[cfg.layer].PreLoad(uiType);
        }
    }

    public static void Open(int uiType, Tuple<ulong, ulong, ulong, ulong> arg) {
        if (FUIEntryRegistry.TryGet(uiType, out var entry)) {
            stacks[entry.layer].Open(uiType, entry);
        }
    }

    public static void Close(int uiType) {
        if (FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            stacks[cfg.layer].Close(uiType);
        }
    }

    public static void Show(int uiType) { }
    public static void Hide(int uiType) { }

    public static bool IsOpen(int uiType) {
        return true;
    }

    public static bool IsTop(int uiType) {
        return true;
    }
}
