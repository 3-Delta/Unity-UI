using System;
using System.Threading;

public class ThreadService {
    // 属性相比函数的一个好处是，断点调试的时候可以从表达式中快速得知结果，而函数则不能。
    public static int CurrentThreadId {
        get { return Thread.CurrentThread.ManagedThreadId; }
    }
}
