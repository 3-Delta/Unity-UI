namespace Logic.Hotfix
{
    // 处理servertime
    public class ServerTimeService : SysBase<ServerTimeService>
    {
        public enum EEvents
        {
            OnTimeSync,
        }

        public readonly DelegateService<EEvents> emiter = new DelegateService<EEvents>();

        protected override void OnEnter()
        {

        }
    }
}
