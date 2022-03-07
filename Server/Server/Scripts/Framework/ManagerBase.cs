using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class BS_ManagerBaseCallback
{
    public virtual void OnInit() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}

public class ManagerBase<T> : BS_ManagerBaseCallback where T : class, new()
{
    protected ManagerBase() { }
    public static T Instance { get { return Singleton<T>.Instance; } }
}