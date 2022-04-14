using System.Collections.Generic;
using System;
using Google.Protobuf;

// https://www.jianshu.com/p/fa959d16eaed
public class NWMgr : Singleton<NWMgr> {
    public NWTransfer transfer { get; private set; } = new NWTransfer();

    public void OnUpdate() {
        transfer?.Update();
    }

    public void OnExit() {
        transfer?.OnExit();
    }

    // private static Dictionary<Type, IMessage> messagePool = new Dictionary<Type, IMessage>();
    //
    // public static T GetMessage<T>() where T : class, IMessage, new() {
    //     Type type = typeof(T);
    //     IMessage result;
    //     if (!messagePool.TryGetValue(type, out result)) {
    //         result = new T();
    //     }
    //
    //     // 默认pb3的实现中没有clear或者reset等接口，需要修改生成pb的代码
    //     // https://edu.uwa4d.com/lesson-detail/165/850/0?isPreview=0
    //     // https://lab.uwa4d.com/lab/5e5f1acd8bab6aaf0285e0b0
    //     // 如何Reset??
    //     return result as T;
    // }
    //
    // public void ReleaseMessage<T>(T message) where T : IMessage {
    //     Type type = typeof(T);
    //     if (messagePool.ContainsKey(type)) {
    //         messagePool.Add(type, message);
    //     }
    // }

    public void Connect(string ip, int port, Action<EConnectStatus> callback = null) {
        transfer?.Connect(ip, port, callback);
    }

    public void Send(ushort protoType, byte[] bytes, bool immediate) {
        transfer?.Send(protoType, bytes, immediate);
    }
}
