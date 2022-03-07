using System.Collections;
using System.Collections.Generic;

// 在Game中统一驱动调用
public class SystemMgr : SystemBase<SystemMgr>
{
	public override void OnInit() 
	{
        SystemList.list.ForEach(system => { system.OnInit(); });
	}
    public override void OnReload()
    {
        SystemList.list.ForEach(system => { system.OnReload(); });
    }
    public override void OnLogin() 
	{
        SystemList.list.ForEach(system => { system.OnLogin(); });
	}
	public override void OnLogout() 
	{
        SystemList.list.ForEach(system => { system.OnLogout(); });
	}
	public override void OnUpdate()
	{
        SystemList.list.ForEach(system => { system.OnUpdate(); });
	}
	public override void OnExit() 
	{
        SystemList.list.ForEach(system => { system.OnExit(); });
	}
}