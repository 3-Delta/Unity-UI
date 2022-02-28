using System;
using System.Collections.Generic;

[Serializable]
public class UIEntry : FUIEntry {
    public UIEntry() {
    }

    public UIEntry Reset(EUIType uiType, string prefabPath, Type ui, EUIOption option = EUIOption.None, EUILayer layer = EUILayer.NormalStack) {
        this.uiType = (int)uiType;
        this.prefabPath = prefabPath;
        this.ui = ui;
        this.option = option;
        this.layer = layer;

        return this;
    }

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
        new UIEntry().Reset(EUIType.UIMain, "UIMain", typeof(UIMain), EUIOption.None, EUILayer.BasementStack),
        new UIEntry().Reset(EUIType.UILogin, "UILogin", typeof(UILogin), EUIOption.HideBefore | EUIOption.CheckGuide | EUIOption.CheckNetwork | EUIOption.CheckQuality | EUIOption.Disable3DCamera | EUIOption.DisableBeforeRaycaster | EUIOption.Mask),
        new UIEntry().Reset(EUIType.UIWairForNetwork, "UIWairForNetwork", typeof(UIWairForNetwork), EUIOption.None),
    };
}
