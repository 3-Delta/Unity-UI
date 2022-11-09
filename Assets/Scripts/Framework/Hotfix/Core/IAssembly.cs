using System;

public interface IAssembly {
    void Load();
    void Clear();
    Type[] GetTypes();
    Type GetType(string typeNameWithNamespace);
    
    object CreateInstance(string typeNameIncludeNamespace);
    StaticMethod CreateStaticMethod(string typeNameWithNamespace, string methodName, int argCount);
    InstanceMethod CreateInstanceMethod(string typeNameWithNamespace, string methodName, ref object refInstance, int argCount);
}
