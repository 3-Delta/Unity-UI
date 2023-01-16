using Logic.Hotfix.Fixed.Pbf;

namespace Logic.Hotfix.Fixed
{
    // 处理servertime
    public class ServerTimeService : SysBase<ServerTimeService>
    {
        protected override void ProcessEvent(bool toRegister)
        {
            if (toRegister)
            {
                NWDelegateService.Add<SCTimeNtf>(0, (ushort)EMsgType.SctimeNtf, OnSctimeNtf, SCTimeNtf.Parser);
            }
            else
            {
                NWDelegateService.Remove<SCTimeNtf>((ushort)EMsgType.SctimeNtf, OnSctimeNtf);
            }
        }

        private long syncLocalTime;
        private long syncRemoteTime;
        
        public long RemoteTimeZoneSeconds { get; private set; }
        public long RemoteNow {
            get {
                var passed = TimeService.NowTotalSeconds - syncLocalTime;
                return syncRemoteTime + passed;
            }
        }

        public void AdjustTime(TimeSync msg)
        {
            RemoteTimeZoneSeconds = msg.TimeZoneSeconds;
            
            syncRemoteTime = msg.TimeSeconds;
            syncLocalTime = TimeService.NowTotalSeconds;
        }
        
        private void OnSctimeNtf(SCTimeNtf msg)
        {
            UnityEngine.Debug.LogError("res.Time: " + msg.Time.TimeSeconds);

            var oldTime = RemoteNow;
            AdjustTime(msg.Time);
            var newTime = RemoteNow;
            
            SystemMgr.Instance.OnTimeAdjusted(newTime, oldTime);
        }
    }
}
