using System.Threading;

/*
    内核当发生访问资源冲突的时候，可以有两种锁的解决方案选择：

    一个是原地等待
    一个是挂起当前进程，调度其他进程执行（睡眠）
    Spinlock 是内核中提供的一种比较常见的锁机制，自旋锁是“原地等待”的方式解决资源冲突的，
    即，一个线程获取了一个自旋锁后，另外一个线程期望获取该自旋锁，获取不到，只能够原地“打转”（忙等待）。
    由于自旋锁的这个忙等待的特性，注定了它使用场景上的限制 —— 自旋锁不应该被长时间的持有（消耗 CPU 资源）。
*/

// 自旋锁：当前线程需要的资源被其他线程锁定使用的时候，该线程一直等待
// 系统自带的自旋锁会切换上下文，自己实现的自旋锁只是循环而已
public static class SpinLock {
    // lockValue初始值必须为0
    public static void Lock(ref int lockValue) {
        // 如果lockValue为1，表示被其他线程占用，就等待
        // 否则自己占用，同时修改lockValue为1
        // 也就是 lockValue 与 第三个参数0比较，相等则设置为第二个参数1，否则

        // 比较location1与comparand，如果不相等，什么都不做；如果location1与comparand相等，则用value替换location1的值。
        // 无论比较结果相等与否，返回值都是location1中原有的值， 如果不是返回原始值，则永远跳不出循环了
        while (Interlocked.CompareExchange(ref lockValue, 1, 0) == 1) {
            /*
            一：Thread.Sleep(1000);
                Thread.Sleep()方法：是强制放弃CPU的时间片，然后重新和其他线程一起参与CPU的竞争。

            二：Thread.SpinWait(1000);
                Thread.SpinWait()方法：只是让CPU去执行一段没有用的代码。当时间结束之后能马上继续执行，而不是重新参与CPU的竞争。

            用Sleep()方法是会让线程放弃CPU的使用权。
            用SpinWait()方法是不会放弃CPU的使用权。
            */
            Thread.SpinWait(1);
        }
    }

    /// <summary>
    /// 解锁，将计数器设置为初始值0
    /// </summary>
    /// <param name="lockValue"></param>
    public static void Unlock(ref int lockValue) {
        Interlocked.Exchange(ref lockValue, 0);
    }
}
