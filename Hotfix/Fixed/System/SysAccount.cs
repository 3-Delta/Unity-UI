using Logic.Pbf;

namespace Logic.Hotfix
{
    public partial class SysAccount : SysBase<SysAccount>
    {
        // 游戏是否进行过重连操作
        private int syncCount = 0;

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

        public ushort id;
        public string name;
        public uint level;

        public uint serverId;
    }

    public partial class SysAccount : SysBase<SysAccount>
    {
        protected override void ProcessEvent(bool toRegister)
        {
            NWDelegateService.emiter.Handle((ushort)EMsgType.Cslogin, OnResLogin, toRegister);
            NWDelegateService.emiter.Handle((ushort)EMsgType.Cslogout, OnResLogout, toRegister);
        }

        // 正式登陆调用，重连不调用
        public void ReqLogin()
        {
            syncCount = 0;
        }

        // 登录回包
        public void OnResLogin()
        {
            SystemMgr.Instance.OnLogin();
        }

        public void ReqLogout()
        {

        }

        public void OnResLogout()
        {
            SystemMgr.Instance.OnLogout();
        }

        public override void OnSynced()
        {
            ++syncCount;
            SystemMgr.Instance.OnSynced();
        }
    }
}