using System;
using Google.Protobuf;
using pb = Google.Protobuf;
using System.Collections.Generic;

public static class NWDelegateService {
    public interface IHandler {
        void Handle<T>(Action<T> callback, bool toBeAdd);
        void Fire(NWPackage package);
    }

    public class Handler<TMsg> : IHandler where TMsg : class, IMessage {
        public ushort requestProtoType;
        public ushort responseProtoType;

        public MessageParser parser;

        public Handler(ushort requestProtoType, ushort responseProtoType, MessageParser parser) {
            this.requestProtoType = requestProtoType;
            this.responseProtoType = responseProtoType;
            this.parser = parser;
        }

        // T 和 接口的 T这里是故意一样的，否则会编译错误
        public void Handle<TMsg>(Action<TMsg> callback, bool toBeAdd) {
            emiter.Handle<TMsg>(responseProtoType, callback, toBeAdd);
        }

        public void Fire(NWPackage package) {
            if (parser != null) {
                if (PbfSerializer.TryDeserialize<TMsg>(parser, package.body.bodyBytes, out TMsg msg)) {
                    emiter.Fire<TMsg>(responseProtoType, msg);
                }
            }
        }

        public override string ToString() {
            return $"request:{requestProtoType.ToString()} response:{responseProtoType.ToString()}";
        }
    }

    public static readonly DelegateService<ushort> emiter = new DelegateService<ushort>();

    // protoType : pb::IMessage
    private static readonly Dictionary<ushort, IHandler> dict = new Dictionary<ushort, IHandler>();

    // 设计目的：为了代码review者可以方便得到Request和Response的匹配关系
    // 这里记录requestProtoType的目的是：
    // 1:方便review者可以轻松匹配request和response的关系
    // 2:将来去实现网络等待光圈的时候可以利用这个request对应的response是否进行回复，如果回复，则关闭网络等待ui。 【WaitUI】
    public static void Add<T>(ushort requestProtoType, ushort responseProtoType, Action<T> callback, MessageParser parser) where T : class, IMessage {
        // parserDict
        if (!dict.TryGetValue(responseProtoType, out var handler)) {
            // converter只传递一次
            handler = new Handler<T>(requestProtoType, responseProtoType, parser);
            dict.Add(responseProtoType, handler);
        }

        handler.Handle(callback, true);
    }

    public static void Remove<T>(ushort responseProtoType, Action<T> callback) where T : class, IMessage {
        if (dict.TryGetValue(responseProtoType, out var handler)) {
            handler.Handle(callback, false);
        }
    }

    // 网络中使用这个Fire
    public static void Fire(ushort protoType, NWPackage package) {
        if (dict.TryGetValue(protoType, out var handler)) {
            handler.Fire(package);
        }
    }

    public static void Clear() {
        dict.Clear();
    }
}
