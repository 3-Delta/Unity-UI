using System;

// 引用计数
public class RefCounter {
    private int _refCount = 0;
    public int refCount {
        get {
            return _refCount;
        }
        protected set {
            _refCount = value;
            if (_refCount <= 0) {
                onZero?.Invoke();
            }
        }
    }

    public bool Using {
        get {
            return refCount > 0;
        }
    }

    public Action onZero { get; set; }

    public void Reset() {
        onZero = null;
        _refCount = 0;
    }

    // 后置++的实现方式， 编译器会根据后置++的实现方式自动实现前置++的实现，所以我们实现不了前置++
    public static RefCounter operator ++(RefCounter self) {
        self.refCount++;
        return self;
    }

    public static RefCounter operator --(RefCounter self) {
        if (self.refCount > 0) {
            self.refCount--;
        }

        return self;
    }
}