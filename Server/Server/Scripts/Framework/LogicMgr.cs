using System;
using System.Collections;
using System.Collections.Generic;

public class LogicMgr : Singleton<LogicMgr>
{
	public void OnInit()
	{
		SystemMgr.Instance.OnInit();
        ManagerMgr.Instance.OnInit();
    }
    public void OnReload()
    {
        SystemMgr.Instance.OnReload();
    }
    public void OnLogin() { SystemMgr.Instance.OnLogin(); }
    public void OnLogout() { SystemMgr.Instance.OnLogout(); }
    public void OnUpdate()
	{
		SystemMgr.Instance.OnUpdate();
        ManagerMgr.Instance.OnUpdate();
    }
    public void OnExit()
    {
        SystemMgr.Instance.OnExit();
        ManagerMgr.Instance.OnExit();
    }
}