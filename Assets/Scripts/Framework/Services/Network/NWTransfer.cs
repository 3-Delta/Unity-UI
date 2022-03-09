using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

// 单个transfer, 将来可能多个transfer协作
// 需要做一些状态的控制，比如处于连接状态，不能再次连接
public class NWTransfer {
    private Socket socket = null;

    public bool IsConnected {
        get { return socket?.Connected ?? false; }
    }

    public bool IsConnecting { get; private set; } = false;

    public NWQueue receivedQueue { get; private set; } = new NWQueue();
    public NWQueue sendQueue { get; private set; } = new NWQueue();

    private NWBuffer buffer = new NWBuffer();
    private ManualResetEvent sendFlag = new ManualResetEvent(false);

    public void OnExit() {
        socket?.Shutdown(SocketShutdown.Both);
        socket?.Close();
    }

    #region // Connect
    public void Connect(string ip, int port, System.Action callback = null) {
        Connect(IPAddress.Parse(ip), port, callback);
    }

    public void Connect(IPAddress ip, int port, System.Action callback = null) {
        Connect(new IPEndPoint(ip, port), callback);
    }

    public void Connect(IPEndPoint ipe, System.Action callback = null) {
        socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        if (!IsConnected) {
            try {
                socket.BeginConnect(ipe, new AsyncCallback(OnConnected), null);
            }
            catch (Exception e) {
                DisConnect();
                UnityEngine.Debug.Log("Connect Failed : " + e.Message);
            }
        }
    }

    public void DisConnect() {
        socket?.Close();
        socket = null;
    }
    #endregion

    #region // OnTransfer
    private void OnConnected(IAsyncResult ar) {
        try {
            // 建立连接
            socket.EndConnect(ar);
            // 收发数据
            buffer.Clear();
            //socket.BeginReceive(buffer.buffer, 0, NW_Def.PACKAGE_HEAD_SIZE, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
            // or
            socket.BeginReceive(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE, SocketFlags.None, new AsyncCallback(OnReceivedPackage), buffer);
        }
        catch (Exception e) {
            DisConnect();
            UnityEngine.Debug.Log("OnConnected Failed : " + e.Message);
        }
    }

