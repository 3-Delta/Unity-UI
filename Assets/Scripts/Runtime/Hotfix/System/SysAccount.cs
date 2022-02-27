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

public class SysAccount : SysBase<SysAccount>, IReload {
    // 数据同步
    public bool hasSynced {
        get { return syncCount > 0; }
    }

    // 主要是为了区分本次server数据下发是 重连下发/登陆下发
    public bool hasReconnectSync {
        get {
            // 同步两次，则表示必然进行了 断线重连
            return syncCount > 1;
        }
    }

    // 游戏是否进行过重连操作
    private int syncCount = 0;

    // 正式登陆调用，重连不调用
    public void ReqLogin() {
        syncCount = 0;
    }

    // 登录回包
    public void OnResLogin() { }

    protected virtual void OnSynced() {
        ++syncCount;
    }

    public void OnReload() {
        // 表格热更新
    }
}

public class SysTeam : SysBase<SysAccount> {
    public uint teamId = 0;
    public ulong camptainId = 0;

    public bool hasTeam {
        get { return teamId != 0; }
    }

    public bool isCamptain {
        get {
            // return camptainId == 账号id;
            return true;
        }
    }

    public bool CanOp {
        get { return ((!hasTeam) || (hasTeam && isCamptain)); }
    }
}
