using System;

public class UIStack : FUIStack {
    public UIStack(EUILayer layer) : base(layer) { }

    protected override FUIBase CreateInstance(FUIEntry cfg) {
        return Activator.CreateInstance(cfg.ui) as UIBase;
    }
}

public class UIMgr : FUIMgr {
    public new static void Init() {
        UIEntryRegistry.Inject();
        FUIMgr.Init();
    }

    public static void Open(EUIType uiType, Tuple<ulong, ulong, ulong, object> arg = null) {
        Open((int)(uiType), arg);
    }

    public static void Close(EUIType uiType) {
        Close((int)(uiType));
    }

    public static void Show(EUIType uiType) {
        Show((int)(uiType));
    }

    public static void Hide(EUIType uiType) {
        Hide((int)(uiType));
    }
}
