using System;
using System.Collections.Generic;

[Flags]
public enum EUIOption {
    None = 1 << 1,
    HideBefore = 1 << 1,
    Disable3DCamera = 1 << 2,
    CheckGuide = 1 << 3,
    CheckNetwork = 1 << 4,
    CheckQUality = 1 << 5,
}

public enum EUILayer {
    Basement = 0,
    NormalStack = 10000,
    TopStack = 20000,
}

[Serializable]
public class FUIEntry {
#if UNITY_EDITOR
    public int uiType;
    public string prefabPath;
    public Type ui;
    public EUIOption option;
    public EUILayer layer;

    public bool hasMask; // 背景遮挡蒙版, 方便整体调控
    public int renderFrameInterval = 1; // 渲染帧率
#else
    public readonly int uiType;
    public readonly string prefabPath;
    public readonly Type ui;
    public readonly EUIOption option;
    public readonly EUILayer layer;

    public readonly bool hasMask; // 背景遮挡蒙版, 方便整体调控
    public readonly int renderFrameInterval = 1; // 渲染帧率
#endif

    public FUIEntry() { }

    public FUIEntry(int uiType, string prefabPath, Type ui, EUIOption option = EUIOption.None,
        EUILayer layer = EUILayer.NormalStack, bool hasMask = false, int renderFrameInterval = 1) {
        this.uiType = uiType;
        this.prefabPath = prefabPath;
        this.ui = ui;
        this.option = option;
        this.layer = layer;

        this.renderFrameInterval = renderFrameInterval;
    }

    public bool Contains(EUIOption option) {
        return ((this.option & option) == option);
    }
}

// ui配置 // 热更层动态插入框架层
public class FUIEntryRegistry {
    private static readonly Dictionary<int, FUIEntry> registry = new Dictionary<int, FUIEntry>();

    public static bool TryGet(int uiType, out FUIEntry entry) {
        return registry.TryGetValue(uiType, out entry);
    }

    public static void Register(FUIEntry entry) {
        if (entry != null && !registry.TryGetValue(entry.uiType, out _)) {
            registry.Add(entry.uiType, entry);
        }
    }
}
