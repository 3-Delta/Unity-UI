using System.Collections;
using System.Collections.Generic;

public class BS_SystemBaseCallback
{
    // 系统初始化
	public virtual void OnInit() {}
    // 系统表格热更新
    public virtual void OnReload() {}
    // 玩家登陆
    public virtual void OnLogin() {}
    // 玩家登出
	public virtual void OnLogout() {}
    // 系统更新
	public virtual void OnUpdate() {}
    // 系统退出
	public virtual void OnExit() {}
}

public class SystemBase<T> : BS_SystemBaseCallback where T : class, new()
{
    protected SystemBase() { }
    public static T Instance { get { return Singleton<T>.Instance; } }
}
