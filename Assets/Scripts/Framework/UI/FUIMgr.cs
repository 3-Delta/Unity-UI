using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FUIStack {
    public static readonly int STACKGAP = 30;

#if UNITY_EDITOR
    public EUILayer layer;
    public List<FUIBase> stack = new List<FUIBase>();
#else
    public EUILayer layer { get; protected set; }
    protected readonly List<FUIBase> stack = new List<FUIBase>();
#endif

    public FUIStack(EUILayer layer) {
        this.layer = layer;
    }

    protected virtual FUIBase CreateInstance(FUIEntry cfg) {
        return Activator.CreateInstance(cfg.ui) as FUIBase;
    }

    protected virtual bool TryCreate(int uiType, FUIEntry cfg, out FUIBase ui) {
        if (!FUIMgr.TryGet(uiType, out ui)) {
            if (cfg == null) {
                return false;
            }

            ui = CreateInstance(cfg);
            if (ui == null) {
                return false;
            }
        }

        return true;
    }

    public void Open(int uiType, FUIEntry cfg, Tuple<ulong, ulong, ulong, object> arg) {
        if (!FUIMgr.TryGet(uiType, out FUIBase ui)) {
            if (TryCreate(uiType, cfg, out ui)) {
                FUIMgr.Add(uiType, ui);
                stack.Add(ui);

                ui.Init(uiType, cfg);
                ui.Open(arg);
                AdjustStack(ui);
            }
        }
        else {
            ui.Transfer(arg);
            AdjustStack(ui);
        }
    }

    public void Close(int uiType, FUIBase ui) {
        ui.Close();
        AdjustStack(ui);

        FUIMgr.Remove(uiType);
        stack.Remove(ui);
    }

    public void Show(int uiType, FUIBase ui) {
        ui.Show();

        AdjustStack(ui);
    }

    public void Hide(int uiType, FUIBase ui) {
        ui.Hide();

        AdjustStack(ui);
    }

    // 调整堆栈
    private void AdjustStack(FUIBase ui) {
        // ui.adapter.SetOrder(0);
    }
}

[Serializable]
public class FUIMgr {
#if UNITY_EDITOR
    public readonly static Dictionary<int, FUIBase> uiDict = new Dictionary<int, FUIBase>();
    public readonly static Dictionary<EUILayer, FUIStack> stacks = new Dictionary<EUILayer, FUIStack>(3);
#else
    private readonly static Dictionary<int, FUIBase> uiDict = new Dictionary<int, FUIBase>();
    private readonly static Dictionary<EUILayer, FUIStack> stacks = new Dictionary<EUILayer, FUIStack>(3);
#endif

    public static Transform uiParent { get; private set; }
    // public static Camera camera2d { get; private set; }

    public static void Init() {
        uiParent = new GameObject("__UI__").transform;
        GameObject.DontDestroyOnLoad(uiParent);

#if UNITY_EDITOR
        uiParent.gameObject.AddComponent<FUIGuarder>();
#endif
    }

    public static bool TryGet(int uiType, out FUIBase ui) {
        return uiDict.TryGetValue(uiType, out ui);
    }

    public static void Add(int uiType, FUIBase ui) {
        uiDict.Add(uiType, ui);
    }

    public static void Remove(int uiType) {
        uiDict.Remove(uiType);
    }

    public static void PreLoad(int uiType) { }

    public static void Open(int uiType, Tuple<ulong, ulong, ulong, object> arg = null) {
        if (FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            if (!stacks.TryGetValue(cfg.layer, out FUIStack stack)) {
                stack = new FUIStack(cfg.layer);
                stacks.Add(cfg.layer, stack);
            }

            stack.Open(uiType, cfg, arg);
        }
    }

    // 必须先open
    public static void Close(int uiType) {
        if (TryGet(uiType, out FUIBase ui) && FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            stacks.TryGetValue(cfg.layer, out FUIStack stack);
            stack.Close(uiType, ui);
        }
    }

    // 必须先open
    public static void Show(int uiType) {
        if (TryGet(uiType, out FUIBase ui) && FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            stacks.TryGetValue(cfg.layer, out FUIStack stack);
            stack.Show(uiType, ui);
        }
    }

    // 必须先open
    public static void Hide(int uiType) {
        if (TryGet(uiType, out FUIBase ui) && FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            stacks.TryGetValue(cfg.layer, out FUIStack stack);
            stack.Hide(uiType, ui);
        }
    }
}
