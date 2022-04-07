namespace Logic.Hotfix {
    // 处理重连
    public class ReconnectService : SysBase<ReconnectService> {
        public bool isReconnecting { get; private set; }

        public void BeginReconnect() { }

        public void EndReconnect() { }
    }
}
