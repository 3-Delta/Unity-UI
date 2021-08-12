public class Progress {
    public int current { get; protected set; }
    public int limit { get; protected set; }

    public float progress {
        get { return current * 1f / limit; }
    }

    public bool IsReached {
        get { return progress >= 1f; }
    }

    public Progress() {
    }

    public Progress(int current, int limit) {
        Reset(current, limit);
    }

    public Progress Reset(int current, int limit) {
        this.current = current;
        this.limit = limit;
        return this;
    }
}