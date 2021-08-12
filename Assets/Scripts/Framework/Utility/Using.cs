using System;
using UnityEngine;

// 类形式
public class UsingC<T> : IDisposable {
    public T value { get; private set; }
    public Action<T> onDisposed;

    public UsingC(T value, Action<T> onDisposed = null) {
        this.value = value;
        this.onDisposed = onDisposed;
    }
    
    public void Dispose() {
        this.value = default;
        this.onDisposed = null;
        
        onDisposed?.Invoke(value);
    }

    // 测试用例
    public static void Test() {
        WWW www = new WWW(null);
        using (UsingC<WWW> u = new UsingC<WWW>(www)) {
            //  这里只是一个简单测试用例，主要目的就是为了防止这种一个preDo
            // 一个postdo的操作中间做doing的操作，然后遗漏某些操作。
            // 借助using调用dispose
        }
    }
}

// 结构体形式
public class UsingSt<T> : IDisposable where T {
    public T value { get; private set; }
    public Action<T> onDisposed;

    public UsingSt(T value, Action<T> onDisposed = null) {
        this.value = value;
        this.onDisposed = onDisposed;
    }

    public void Dispose() {
        this.value = default;
        this.onDisposed = null;
        
        onDisposed?.Invoke(value);
    }

    // 测试用例
    public static void Test() {
        WWW www = new WWW(null);
        using (UsingSt<WWW> u = new UsingSt<WWW>(www)) {
            //  这里只是一个简单测试用例，主要目的就是为了防止这种一个preDo
            // 一个postdo的操作中间做doing的操作，然后遗漏某些操作。
            // 借助using调用dispose
        }
    }
}
