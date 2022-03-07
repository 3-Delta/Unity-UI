﻿using System;

namespace Logic.Hotfix
{
    public class UIMgr : FUIMgr
    {
        public new static void Init()
        {
            UIEntryRegistry.Inject();
            FUIMgr.Init();
        }

        public static void Open(EUIType uiType, Tuple<ulong, ulong, ulong, object> arg = null)
        {
            Open((int)(uiType), arg);
        }

        public static void Close(EUIType uiType)
        {
            Close((int)(uiType));
        }

        public static void Show(EUIType uiType)
        {
            Show((int)(uiType));
        }

        public static void Hide(EUIType uiType)
        {
            Hide((int)(uiType));
        }

        public static void CloseUtil(EUIType uiType, bool onlyJudgeOwnerStack)
        {
            CloseUtil((int)(uiType), onlyJudgeOwnerStack);
        }
    }
}
