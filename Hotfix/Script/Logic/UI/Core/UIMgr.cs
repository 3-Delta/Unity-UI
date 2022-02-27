using System;

public class UIMgr {
    public static void Init() {
        UIEntryRegistry.Inject();
        FUIMgr.Init();
    }

    public static void Open(EUIType uiType, Tuple<ulong, ulong, ulong, object> arg = null) {
        FUIMgr.Open((int)(uiType), arg);
    }

    public static void Close(EUIType uiType) {
        FUIMgr.Close((int)(uiType));
    }

    public static void Show(EUIType uiType) {
        FUIMgr.Show((int)(uiType));
    }

    public static void Hide(EUIType uiType) {
        FUIMgr.Hide((int)(uiType));
    }

    public static void CloseUtil(EUIType uiType, bool onlyJudgeOwnerStack) {
        FUIMgr.CloseUtil((int)(uiType), onlyJudgeOwnerStack);
    }
}
