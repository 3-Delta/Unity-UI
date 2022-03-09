﻿using Logic.Hotfix;

using System.Collections;
using System.Collections.Generic;

// 在Game中统一驱动调用
public class SystemMgr : SysBase<SystemMgr>
{
    protected override void OnEnter()
    {
        for (int i = 0, length = SystemList.list.Count; i < length; i++)
        {
            SystemList.list[i].Enter();
        }
    }
    protected override void OnExit()
    {
        for (int i = 0, length = SystemList.list.Count; i < length; i++)
        {
            SystemList.list[i].Exit();
        }
    }
    public override void OnReload()
    {
        for (int i = 0, length = SystemList.list.Count; i < length; i++)
        {
            SystemList.list[i].OnReload();
        }
    }
    public override void OnLogin()
    {
        for (int i = 0, length = SystemList.list.Count; i < length; i++)
        {
            SystemList.list[i].OnLogin();
        }
    }
    public override void OnLogout()
    {
        for (int i = 0, length = SystemList.list.Count; i < length; i++)
        {
            SystemList.list[i].OnLogout();
        }
    }
}