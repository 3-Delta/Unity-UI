using System;
using System.Collections.Generic;

public class PreCurrent<T> {
    public T last { get; private set; } = default;
    public T current { get; private set; } = default;
    public Action<T, T> onChanged = null;

    public PreCurrent() { }

    public PreCurrent(Action<T, T> onChanged) {
        this.onChanged = onChanged;
    }

    public PreCurrent<T> Set(T current) {
        if (EqualityComparer<T>.Default.Equals(this.current, current)) {
            this.last = this.current;
            this.current = current;
            onChanged?.Invoke(this.last, this.current);
        }

        return this;
    }

    // for reuse
    public PreCurrent<T> Reset() {
        this.last = default;
        this.current = default;
        return this;
    }
}
