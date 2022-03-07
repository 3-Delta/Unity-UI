namespace Logic.Hotfix
{
    public class SysAccount : SysBase<SysAccount>, IReload
    {
        // 数据同步
        public bool hasSynced
        {
            get { return syncCount > 0; }
        }

        // 主要是为了区分本次server数据下发是 重连下发/登陆下发
        public bool hasReconnectSync
        {
            get
            {
                // 同步两次，则表示必然进行了 断线重连
                return syncCount > 1;
            }
        }

        // 游戏是否进行过重连操作
        private int syncCount = 0;

        // 正式登陆调用，重连不调用
        public void ReqLogin()
        {
            syncCount = 0;
        }

        // 登录回包
        public void OnResLogin() { }

        public override void OnSynced()
        {
            ++syncCount;
        }

        public void OnReload()
        {
            // 表格热更新
        }
    }
}