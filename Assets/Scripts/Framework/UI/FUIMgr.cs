using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FUIStack {
    public static readonly int LAYER_GAP = 10000;
    public static readonly int UI_GAP = 30;

#if UNITY_EDITOR
    public EUILayer layer;
    public List<FUIBase> stack = new List<FUIBase>();
#else
    public EUILayer layer { get; protected set; }
    protected readonly List<FUIBase> stack = new List<FUIBase>();
#endif

    public int Count {
        get { return stack.Count; }
    }

    public int ActiveCount {
        get {
            int rlt = 0;
            for (int i = 0, length = Count; i < length; ++i) {
                if (stack[i].showHide) {
                    ++rlt;
                }
            }

            return rlt;
        }
    }

    public FUIStack(EUILayer layer) {
        this.layer = layer;
    }

    protected virtual bool TryCreate(int uiType, FUIEntry cfg, out FUIBase ui) {
        if (!FUIMgr.TryGet(uiType, out ui)) {
            if (cfg == null) {
                return false;
            }

            ui = cfg.CreateInstance();
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

    public bool IsTop(int uiType, FUIBase ui) {
        int index = stack.IndexOf(ui);
        return (index != -1) && (index == Count - 1);
    }
}

[Serializable]
public class FUIMgr {
    public static readonly string UI_PARENT_NAME = "__UI__";
#if UNITY_EDITOR
    public readonly static Dictionary<int, FUIBase> uiDict = new Dictionary<int, FUIBase>();
    public readonly static Dictionary<EUILayer, FUIStack> stacks = new Dictionary<EUILayer, FUIStack>(3);
#else
    private readonly static Dictionary<int, FUIBase> uiDict = new Dictionary<int, FUIBase>();
    private readonly static Dictionary<EUILayer, FUIStack> stacks = new Dictionary<EUILayer, FUIStack>(3);
#endif

    public class UIRootCfg {
        public GameObject root;
#if UNITY_EDITOR
        public EUILayer layer;
        public string name;
#endif

        public UIRootCfg(GameObject root, EUILayer layer, string name) {
            this.root = root;
#if UNITY_EDITOR
            this.layer = layer;
            this.name = name;
#endif
        }
    }

    public static readonly Dictionary<EUILayer, UIRootCfg> roots = new Dictionary<EUILayer, UIRootCfg>();

    public static Action<int, FUIEntry> OnOpen;
    public static Action<int, FUIEntry> OnClose;
    public static Action<int, FUIEntry> OnShow;
    public static Action<int, FUIEntry> OnHide;

    public static void Init() {
        Transform parent = new GameObject(UI_PARENT_NAME).transform;
        GameObject.DontDestroyOnLoad(parent);

#if UNITY_EDITOR
        parent.gameObject.AddComponent<FUIGuarder>();
#endif

        roots.Clear();
        for (int i = 1; i < (int)EUILayer.Max; ++i) {
            EUILayer layer = ((EUILayer)i);
            string name = layer.ToString();

            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);

#if UNITY_EDITOR
            go.AddComponent<FUIStackGuarder>().uiLayer = layer;
#endif

            roots.Add(layer, new UIRootCfg(go, layer, name));
        }
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

    public static void CloseUtil(int uiType, bool onlyJudgeOwnerStack) { }

    public static bool IsTop(int uiType, bool onlyJudgeOwnerStack /*在自己所属栈中isTop*/) {
        if (TryGet(uiType, out FUIBase ui) && FUIEntryRegistry.TryGet(uiType, out var cfg)) {
            EUILayer layer = cfg.layer;
            stacks.TryGetValue(layer, out FUIStack stack);
            bool isTopInOwnerStack = stack.IsTop(uiType, ui);
            if (onlyJudgeOwnerStack) {
                return isTopInOwnerStack;
            }

            if (!isTopInOwnerStack) {
                return false;
            }

            bool hasNextLayer = FUIEntry.TryGetNextLayer(layer, out var nextLayer);
            return !(hasNextLayer && stacks.TryGetValue(nextLayer, out FUIStack nextStack) && nextStack.ActiveCount > 0);
        }

        return false;
    }

    public static bool IsActive(int uiType) {
        if (TryGet(uiType, out FUIBase ui)) {
            return ui.showHide;
        }

        return false;
    }
}
