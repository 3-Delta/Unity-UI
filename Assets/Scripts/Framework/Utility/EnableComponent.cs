using System;

public class EnableComponent<T> where T : class {
    public T component { get; private set; }
    public bool enabled { get; set; } = false;

    public EnableComponent(T component) {
        Reset(component, false);
    }

    public EnableComponent<T> Reset(T component, bool enabled) {
        this.component = component;
        this.enabled = enabled;
        return this;
    }
}