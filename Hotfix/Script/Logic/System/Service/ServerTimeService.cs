using Google.Protobuf;

using Logic.Pbf;

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

        protected override void ProcessEvent(bool toRegister)
        {
            if (toRegister)
            {
                NWDelegateService.Add(0, (ushort)MsgType.SctimeNtf, OnSctimeNtf, (pkg) =>
                {
                    // 参考项目如何处理协议的解析 ？？
                    return null;
                });
            }
            else
            {
                NWDelegateService.Remove((ushort)MsgType.SctimeNtf, OnSctimeNtf);
            }
        }

        private void OnSctimeNtf(IMessage msg)
        {
        }
    }
}
