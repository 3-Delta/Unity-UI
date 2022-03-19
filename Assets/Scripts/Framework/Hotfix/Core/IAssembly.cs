using System;

public interface IAssembly {
    void Load();
    void Clear();
    Type[] GetTypes();

    object CreateInstance(string fullName);
    StaticMethod CreateStaticMethod(string typeNameIncludeNamespace, string methodName, int argCount);
    InstanceMethod CreateInstanceMethod(string typeNameIncludeNamespace, string methodName, ref object refInstance, int argCount);
}
