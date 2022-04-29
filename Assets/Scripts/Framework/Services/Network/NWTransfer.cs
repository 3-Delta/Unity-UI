using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public enum EConnectStatus {
    UnConnected,
    Connecting, // 连接中
    Connected, // 连接成功
}

public enum EDisconnectType {
    Manual,
    ConnectException,
    SendException,
    ReceiveException,
}

// 单个transfer, 将来可能多个transfer协作
// 需要做一些状态的控制，比如处于连接状态，不能再次连接
[Serializable]
public class NWTransfer {
    public class Packer<T> {
        public T value;
    }

    private Socket socket = null;
    private Packer<NWPackage> packagePacker = new Packer<NWPackage>();

    [SerializeField] private EDisconnectType _disconnectType = EDisconnectType.Manual;

    public EDisconnectType DisconnectType {
        get { return _disconnectType; }
        private set { _disconnectType = value; }
    }

    [SerializeField] private EConnectStatus _connectStatus = EConnectStatus.UnConnected;

    public EConnectStatus ConnectStatus {
        get { return _connectStatus; }
        private set { _connectStatus = value; }
    }

    [SerializeField] private ushort _sendSequence = 0;

    public ushort SendSequence {
        get { return _sendSequence; }
        private set { _sendSequence = value; }
    }

    [SerializeField] private ushort _receiveSequence = 0;

    public ushort ReceiveSequence {
        get { return _receiveSequence; }
        private set { _receiveSequence = value; }
    }

    private ushort _lastSuccessedProtoType;

    public ushort LastSuccessedProtoType {
        get { return _lastSuccessedProtoType; }
        private set { _lastSuccessedProtoType = value; }
    }

    public Action<EConnectStatus> OnConnect;

    // 只有连接成功才有数值
    public IPEndPoint LocalEndPoint {
        get {
            if (IsConnected) {
                return socket.LocalEndPoint as IPEndPoint;
            }

            return null;
        }
    }

    // 只有连接成功才有数值
    public IPEndPoint RemoteEndPoint {
        get {
            if (IsConnected) {
                return socket.RemoteEndPoint as IPEndPoint;
            }

            return null;
        }
    }

    public bool IsConnected {
        get { return ConnectStatus == EConnectStatus.Connected; }
    }

    public bool IsConnecting {
        get { return ConnectStatus == EConnectStatus.Connecting; }
    }

    public NWQueue receivedQueue { get; private set; } = new NWQueue();
    public NWQueue sendQueue { get; private set; } = new NWQueue();

    private NWBuffer buffer = new NWBuffer();
    private ManualResetEvent sendFlag = new ManualResetEvent(false);

    public void OnExit() {
        DisConnect(EDisconnectType.Manual);
    }

#region Connect
    public void Connect(string ip, int port, Action<EConnectStatus> callback = null) {
        if (IPAddress.TryParse(ip, out IPAddress address)) {
            Connect(address, port, callback);
        }
        else {
            Debug.LogError("Ip格式不合法");
        }
    }

    public void Connect(IPAddress ip, int port, Action<EConnectStatus> callback = null) {
        Connect(new IPEndPoint(ip, port), callback);
    }

    public void Connect(IPEndPoint ipe, Action<EConnectStatus> callback = null) {
        if (ConnectStatus == EConnectStatus.UnConnected) {
            socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;

            // 超时针对同步方法，异步方法不生效
            // socket.ReceiveTimeout = 0;
            // socket.SendTimeout = 600;

            sendQueue.Clear();
            receivedQueue.Clear();
            OnConnect = callback;

            try {
                ConnectStatus = EConnectStatus.Connecting;
                socket.BeginConnect(ipe, OnConnected, socket);
                OnConnect?.Invoke(ConnectStatus);
            }
            catch (Exception e) {
                DisConnect(EDisconnectType.ConnectException);
                Debug.Log("Connect Failed : " + e.Message);
            }
        }
        else {
            Debug.LogError("已连接 或者 正在连接");
        }
    }

