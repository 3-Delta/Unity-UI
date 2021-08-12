// 为了解决：timer, 协程等在UI的ondestroy自动cancel的机制管理

using System.Collections.Generic;

public interface ICancelable {
    void Cancel();
}

public interface IHolder<T> : ICancelable {
    IList<T> CancelableCollection { get; set; }
}

// UIBase继承，并且实现这个
public interface ICancelableHolder : IHolder<ICancelable> {
    void AddCancelable(ICancelable item);
    //void RemoveCancelable(T item);
}

// 类形式
public class CancelableC<T> : ICancelable where T : class, ICancelable {
    protected T value; // 比如：Timer timer;

    public CancelableC(T value, ICancelableHolder holder) {
        this.value = value;
        holder?.AddCancelable(this);
    }

    public void Cancel() {
        value?.Cancel();
    }
}

// 结构体形式
public class CancelableSt<T> : ICancelable where T : class, ICancelable {
    protected T value; // 比如：Timer timer;

    public CancelableSt(T value, ICancelableHolder holder) {
        this.value = value;
        holder?.AddCancelable(this);
    }

    public void Cancel() {
        value?.Cancel();
    }
}