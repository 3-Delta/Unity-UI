using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

// Path: Assets/Scripts/Base/BS_ManagerList.cs.
// SvnVersion: -1.
// Author: kaclok created 2018/12/16 15:38:22 Sunday on pc: KACLOK.
// Copyright@nullgame`s testgame. All rights reserved.

public static class ManagerList
{
    // BS_ManagerMgr和BS_DriveMgr不能放入
    public readonly static List<BS_ManagerBaseCallback> list = new List<BS_ManagerBaseCallback>()
    {
        NWMgr.Instance,
        GMMgr.Instance,
        RedisMgr.Instance,
        TimeMgr.Instance,
        FrameMgr.Instance,
        LogMgr.Instance,
    };
}