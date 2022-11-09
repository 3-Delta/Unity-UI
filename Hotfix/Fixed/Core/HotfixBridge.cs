namespace Logic.Hotfix.Fixed
{
    // 非热更层 调用 热更层 的入口
    public static class HotfixBridge
    {
        // 热更层初始化
        public static void Init()
        {
            SystemMgr.Instance.Enter();

            // 填充UIEntry结构
            UIInject();
            FUIMgr.Init();
        }

        public static void UIInject()
        {
            UIEntryRegistry.Inject();
        }
    }
}
