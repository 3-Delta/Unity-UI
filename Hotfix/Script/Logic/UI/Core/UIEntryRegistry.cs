using System;
using System.Collections.Generic;

[Serializable]
public class UIEntry : FUIEntry {
    public UIEntry(EUIType uiType, string prefabPath, Type ui, EUIOption option = EUIOption.None, EUILayer layer = EUILayer.NormalStack) :
        base((int)uiType, prefabPath, ui, option, layer) { }

    public override FUIBase CreateInstance() {
        return Activator.CreateInstance(ui) as FUIBase;
    }
}

public class UIEntryRegistry {
    public static void Inject() {
        for (int i = 0, length = registry.Count; i < length; ++i) {
            FUIEntryRegistry.Register(registry[i]);
        }

        registry.Clear();
        registry = null;
    }

    private static List<UIEntry> registry = new List<UIEntry>() {
        new UIEntry(EUIType.UIMain, "UIMain", typeof(UIMain), EUIOption.None, EUILayer.BasementStack),
        new UIEntry(EUIType.UILogin, "UILogin", typeof(UILogin), EUIOption.HideBefore | EUIOption.CheckGuide | EUIOption.CheckNetwork | EUIOption.CheckQuality | EUIOption.Disable3DCamera | EUIOption.DisableBeforeRaycaster | EUIOption.Mask),
        new UIEntry(EUIType.UIWairForNetwork, "UIWairForNetwork", typeof(UIWairForNetwork), EUIOption.None),
    };
}
