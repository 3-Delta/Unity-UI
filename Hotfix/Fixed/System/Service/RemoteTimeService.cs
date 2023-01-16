using Logic.Hotfix.Fixed.Pbf;

using System;

namespace Logic.Hotfix.Fixed
{
    // 处理servertime
    public partial class RemoteTimeService : SysBase<RemoteTimeService>
    {
        public readonly DelegateService<Events> eventEmiter = new DelegateService<Events>();
        public enum Events
        {
            OnCrossDay, // 跨天
            OnCrossWeek, // 跨周
            OnCrossMonth, // 跨月
        }

        private long lastCheckDayTime;
        private MTimer timer4CorssDay;

        private long lastCheckWeekTime;
        private MTimer timer4CorssWeek;

        private long lastCheckMonthTime;
        private MTimer timer4CorssMonth;

        protected override void ProcessEvent(bool toRegister)
        {
            NWDelegateService.Handle<SCTimeNtf>(0, (ushort)EMsgType.SctimeNtf, OnSctimeNtf, SCTimeNtf.Parser, toRegister);
        }

        public override void OnLogin(bool isReconnect)
        {
            lastCheckDayTime = 0;
            lastCheckWeekTime = 0;
            lastCheckMonthTime = 0;

            timer4CorssDay?.Cancel();
            timer4CorssWeek?.Cancel();
            timer4CorssMonth?.Cancel();
        }

        public override void OnLogout()
        {
            lastCheckDayTime = 0;
            lastCheckWeekTime = 0;
            lastCheckMonthTime = 0;

            timer4CorssDay?.Cancel();
            timer4CorssWeek?.Cancel();
            timer4CorssMonth?.Cancel();
        }

        private long syncLocalTime;
        private long syncRemoteTime;

        public long RemoteTimeZoneSeconds { get; private set; }
        public long RemoteNow
        {
            get
            {
                var passed = TimeService.LocalNowTotalSeconds - syncLocalTime;
                return syncRemoteTime + passed;
            }
        }

        private void DoAdjustTime(TimeSync msg)
        {
            RemoteTimeZoneSeconds = msg.TimeZoneSeconds;
            syncRemoteTime = msg.TimeSeconds;
            syncLocalTime = TimeService.LocalNowTotalSeconds;
        }

        // byAdjust表示是初始化还是运行时调整时间
        public void AdjustTime(TimeSync msg, bool byAdjust = false)
        {
            var oldTime = RemoteNow;

            // 重新赋值
            DoAdjustTime(msg);

            // 如果是初次时间设置，则通过设置oldTime和now一直让不触发跨天等事件，因为初次的时候oldTime必然为0
            if (!byAdjust)
            {
                oldTime = RemoteNow;
            }
            RebuildTimer(oldTime);
        }

        private void RebuildTimer(long oldTime)
        {
            long now = RemoteNow;

            lastCheckDayTime = oldTime;
            // 循环timer，每间隔xs判断一下
            timer4CorssDay?.Cancel();
            timer4CorssDay = MTimer.CreateOrReuse(ref timer4CorssDay, 5, () =>
            {
                if (!TimeService.IsSameDay(now, lastCheckDayTime))
                {
                    lastCheckDayTime = now;
                    eventEmiter.Fire(Events.OnCrossDay);
                }
            }, MTimer.True);

            lastCheckWeekTime = oldTime;
            // 循环timer，每间隔xs判断一下
            timer4CorssWeek?.Cancel();
            timer4CorssWeek = MTimer.CreateOrReuse(ref timer4CorssWeek, 5, () =>
            {
                if (!TimeService.IsSameWeek(now, lastCheckWeekTime))
                {
                    lastCheckWeekTime = now;
                    eventEmiter.Fire(Events.OnCrossWeek);
                }
            }, MTimer.True);

            lastCheckMonthTime = oldTime;
            // 循环timer，每间隔xs判断一下
            timer4CorssMonth?.Cancel();
            timer4CorssMonth = MTimer.CreateOrReuse(ref timer4CorssMonth, 5, () =>
            {
                if (!TimeService.IsSameMonth(now, lastCheckMonthTime))
                {
                    lastCheckWeekTime = now;
                    eventEmiter.Fire(Events.OnCrossMonth);
                }
            }, MTimer.True);
        }

        private void OnSctimeNtf(SCTimeNtf msg)
        {
            UnityEngine.Debug.LogError("res.Time: " + msg.Time.TimeSeconds);

            var oldTime = RemoteNow;
            AdjustTime(msg.Time, true);
            var newTime = RemoteNow;

            SystemMgr.Instance.OnTimeAdjusted(newTime, oldTime);
        }
    }

    public partial class RemoteTimeService {
        // 今日0点+offsetSeconds
        public DateTime Today(long offsetSeconds) {
            return TimeService.Day(RemoteNow, offsetSeconds);
        }

        public DateTime DayOfWeek(long offsetSeconds)
        {
            return TimeService.Week(RemoteNow, offsetSeconds);
        }

        public DateTime DayOfMonth(long offsetSeconds)
        {
            return TimeService.Month(RemoteNow, offsetSeconds);
        }
    }
}
