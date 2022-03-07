using System;

public class ManagerMgr : ManagerBase<ManagerMgr>
{
    public override void OnInit()
    {
        ManagerList.list.ForEach(mgr => { mgr.OnInit(); });
    }
    public override void OnUpdate()
    {
        ManagerList.list.ForEach(mgr => { mgr.OnUpdate(); });
    }
    public override void OnExit()
    {
        ManagerList.list.ForEach(mgr => { mgr.OnExit(); });
    }
}