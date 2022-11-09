using System;

public interface IAssembly {
    void Load();
    void Clear();
    Type[] GetTypes();
    
    object CreateInstance(string typeNameWithNamespace);
    StaticMethod CreateStaticMethod(string typeNameWithNamespace, string methodName, int argCount);
    InstanceMethod CreateInstanceMethod(string typeNameWithNamespace, string methodName, ref object refInstance, int argCount);
}
