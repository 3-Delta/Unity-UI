using System;
using System.Collections.Generic;

[Flags]
public enum EUIOption {
    None = 0,
    HideBefore = 1,
    Hide3DCamera = 2,
    CHeckGuide = 4,
}

public class UIEntry {
    public int uiType;
    public string prefabPath;
    public EUIOption option;
    public Type script;
}

public class UIEntrys {
    private static readonly Dictionary<int, UIEntry> entrys = new Dictionary<int, UIEntry>();

    public static bool TryGet(int uiType, out UIEntry entry) {
        entry = null;
        return entrys.TryGetValue(uiType, out entry);
    }

    public static void Clear() {
        entrys.Clear();
    }

    public static void Add(UIEntry entry) {
        if (entry != null && !entrys.TryGetValue(entry.uiType, out _)) {
            entrys.Add(entry.uiType, entry);
        }
    }
}
