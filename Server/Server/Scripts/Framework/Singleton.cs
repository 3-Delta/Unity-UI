using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// c# 纯粹的单例模式，不能再外部new，只能Instance调用
// https://www.jianshu.com/p/4f76a420a490
public static class SingletonCreator
{
    public static T Create<T>() where T : class
    {
        // 获取无参私有构造函数
        T instance = null;
        ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        ConstructorInfo privateCtor = Array.Find(ctors, c => c.GetParameters().Length == 0);
        if (privateCtor != null)
        {
            instance = privateCtor.Invoke(null) as T;
        }
        return instance;
    }
}
public class Singleton<T> where T : class
{
    private static T instance = null;
    private static readonly object lockFlag = new object();

    protected Singleton() { }
    public static T Instance
    {
        get
        {
            // 这里将原来的new修改为Creator去使用反射创建
            if (instance == null)
            {
                lock (lockFlag)
                {
                    if (instance == null)
                    {
                        instance = SingletonCreator.Create<T>();
                    }
                }
            }
            return instance;
        }
    }
    public static void Release() { instance = null; }
}