    private void OnReceivedPackage(IAsyncResult ar) {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try {
            SocketError errCode = SocketError.Success;
            int read = socket.EndReceive(ar, out errCode);
            // 丢失连接
            if (read < 0) {
                DisConnect();
                Console.WriteLine("OnReceivedPackage");
                return;
            }

            buffer.realLength += read;
            if (buffer.realLength < NWDef.PACKAGE_HEAD_SIZE) {
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedPackage), buffer);
            }
            else {
                buffer.package.head.Decode(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
                if (buffer.realLength < buffer.package.head.msgSize + NWDef.PACKAGE_HEAD_SIZE) {
                    socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, new AsyncCallback(OnReceivedBody), buffer);
                }
                else {
                    buffer.package.body.Decode(buffer.buffer, NWDef.PACKAGE_HEAD_SIZE, NWDef.PACKAGE_HEAD_SIZE + buffer.package.head.msgSize - 1);
                    receivedQueue.Enqueue(buffer.package);

                    // 保存已接收的数据
                    int remainLength = buffer.realLength - buffer.package.head.msgSize - NWDef.PACKAGE_HEAD_SIZE;
                    Buffer.BlockCopy(buffer.buffer, buffer.package.head.msgSize, buffer.buffer, 0, remainLength);
                    buffer.realLength = remainLength;
                    socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedPackage), buffer);
                }
            }
        }
        catch (Exception e) {
            DisConnect();
            Console.WriteLine("OnReceivedPackage Failed : " + e.ToString());
        }
    }

    private void OnReceivedHead(IAsyncResult ar) {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try {
            SocketError errCode = SocketError.Success;
            int read = socket.EndReceive(ar, out errCode);
            // 丢失连接
            if (read < 0) {
                DisConnect();
                UnityEngine.Debug.Log("OnReceivedHead");
                return;
            }

            // 暂时将包头存储到buffer中，开始接受body的时候正式转移到head中
            buffer.realLength += read;
            // 包头必须读满
            if (buffer.realLength < NWDef.PACKAGE_HEAD_SIZE) {
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
            }
            else {
                // 处理包头
                buffer.package.head.Decode(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
                socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, new AsyncCallback(OnReceivedBody), buffer);
            }
        }
        catch (Exception e) {
            DisConnect();
            UnityEngine.Debug.Log("OnReceivedHead Failed : " + e.ToString());
        }
    }

    private void OnReceivedBody(IAsyncResult ar) {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try {
            SocketError errCode = SocketError.Success;
            int read = socket.EndReceive(ar, out errCode);
            // 断开连接
            if (read < 0) {
                DisConnect();
                UnityEngine.Debug.Log("OnReceivedBody");
                return;
            }

            buffer.realLength += read;
            // 消息体不满足长度
            if (buffer.realLength < buffer.package.head.msgSize + NWDef.PACKAGE_HEAD_SIZE) {
                socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, new AsyncCallback(OnReceivedBody), buffer);
            }
            else {
                // 入队
                buffer.package.body.Decode(buffer.buffer, NWDef.PACKAGE_HEAD_SIZE, NWDef.PACKAGE_HEAD_SIZE + buffer.package.head.msgSize - 1);
                receivedQueue.Enqueue(buffer.package);

                // 保存已接收的数据
                int remainLength = buffer.realLength - buffer.package.head.msgSize - NWDef.PACKAGE_HEAD_SIZE;
                Buffer.BlockCopy(buffer.buffer, buffer.package.head.msgSize, buffer.buffer, 0, remainLength);
                buffer.realLength = remainLength;
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
            }
        }
        catch (Exception e) {
            DisConnect();
            UnityEngine.Debug.Log("OnReceivedBody Failed : " + e.ToString());
        }
    }
    #endregion

    #region // 收发数据
    public void Send(ushort protoType, byte[] bytes, ushort playerID, bool immediate) {
        // 必须要保证bytes的length要<=head中size的最大值，因为如果大于的话，将来在接收方 分包 的时候就会出现分包错误的问题
        if (IsConnected && bytes != null) {
            if (bytes.Length > NWDef.PACKAGE_BODY_MAX_SIZE) {
                Debug.LogError("bytes too long!");
                return;
            }

            NWPackage package = new NWPackage(protoType, bytes, playerID);
            if (immediate) {
                Send(ref package);
            }
            else {
                sendQueue.Enqueue(package);
            }
        }
    }

    private ushort lastSuccessedProtoType;

    private void Send(ref NWPackage package) {
        byte[] packageBytes = package.Encode();
        try {
            // 因为回调可能先于加锁语句执行，所以需要保证发送的原子性
            lock (this) {
                // 注意这里package装箱拆箱的风险
                socket.BeginSend(packageBytes, 0, packageBytes.Length, SocketFlags.None, new AsyncCallback(OnSend), package);
                // 阻塞，直到在发送成功之后Set，目的是为了控制网络底层的线程同步，因为这里会把主线程阻塞，也就是
                // 后面调用Send的时候是不会受理的。
                sendFlag.Reset();
                sendFlag.WaitOne();
            }
        }
        catch (Exception e) {
            DisConnect();
            Debug.LogError("Send Failed : " + package.head.msgType.ToString() + " " + e.ToString());
        }
    }

    private void OnSend(IAsyncResult ar) {
        NWPackage package = (NWPackage)ar.AsyncState;
        try {
            lastSuccessedProtoType = package.head.msgType;
            socket.EndSend(ar);
        }
        catch (Exception e) {
            DisConnect();
            Debug.LogError(string.Format("EndSend Failed CurrentProtoType:{0}, lastSuccessedProtoType:{1}, ErrorMessage:{2}", lastSuccessedProtoType.ToString(), package.head.msgType.ToString(), e.ToString()));
        }

        sendFlag.Set();
    }

    public void Update() {
        NWPackage package;
        if (sendQueue.Count > 0) {
            package = new NWPackage();
            if (sendQueue.Dequeue(ref package)) {
                Send(ref package);
            }
        }

        if (receivedQueue.Count > 0) {
            package = new NWPackage();
            if (receivedQueue.Dequeue(ref package)) {
                ushort protoType = (ushort)package.head.msgType;
                //DelegateMgr<ushort>.Fire<NW_Package>(protoType, package);
                NWDelegateService.Fire(protoType, package);
            }
        }
    }
    #endregion
}
