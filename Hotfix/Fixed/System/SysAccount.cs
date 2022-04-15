using Logic.Pbf;

namespace Logic.Hotfix
{
    public partial class SysAccount : SysBase<SysAccount>
    {
        // 游戏是否进行过重连操作
        private int _syncCount = 0;

        // 数据同步
        public bool hasSynced
        {
            get { return _syncCount > 0; }
        }

        public bool hasLogin { get; private set; } = false;
        public bool msgFromReconnect { get; private set; } = false;

        // 主要是为了区分本次server数据下发是 重连下发/登陆下发
        public bool hasReconnectSync
        {
            get
            {
                // 同步两次，则表示必然进行了 断线重连
                return _syncCount > 1;
            }
        }

        public ushort playerId;
        public string accountName;

        public uint serverId;
    }

    public partial class SysAccount : SysBase<SysAccount>
    {
        protected override void ProcessEvent(bool toRegister)
        {
            NWDelegateService.emiter.Handle((ushort)EMsgType.Cslogin, OnResLogin, toRegister);
            NWDelegateService.emiter.Handle((ushort)EMsgType.Cslogout, OnResLogout, toRegister);
            NWDelegateService.emiter.Handle((ushort)EMsgType.Csreconnect, OnResReconnect, toRegister);
        }
        
        public override void OnSynced()
        {
            ++syncCount;
            msgFromReconnect = false;
            SystemMgr.Instance.OnSynced();
        }

        public void Connect(string ip, int port) {
            NWMgr.Instance.Connect(ip, port, _JudgeConnect);
        }

        private void _JudgeConnect(EConnectStatus connectStatus) {
            msgFromReconnect = false;
            if (connectStatus == EConnectStatus.Connected) {
                if (hasLogin) {
                    ReqReconnect();
                }
                else {
                    ReqLogin();
                }
            }
        }

        #region 登录
        // 正式登陆调用，重连不调用
        public void ReqLogin()
        {
        }

        // 登录回包
        public void OnResLogin() {
            _syncCount = 0;
            hasLogin = true;
            msgFromReconnect = false;
            SystemMgr.Instance.OnLogin();
        }
        #endregion

        #region 重连
        public void ReqReconnect() {
            
        }

        private void OnResReconnect() {
            msgFromReconnect = true;
        }
        #endregion

        #region 注销
        public void ReqLogout() {
        }

        public void OnResLogout()
        {
            _syncCount = 0;
            hasLogin = false;
            msgFromReconnect = false;
            SystemMgr.Instance.OnLogout();
        }
        #endregion

        #region 账号封禁等操作
        #endregion
    }
}
