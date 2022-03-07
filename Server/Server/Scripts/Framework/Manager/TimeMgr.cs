using System;

public class TimeMgr : ManagerBase<TimeMgr>
{
    public const int TIMEGAP = 25;
    // 毫秒
    public ulong time { get; private set; } = 0;

    public override void OnInit()
    {
        time = 0;
    }
    public override void OnUpdate()
    {
        time += TIMEGAP;
    }
}
