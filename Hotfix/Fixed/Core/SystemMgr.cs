namespace Logic.Hotfix.Fixed
{
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
        public override void OnLogin(bool isReconnect)
        {
            for (int i = 0, length = SystemList.list.Count; i < length; i++)
            {
                SystemList.list[i].OnLogin(isReconnect);
            }
        }
        public override void OnLogout()
        {
            for (int i = 0, length = SystemList.list.Count; i < length; i++)
            {
                SystemList.list[i].OnLogout();
            }
        }

        public override void OnSynced(bool isReconnect)
        {
            for (int i = 0, length = SystemList.list.Count; i < length; i++)
            {
                SystemList.list[i].OnSynced(isReconnect);
            }
        }

        public override void OnTimeAdjusted(long newTime, long oldTime)
        {
            for (int i = 0, length = SystemList.list.Count; i < length; i++)
            {
                SystemList.list[i].OnTimeAdjusted(newTime, oldTime);
            }
        }

        public override void ToVisualize()
        {
#if UNITY_EDITOR
            for (int i = 0, length = SystemList.list.Count; i < length; i++)
            {
                SystemList.list[i].ToVisualize();
            }
#endif
        }
    }
}
