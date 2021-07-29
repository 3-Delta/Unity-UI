using System;
using System.Collections.Generic;

[Flags]
public enum EUIOption {
    None = 1 << 1,
    HideBefore = 1 << 1,
    Disable3DCamera = 1 << 2,
    CheckGuide = 1 << 3,
    CheckNetwork = 1 << 4,
}

public enum EUILayer {
    Normal = 1,
    Base = 2,
    Top = 3,
}

[Serializable]
public class UIEntry {
    public readonly int uiType;
    public readonly string prefabPath;
    public readonly Type uiController;
    public readonly EUIOption option;
    public readonly EUILayer layer;

    public readonly bool hasMask; // 背景遮挡蒙版, 方便整体调控
    public readonly int renderFrameInterval = 1; // 渲染帧率

    public UIEntry() {
    }

    public UIEntry(int uiType, string prefabPath, Type uiController, EUIOption option = EUIOption.None,
        EUILayer layer = EUILayer.Normal, bool hasMask = false, int renderFrameInterval = 1) {
        this.uiType = uiType;
        this.prefabPath = prefabPath;
        this.uiController = uiController;
        this.option = option;
        this.layer = layer;

        this.renderFrameInterval = renderFrameInterval;
    }

    public bool Contains(EUIOption option) {
        return ((this.option & option) == option);
    }
}

public class UIEntryRegistry {
    private static readonly Dictionary<int, UIEntry> registry = new Dictionary<int, UIEntry>();

    public static bool TryGet(int uiType, out UIEntry entry) {
        entry = null;
        return registry.TryGetValue(uiType, out entry);
    }

    public static void Clear() {
        registry.Clear();
    }

    public static void Register(UIEntry entry) {
        if (entry != null && !registry.TryGetValue(entry.uiType, out _)) {
            registry.Add(entry.uiType, entry);
        }
    }
}