public interface IPreProcess {
    void PreProcess();
}

public interface IPostProcess {
    void PostProcess();
}

public interface IProcess : IPreProcess, IPostProcess {
}

public interface IPostProcess<T> {
    void PostProcess(T arg);
}

public interface IProcess<T> : IPreProcess, IPostProcess<T> {
}