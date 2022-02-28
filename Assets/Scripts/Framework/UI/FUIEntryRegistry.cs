using System;
using System.Collections.Generic;

[Flags]
public enum EUIOption {
    None = 1 << 1,
    HideBefore = 1 << 2, // 隐藏前UI
    Disable3DCamera = 1 << 3, // 关闭3d相机
    CheckQuality = 1 << 4, // 调整整体game的quality
    DisableBeforeRaycaster = 1 << 5, // 关闭前UI的raycaster, 如果前ui被hide,则没必要执行此设置
    Mask = 1 << 6, // 背景遮挡蒙版, 方便整体调控
    CheckGuide = 1 << 7, // 检测引导
    CheckNetwork = 1 << 8, // 网络等待，网络转圈
}

public enum EUILayer {
    BasementStack = 1, // 只允许场景的第一个UI，比如主界面，战斗主界面 进行该设置
    NormalStack = 2,
    TopStack = 3,
    
    Max, // 禁止使用
}

[Serializable]
public class FUIEntry {
#if UNITY_EDITOR && __DEV_MODE__
    public int uiType;
    public string prefabPath;
    public Type ui;
    public EUIOption option;
    public EUILayer layer;
#else
    public readonly int uiType;
    public readonly string prefabPath;
    public readonly Type ui;
    public readonly EUIOption option;
    public readonly EUILayer layer;
#endif

    public FUIEntry() { }

    public FUIEntry(int uiType, string prefabPath, Type ui, EUIOption option = EUIOption.None, EUILayer layer = EUILayer.NormalStack) {
        this.uiType = uiType;
        this.prefabPath = prefabPath;
        this.ui = ui;
        this.layer = layer;
        this.option = option;
    }

    public virtual FUIBase CreateInstance() {
        return Activator.CreateInstance(ui) as FUIBase;
    }

    public bool Contains(EUIOption target) {
        return ((option & target) == target);
    }

    public static bool TryGetNextLayer(EUILayer layer, out EUILayer nextLayer) {
        if (layer < EUILayer.TopStack) {
            nextLayer = layer + 1;
            return true;
        }

        nextLayer = layer;
        return false;
    }
    
    public static bool TryGetPreLayer(EUILayer layer, out EUILayer preLayer) {
        if (layer > EUILayer.BasementStack) {
            preLayer = layer - 1;
            return true;
        }

        preLayer = layer;
        return false;
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
