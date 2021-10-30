public struct Progress {
    private int current;
    private int limit;

    public float progress {
        get { return current * 1f / limit; }
    }

    public bool IsReached {
        get { return progress >= 1f; }
    }

    public Progress(int current, int limit) {
        this.current = current;
        this.limit = limit;
    }

    public Progress Reset(int current, int limit) {
        this.current = current;
        this.limit = limit;
        return this;
    }
}
