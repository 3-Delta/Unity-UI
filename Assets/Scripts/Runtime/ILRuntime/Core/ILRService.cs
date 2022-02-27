using System;
using System.Collections.Generic;

using ILRuntime.Runtime.Intepreter;

// 缺少：自动生成所有的delegate转换器
// 自动生成所有的继承适配器
// 参考： https://github.com/qq576067421/cshotfix
public static class ILRService {
    public static void Init(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
        // 代理适配器
        RegisterDelegate(appDomain);

        // 继承适配器【热更类型继承框架类型】
        RegisterAdaptor(appDomain);

        // 值类型绑定【本质也是重定向】
        RegisterBinder(appDomain);

        // 重定向【热更调用框架的函数】
        RegisterRedirection(appDomain);

        // wainning from CLRBindingTestClass.cs : 初始化CLR绑定请放在初始化的最后一步
        // 绑定文件【本质也是重定向】
        RegisterBinding(appDomain);

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        appDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
#if UNITY_EDITOR
        appDomain.DebugService.StartDebugService(56000);
#endif
    }

    private static void RegisterDelegate(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
        ILRuntime.Runtime.Generated.CLRManualDelegates.Initialize(appDomain);
        // todo: 将来由DelegateCodeGenerater.cs生成
        // ILRuntime.Runtime.Generated.CLRAnalysisDelegates.Initialize(appDomain);
    }

    private static void RegisterAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
        ILRuntime.Runtime.Generated.CLRManualAdapterRegistion.Register(appDomain);
        ILRuntime.Runtime.Generated.CLRAnalysisAdapterRegistion.Register(appDomain);
    }

    private static void RegisterBinder(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
        appDomain.RegisterValueTypeBinder(typeof(UnityEngine.Vector2), new Vector2Binder());
        appDomain.RegisterValueTypeBinder(typeof(UnityEngine.Vector3), new Vector3Binder());
        appDomain.RegisterValueTypeBinder(typeof(UnityEngine.Quaternion), new QuaternionBinder());

        // 后续项目新增
        appDomain.RegisterValueTypeBinder(typeof(UnityEngine.Color), new ColorBinder());
        // appDomain.RegisterValueTypeBinder(typeof(DateTime), new DateTimeBinder());
        // appDomain.RegisterValueTypeBinder(typeof(TimeSpan), new TimeSpanBinder());
    }

    private static void RegisterRedirection(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
        ILRuntime.Runtime.Generated.CLRManualRedirection.Register(appDomain);
    }

    private static void RegisterBinding(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
        // 必须先注册手动
        // 再注册自动， 因为同种类型注册，后面注册的不会覆盖前面注册的
        ILRuntime.Runtime.Generated.CLRManualBindings.Initialize(appDomain);
        // 如果这里编译报错，则暂时注释，然后回到unity点击菜单栏的Tools，生成绑定文件
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);
    }
}
