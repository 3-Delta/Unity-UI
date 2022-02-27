using System.Threading;
using System;

// https://www.cnblogs.com/blqw/p/3475734.html
// 读写锁相比较lock的优势就是可以多个线程同时read,但是只能一个线程write。而lock不管是read还是write,都是只能一个线程进入临界区
public class UsingReadWriteLock<T> {
    #region 内部类
    // <summary> 利用IDisposable的using语法糖方便的释放锁定操作
    private struct Lock : IDisposable {
        private ReaderWriterLockSlim readWriteLock;

        // 是否为写入模式
        private bool isWrite;

        public Lock(ReaderWriterLockSlim rwl, bool isWrite) {
            this.readWriteLock = rwl;
            this.isWrite = isWrite;
        }

        public void Dispose() {
            if (isWrite) {
                if (readWriteLock.IsWriteLockHeld) {
                    readWriteLock.ExitWriteLock();
                }
            }
            else {
                if (readWriteLock.IsReadLockHeld) {
                    readWriteLock.ExitReadLock();
                }
            }
        }
    }

    // 空对象模式
    // 空的可释放对象,免去了调用时需要判断是否为null的问题
    private struct Disposable : IDisposable {
        public static readonly Disposable Empty = new Disposable();

        public void Dispose() { }
    }
    #endregion

    private ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

    private T data;

    // 是否启用,当该值为false时,Read()和Write()方法将返回 Disposable.Empty
    public bool enable { get; set; }

    public UsingReadWriteLock() {
        enable = true;
    }

    public UsingReadWriteLock(T data) : this() {
        this.data = data;
    }

    public T Data {
        get {
            // IsReadLockHeld 　　获取一个值，该值指示当前线程是否已进入读取模式的锁定状态。
            // IsWriteLockHeld    获取一个值，该值指示当前线程是否已进入写入模式的锁定状态。
            if (rwl.IsReadLockHeld || rwl.IsWriteLockHeld) {
                return data;
            }

            throw new MemberAccessException("请先进入读取或写入锁定模式再进行操作");
        }
        set {
            if (rwl.IsWriteLockHeld) {
                data = value;
            }
        }
    }

    public IDisposable Read() {
        if (enable == false || rwl.IsReadLockHeld || rwl.IsWriteLockHeld) {
            return Disposable.Empty;
        }
        else {
            rwl.EnterReadLock();
            return new Lock(rwl, false);
        }
    }

    public IDisposable Write() {
        if (enable == false || rwl.IsWriteLockHeld) {
            return Disposable.Empty;
        }
        //else if (rwl.IsReadLockHeld)
        //{
        //    // nothing todo
        //}
        else {
            rwl.EnterWriteLock();
            return new Lock(rwl, true);
        }
    }
}
