#define DEVELOPMENT_MODE

namespace Logic.Hotfix.Fixed
{
    public interface IReload
    {
#if DEBUG
        // 只在开发模式下使用
        void OnReload();
#else
    // do nothing
#endif
    }

    public interface ISysModule
    {
        void Enter();
        void Exit();

        void OnLogin();
        void OnLogout();

        void OnSynced();
        void OnReload();
    }

    [System.Serializable]
    public abstract class SysBase<T> : ISysModule where T : ISysModule, new()
    {
        protected SysBase() { }

        public static T Instance = System.Activator.CreateInstance<T>();


        public void Enter()
        {
            OnEnter();
            ProcessEvent(true);
        }

        public void Exit()
        {
            ProcessEvent(false);
            OnExit();
        }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }
        protected virtual void ProcessEvent(bool toRegister) { }


        public virtual void OnLogin() { }

        public virtual void OnLogout() { }

        public virtual void OnSynced() { }

        // 表格配置等重载
        public virtual void OnReload() { }

        // 数据可视化,比如序列化成io的json文件，或者序列化道redis，或者数据库中
        public virtual void ToVisualize() { }
    }
}