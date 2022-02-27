#define DEVELOPMENT_MODE

public interface IReload {
#if DEVELOPMENT_MODE
    // 只在开发模式下使用
    void OnReload();
#else
    // do nothing
#endif
}

public interface ISysModule {
    void Enter();
    void Exit();

    void OnLogin();
    void OnLogout();

    void OnSynced();
}

public abstract class SysBase<T> : ISysModule where T : ISysModule, new() {
    protected SysBase() { }

    public static T Instance = System.Activator.CreateInstance<T>();

    protected virtual void ProcessEvent(bool toRegister) { }

    public void Enter() {
        OnEnter();
        ProcessEvent(true);
    }

    public void Exit() {
        ProcessEvent(false);
        OnEnter();
    }

    protected virtual void OnEnter() { }

    protected virtual void OnExit() { }

    public virtual void OnLogin() { }

    public virtual void OnLogout() { }

    public virtual void OnSynced() { }
}
