using System;
using System.Collections.Generic;

public class FrameMgr : ManagerBase<FrameMgr>
{
    public ulong frame { get; private set; } = 0;

    public override void OnInit()
    {
        frame = 0;
    }
    public override void OnUpdate()
    {
        ++frame;
    }
}
