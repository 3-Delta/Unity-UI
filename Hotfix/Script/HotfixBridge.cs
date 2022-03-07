using System;
using System.Collections.Generic;

namespace Logic.Hotfix
{
    // 非热更层 调用 热更层 的入口
    public static class HotfixBridge
    {
        // 热更层初始化
        public static void Init()
        {
            // 填充UIEntry结构
            UIMgr.Init();
        }
    }
}
