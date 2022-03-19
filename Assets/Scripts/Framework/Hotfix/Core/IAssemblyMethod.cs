using System;

public interface IAssemblyMethod {
    void Exec();
    void Exec<T1>(T1 arg1);
    void Exec<T1, T2>(T1 arg1, T2 arg2);
    void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
}

public abstract class StaticMethod : IAssemblyMethod {
    public abstract void Exec();
    public abstract void Exec<T1>(T1 arg1);
    public abstract void Exec<T1, T2>(T1 arg1, T2 arg2);
    public abstract void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
}

public abstract class InstanceMethod : IAssemblyMethod {
    public abstract void Exec();
    public abstract void Exec<T1>(T1 arg1);
    public abstract void Exec<T1, T2>(T1 arg1, T2 arg2);
    public abstract void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
}

public class ILRStaticMethod : StaticMethod {
    private readonly ILRuntime.Runtime.Enviorment.AppDomain appDomain;
    private readonly ILRuntime.CLR.Method.IMethod method;
    private readonly object[] args;

    public ILRStaticMethod(ILRuntime.Runtime.Enviorment.AppDomain appDomain, string typeName, string methodName, int argCount = 0) {
        this.appDomain = appDomain;

        // 反射得到程序集中所有type,然后包装成iltype
        ILRuntime.CLR.TypeSystem.IType ilType = appDomain.LoadedTypes[typeName];
        // 从  typeName所在的   iltype中获取合适的method, 内部将clrmethod包装成 ilmethod
        this.method = ilType.GetMethod(methodName, argCount);
        this.args = new object[argCount];
    }

    public override void Exec() {
        appDomain.Invoke(method, null, args);
    }

    public override void Exec<T1>(T1 arg1) {
        args[0] = arg1;
        appDomain.Invoke(method, null, args);
    }

    public override void Exec<T1, T2>(T1 arg1, T2 arg2) {
        args[0] = arg1;
        args[1] = arg2;
        appDomain.Invoke(method, null, args);
    }

    public override void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) {
        args[0] = arg1;
        args[1] = arg2;
        args[2] = arg3;
        appDomain.Invoke(method, null, args);
    }
}

public class ILRInstanceMethod : InstanceMethod {
    private readonly ILRuntime.Runtime.Enviorment.AppDomain appDomain;
    private readonly ILRuntime.CLR.Method.IMethod method;
    private readonly object instance;
    private readonly object[] args;

    public ILRInstanceMethod(ILRuntime.Runtime.Enviorment.AppDomain appDomain, string typeName, string methodName, ref object refInstance, int argCount = 0) {
        this.appDomain = appDomain;
        this.method = appDomain.LoadedTypes[typeName].GetMethod(methodName, argCount);
        this.args = new object[argCount];

        if (refInstance == null) {
            refInstance = appDomain.Instantiate(typeName);
        }
        this.instance = refInstance;
    }

    public override void Exec() {
        appDomain.Invoke(method, instance, args);
    }

    public override void Exec<T1>(T1 arg1) {
        args[0] = arg1;
        appDomain.Invoke(method, instance, args);
    }

    public override void Exec<T1, T2>(T1 arg1, T2 arg2) {
        args[0] = arg1;
        args[1] = arg2;
        appDomain.Invoke(method, instance, args);
    }

    public override void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) {
        args[0] = arg1;
        args[1] = arg2;
        args[2] = arg3;
        appDomain.Invoke(method, instance, args);
    }
}

public class MonoStaticMethod : StaticMethod {
    private readonly System.Reflection.MethodInfo method;
    private readonly object[] args;

    public MonoStaticMethod(System.Reflection.Assembly assembly, string typeName, string methodName) {
        this.method = assembly.GetType(typeName).GetMethod(methodName);
        this.args = new object[method.GetParameters().Length];
    }

    public override void Exec() {
        method.Invoke(null, args);
    }

    public override void Exec<T1>(T1 arg1) {
        args[0] = arg1;
        method.Invoke(null, args);
    }

    public override void Exec<T1, T2>(T1 arg1, T2 arg2) {
        args[0] = arg1;
        args[1] = arg2;
        method.Invoke(null, args);
    }

    public override void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) {
        args[0] = arg1;
        args[1] = arg2;
        args[2] = arg3;
        method.Invoke(null, args);
    }
}

public class MonoInstanceMethod : InstanceMethod {
    private readonly System.Reflection.MethodInfo method;
    private readonly object instance;
    private readonly object[] args;

    public MonoInstanceMethod(System.Reflection.Assembly assembly, string typeName, string methodName, ref object refInstance) {
        this.method = assembly.GetType(typeName).GetMethod(methodName);
        this.args = new object[method.GetParameters().Length];

        if (refInstance == null) {
            refInstance = assembly.CreateInstance(typeName);
        }
        this.instance = refInstance;
    }

    public override void Exec() {
        method.Invoke(instance, args);
    }

    public override void Exec<T1>(T1 arg1) {
        args[0] = arg1;
        method.Invoke(instance, args);
    }

    public override void Exec<T1, T2>(T1 arg1, T2 arg2) {
        args[0] = arg1;
        args[1] = arg2;
        method.Invoke(instance, args);
    }

    public override void Exec<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) {
        args[0] = arg1;
        args[1] = arg2;
        args[2] = arg3;
        method.Invoke(instance, args);
    }
}