    // 潜在bug:因为发送不是即时的，会在下一帧发送，所以disconnect的时候，会有一些信息肯定发不出去
    // shutdown只是会保证已经在网卡中的数据会被接收和发送，而不是我们上层队列中的协议数据会被发送和接收。
    public void DisConnect(EDisconnectType disconnectType) {
        if (ConnectStatus == EConnectStatus.UnConnected) {
            return;
        }

        // 断开之前清理queue
        SendFromQueue();
        DispatchFromQueue();

        try {
            // https://docs.microsoft.com/zh-cn/dotnet/api/system.net.sockets.socket.close?view=net-6.0
            socket?.Shutdown(SocketShutdown.Both);
        }
        finally {
            socket?.Close();
            socket = null;

            DisconnectType = disconnectType;
            Debug.LogError("DisConnectType -> " + disconnectType.ToString());

            _connectStatus = EConnectStatus.UnConnected;
            _sendSequence = 0;

            sendQueue.Clear();
            receivedQueue.Clear();
            OnConnect?.Invoke(ConnectStatus);
        }
    }

    private void OnConnected(IAsyncResult ar) {
        try {
            if (socket.Connected) {
                ConnectStatus = EConnectStatus.Connected;

                // 建立连接
                socket.EndConnect(ar);

                OnConnect?.Invoke(ConnectStatus);

                // 收发数据
                buffer.Clear();
                //socket.BeginReceive(buffer.buffer, 0, NW_Def.PACKAGE_HEAD_SIZE, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
                // or
                socket.BeginReceive(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE, SocketFlags.None, OnReceivedPackage, buffer);
            }
            else {
                DisConnect(EDisconnectType.ConnectException);
                Debug.Log("OnConnected Failed : 超时");
            }
        }
        catch (Exception e) {
            DisConnect(EDisconnectType.ConnectException);
            Debug.Log("OnConnected Failed : " + e.Message);
        }
    }
#endregion

#region Package
    private void OnReceivedPackage(IAsyncResult ar) {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try {
            int size = socket.EndReceive(ar, out SocketError errCode);
            // 丢失连接
            if (size < 0 || errCode != SocketError.Success) {
                DisConnect(EDisconnectType.ReceiveException);
                Console.WriteLine("OnReceivedPackage");
                return;
            }

            buffer.realLength += size;
            if (buffer.realLength < NWDef.PACKAGE_HEAD_SIZE) {
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, OnReceivedPackage, buffer);
            }
            else {
                buffer.package.head.Decode(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
                if (buffer.realLength < buffer.package.head.msgSize + NWDef.PACKAGE_HEAD_SIZE) {
                    socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, OnReceivedBody, buffer);
                }
                else {
                    buffer.package.body.Decode(buffer.buffer, NWDef.PACKAGE_HEAD_SIZE, NWDef.PACKAGE_HEAD_SIZE + buffer.package.head.msgSize - 1);
                    receivedQueue.Enqueue(buffer.package);

                    // 保存已接收的数据
                    int remainLength = buffer.realLength - buffer.package.head.msgSize - NWDef.PACKAGE_HEAD_SIZE;
                    Buffer.BlockCopy(buffer.buffer, buffer.package.head.msgSize, buffer.buffer, 0, remainLength);
                    buffer.realLength = remainLength;
                    socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, OnReceivedPackage, buffer);
                }
            }
        }
        catch (Exception e) {
            DisConnect(EDisconnectType.ReceiveException);
            Console.WriteLine("OnReceivedPackage Failed : " + e.ToString());
        }
    }
#endregion

#region Head & Body
    private void OnReceivedHead(IAsyncResult ar) {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try {
            int size = socket.EndReceive(ar, out SocketError errCode);
            // 丢失连接
            if (size < 0 || errCode != SocketError.Success) {
                DisConnect(EDisconnectType.ReceiveException);
                Debug.Log("OnReceivedHead");
                return;
            }

            // 暂时将包头存储到buffer中，开始接受body的时候正式转移到head中
            buffer.realLength += size;
            // 包头必须读满
            if (buffer.realLength < NWDef.PACKAGE_HEAD_SIZE) {
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, OnReceivedHead, buffer);
            }
            else {
                // 处理包头
                buffer.package.head.Decode(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
                socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, OnReceivedBody, buffer);
            }
        }
        catch (Exception e) {
            DisConnect(EDisconnectType.ReceiveException);
            Debug.Log("OnReceivedHead Failed : " + e.ToString());
        }
    }

    private void OnReceivedBody(IAsyncResult ar) {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try {
            int size = socket.EndReceive(ar, out SocketError errCode);
            // 断开连接
            if (size < 0 || errCode != SocketError.Success) {
                DisConnect(EDisconnectType.ReceiveException);
                Debug.Log("OnReceivedBody");
                return;
            }

            buffer.realLength += size;
            // 消息体不满足长度
            if (buffer.realLength < buffer.package.head.msgSize + NWDef.PACKAGE_HEAD_SIZE) {
                socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, OnReceivedBody, buffer);
            }
            else {
                // 入队
                buffer.package.body.Decode(buffer.buffer, NWDef.PACKAGE_HEAD_SIZE, NWDef.PACKAGE_HEAD_SIZE + buffer.package.head.msgSize - 1);
                receivedQueue.Enqueue(buffer.package);

                // 保存已接收的数据
                int remainLength = buffer.realLength - buffer.package.head.msgSize - NWDef.PACKAGE_HEAD_SIZE;
                Buffer.BlockCopy(buffer.buffer, buffer.package.head.msgSize, buffer.buffer, 0, remainLength);
                buffer.realLength = remainLength;
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, OnReceivedHead, buffer);
            }
        }
        catch (Exception e) {
            DisConnect(EDisconnectType.ReceiveException);
            Debug.Log("OnReceivedBody Failed : " + e.ToString());
        }
    }
