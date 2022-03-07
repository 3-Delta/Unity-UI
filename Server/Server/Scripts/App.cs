using System;
using System.Threading;

public static class App
{
    public enum EAppState
    {
        Running,
        Reload, // 热更新表格
        Exit,
    }

    public static EAppState currentState = EAppState.Running;

    public static void Init()
    {
        currentState = EAppState.Running;
        LogicMgr.Instance.OnInit();

        //EventManager<EMsgType>.Handle(EMsgType.OnReload, () =>
        //{
        //    LogicMgr.Instance.OnReload();
        //});
    }

    public static void Update()
    {
        while (currentState != EAppState.Exit)
        {
            LogicMgr.Instance.OnUpdate();
            Thread.Sleep(TimeMgr.TIMEGAP);
        }
    }

    public static void Exit()
    {
        LogicMgr.Instance.OnExit();
    }
}
