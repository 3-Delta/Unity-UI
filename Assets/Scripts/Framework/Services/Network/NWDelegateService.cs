using System;
using Google.Protobuf;
using pb = Google.Protobuf;
using System.Collections.Generic;

public static class NWDelegateService {
    public class Handler {
        public ushort requestProtoType;
        public ushort responseProtoType;

        public Func<NWPackage, IMessage> converter;

        public Handler(ushort requestProtoType, ushort responseProtoType, Func<NWPackage, IMessage> converter) {
            this.requestProtoType = requestProtoType;
            this.responseProtoType = responseProtoType;
            this.converter = converter;
        }

        public void Handle(Action<IMessage> callback, bool toBeAdd) {
            emiter.Handle<IMessage>(responseProtoType, callback, toBeAdd);
        }

        public void Fire(NWPackage package) {
            if (converter != null) {
                IMessage msg = converter.Invoke(package);
                if (msg != null) {
                    emiter.Fire<IMessage>(responseProtoType, msg);
                }
            }
        }

        public override string ToString() {
            return $"request:{requestProtoType.ToString()} response:{responseProtoType.ToString()}";
        }
    }

    public static readonly DelegateService<ushort> emiter = new DelegateService<ushort>();

    // protoType : pb::IMessage
    private static readonly Dictionary<ushort, Handler> dict = new Dictionary<ushort, Handler>();

    // 设计目的：为了代码review者可以方便得到Request和Response的匹配关系
    // 这里记录requestProtoType的目的是：
    // 1:方便review者可以轻松匹配request和response的关系
    // 2:将来去实现网络等待光圈的时候可以利用这个request对应的response是否进行回复，如果回复，则关闭网络等待ui。 【WaitUI】
    public static void Add(ushort requestProtoType, ushort responseProtoType, Action<IMessage> callback, Func<NWPackage, IMessage> converter) {
        // parserDict
        if (!dict.TryGetValue(responseProtoType, out Handler handler)) {
            // converter只传递一次
            handler = new Handler(requestProtoType, responseProtoType, converter);
            dict.Add(responseProtoType, handler);
        }

        handler.Handle(callback, true);
    }

    public static void Remove(ushort responseProtoType, Action<IMessage> callback) {
        if (dict.TryGetValue(responseProtoType, out Handler handler)) {
            handler.Handle(callback, false);
        }
    }

    // 网络中使用这个Fire
    public static void Fire(ushort protoType, NWPackage package) {
        if (dict.TryGetValue(protoType, out Handler handler)) {
            handler.Fire(package);
        }
    }

    public static void Clear() {
        dict.Clear();
    }
}
