#define DEVELOPMENT_MODE

using System.IO;

using UnityEngine;

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

        // 参数表示是否为重连导致的登录
        void OnLogin(bool isReconnect);
        void OnLogout();

        // 参数表示是否为重连导致的数据同步
        void OnSynced(bool isReconnect);
        void OnReload();

        #if UNITY_EDITOR
        void ToVisualize();
        #endif
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


        public virtual void OnLogin(bool isReconnect) { }

        public virtual void OnLogout() { }

        public virtual void OnSynced(bool isReconnect) { }

        // 表格配置等重载
        public virtual void OnReload() { }

        // 数据可视化,比如序列化成io的json文件，或者序列化道redis，或者数据库中
        public virtual void ToVisualize() {
            string json = JsonUtility.ToJson(this);
            string path = Application.dataPath + "/../Library/ScriptAssemblies/" + typeof(T) + ".json";
            if (!File.Exists(path)) {
                using (File.Create(path)) { }
            }
            File.WriteAllText(path, json);
        }
    }
}