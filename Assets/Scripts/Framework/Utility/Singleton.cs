using System;
using System.Collections.Generic;

public static class Singleton {
    private class Nested<T> where T : class, new() {
        public static readonly T instance = Activator.CreateInstance<T>();
    }

    // 内联热性
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T Instance<T>() where T : class, new() {
        return Nested<T>.instance;
    }
}

public abstract class Singleton<T> where T : new() {
    protected Singleton() {
    }

    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                _instance = System.Activator.CreateInstance<T>();
            }

            return _instance;
        }
    }

    public void Release() {
        _instance = default;
    }
}

public static class SingletonRegister<T> {
    private static HashSet<T> hash = new HashSet<T>();

    public static bool IsRegist(T ilType) {
        bool ret = ilType != null && hash.Contains(ilType);
        return ret;
    }

    public static bool Regist(T ilType) {
        if (ilType != null && !IsRegist(ilType)) {
            hash.Add(ilType);
            return true;
        }

        return false;
    }

    public static bool UnRegist(T ilType) {
        if (IsRegist(ilType)) {
            hash.Remove(ilType);
            return true;
        }

        return false;
    }

    public static void Clear() {
        hash.Clear();
    }
}
