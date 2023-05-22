using System;
using System.Collections.Generic;

public struct PreCurrent<T> {
    public T last { get; private set; }
    public T current { get; private set; }
    public Action<T, T> onChanged;

    public PreCurrent( T last, T current = default, Action<T, T> onChanged = null) {
        this.last = last;
        this.current = current;
        this.onChanged = onChanged;
    }

    public PreCurrent<T> Set(T target) {
    	this.last = this.current;
    	this.current = target;
    	
        if (!EqualityComparer<T>.Default.Equals(this.last, current)) {
            onChanged?.Invoke(this.last, this.current);
        }

        return this;
    }
}
