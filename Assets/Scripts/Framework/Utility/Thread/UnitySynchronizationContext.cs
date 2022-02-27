using System;
using System.Threading;
using UnityEngine;

// 子线程执行unity的class
// 子线程通过线程上下文将子线程的操作action传递给unity主线程，这样
// 变相的就可以认为子线程可以执行unity的class.
// 其实和Loom的实现原理一样，loom是将action从子线程中抛给 多个线程共享的queue中，然后主线程去执行这个queue
// UnityEngine.UnitySynchronizationContext
public static class UnitySynchronizationContext {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init() {
        UnitySyncContext = SynchronizationContext.Current;
        UnityThreadId = Thread.CurrentThread.ManagedThreadId;
    }

    public static int UnityThreadId { get; private set; }
    public static SynchronizationContext UnitySyncContext { get; private set; }

    public static bool IsUnityThread {
        get { return ThreadService.CurrentThreadId == UnityThreadId; }
    }

    public static void ExecuteOnUnityScheduler(Action action, bool post = true) {
        // 当前线程上下文是unity主线程上下文，则直接执行，否则
        // 利用unity上下文转移到unity线程执行
        // 这部分逻辑其实在 UnitySyncContext 也会判断，所以这里注释掉
        // if (SynchronizationContext.Current == UnitySyncContext)
        // {
        //     action?.Invoke();
        // }
        // else
        {
            if (post) {
                UnitySyncContext.Post(_ => action?.Invoke(), null);
            }
            else {
                UnitySyncContext.Send(_ => action?.Invoke(), null);
            }
        }
    }
}