#endregion

#region 收发数据
    public void Send(ushort protoType, byte[] bytes) {
        // 必须要保证bytes的length要<=head中size的最大值，因为如果大于的话，将来在接收方 分包 的时候就会出现分包错误的问题
        if (IsConnected && bytes != null) {
            if (bytes.Length > NWDef.PACKAGE_BODY_MAX_SIZE) {
                Debug.LogError("bytes too long!");
                return;
            }

            NWPackage package = new NWPackage(protoType, bytes, ++SendSequence);
            sendQueue.Enqueue(package);
        }
    }

    private void Send(ref NWPackage package) {
        if (!IsConnected) {
            return;
        }

        byte[] packageBytes = package.Encode();
        try {
            // 因为回调可能先于加锁语句执行，所以需要保证发送的原子性
            lock (this) {
                // 注意这里package装箱拆箱的风险
                packagePacker.value = package;
                socket.BeginSend(packageBytes, 0, packageBytes.Length, SocketFlags.None, OnSend, packagePacker);
                // 阻塞，直到在发送成功之后Set，目的是为了控制网络底层的线程同步，因为这里会把主线程阻塞，也就是
                // 后面调用Send的时候是不会受理的。
                sendFlag.Reset();
                sendFlag.WaitOne();
            }
        }
        catch (Exception e) {
            DisConnect(EDisconnectType.SendException);
            Debug.LogError("Send Failed : " + package.head.msgType.ToString() + " " + e.ToString());
        }
    }

    private void OnSend(IAsyncResult ar) {
        Packer<NWPackage> packagePacker = ar.AsyncState as Packer<NWPackage>;
        NWPackage package = packagePacker.value;
        try {
            int size = socket.EndSend(ar, out SocketError errCode);
            LastSuccessedProtoType = package.head.msgType;
            if (size < 0 || errCode != SocketError.Success) {
                DisConnect(EDisconnectType.SendException);
                Debug.LogError(string.Format("EndSend Failed CurrentProtoType:{0}, LastSuccessedProtoType:{1}", LastSuccessedProtoType.ToString(), package.head.msgType.ToString()));
            }
        }
        catch (Exception e) {
            DisConnect(EDisconnectType.SendException);
            Debug.LogError(string.Format("EndSend Failed CurrentProtoType:{0}, LastSuccessedProtoType:{1}, ErrorMessage:{2}", LastSuccessedProtoType.ToString(), package.head.msgType.ToString(), e.ToString()));
        }
        finally {
            sendFlag.Set();
        }
    }

    private void SendFromQueue() {
        if (sendQueue.Count > 0) {
            NWPackage package = new NWPackage();
            if (sendQueue.Dequeue(ref package)) {
                Send(ref package);
            }
        }
    }

    private void DispatchFromQueue() {
        if (receivedQueue.Count > 0) {
            NWPackage package = new NWPackage();
            if (receivedQueue.Dequeue(ref package)) {
                // 防止来自server的数据包被截获，导致给客户端频繁发包
                if (package.head.sequence != ReceiveSequence) {
                    ReceiveSequence = package.head.sequence;
                    ushort protoType = package.head.msgType;
                    try {
                        NWDelegateService.Fire(protoType, package);
                    }
                    catch (Exception e) {
                        Debug.LogErrorFormat("There are errors {0} in msgType {1}", e, protoType.ToString());
                    }
                }
            }
        }
    }

    public void Update() {
        SendFromQueue();
        DispatchFromQueue();
    }
#endregion

}
