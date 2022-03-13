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
                NWDelegateService.Add<SCTimeNtf>(0, (ushort)MsgType.SctimeNtf, OnSctimeNtf, SCTimeNtf.Parser);
            }
            else
            {
                NWDelegateService.Remove<SCTimeNtf>((ushort)MsgType.SctimeNtf, OnSctimeNtf);
            }
        }

        private void OnSctimeNtf(SCTimeNtf msg)
        {
            UnityEngine.Debug.LogError("res.Time: " + msg.Time);
        }
    }
}
