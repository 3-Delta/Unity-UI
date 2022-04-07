using Logic.Pbf;

using System;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;

// 单个transfer,在服务器模式中，代表一个与服务器连接的客户端
public class NWTransfer
{
    public Socket socket = null;
    public bool IsConnected { get { return socket != null && socket.Connected; } }
    public NWQueue receivedQueue { get; private set; } = new NWQueue();
    public ulong lastInteractTime { get; private set; } // 心跳时间

    private Thread receivedThread = null;

    private void ReceivedThreadUpdate()
    {
        while (true)
        {
            System.Threading.Thread.Sleep(100);

            NWPackage package = new NWPackage();
            if (receivedQueue.Dequeue(ref package))
            {
                DelegateMgr<EMsgType>.Fire<ulong, NWPackageBody>((EMsgType)package.head.msgType, package.head.playerID, package.body);
            }
        }
    }
    public NWTransfer(Socket socket)
    {
        this.socket = socket;
        receivedThread = new Thread(new ThreadStart(ReceivedThreadUpdate));
        receivedThread.Start();
    }
    public void OnExit()
    {
#pragma warning disable SYSLIB0006 // 类型或成员已过时
        receivedThread?.Abort();
#pragma warning restore SYSLIB0006 // 类型或成员已过时
        DisConnect();
    }
    public void DisConnect()
    {
        socket?.Close();
        lastInteractTime = 0;
        socket = null;
    }
    public void BeginReceive()
    {
        NWBuffer buffer = new NWBuffer();
        // Receive系列函数，接收的size <= 我们预期的
        // socket.BeginReceive(buffer.buffer, 0, NW_Def.PACKAGE_HEAD_SIZE, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
        // or
        socket.BeginReceive(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE, SocketFlags.None, new AsyncCallback(OnReceivedPackage), buffer);
    }
    private void OnReceivedPackage(IAsyncResult ar)
    {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try
        {
            SocketError errCode = SocketError.Success;
            int read = socket.EndReceive(ar, out errCode);
            // 丢失连接
            if (read < 0)
            {
                DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
                LogMgr.LogError("OnReceivedPackage");
                return;
            }
            buffer.realLength += read;
            if (buffer.realLength < NWDef.PACKAGE_HEAD_SIZE)
            {
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedPackage), buffer);
            }
            else
            {
                buffer.package.head.Decode(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
                if (buffer.realLength < buffer.package.head.msgSize + NWDef.PACKAGE_HEAD_SIZE)
                {
                    socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, new AsyncCallback(OnReceivedBody), buffer);
                }
                else
                {
                    buffer.package.body.Decode(buffer.buffer, NWDef.PACKAGE_HEAD_SIZE, NWDef.PACKAGE_HEAD_SIZE + buffer.package.head.msgSize - 1);
                    TryProcessPackage(buffer);
                    receivedQueue.Enqueue(buffer.package);

                    // 保存已接收的数据
                    int remainLength = buffer.realLength - buffer.package.head.msgSize - NWDef.PACKAGE_HEAD_SIZE;
                    Buffer.BlockCopy(buffer.buffer, buffer.package.head.msgSize, buffer.buffer, 0, remainLength);
                    buffer.realLength = remainLength;
                    socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedPackage), buffer);
                }
            }
        }
        catch (Exception e)
        {
            DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
            LogMgr.LogError("OnReceivedPackage Failed : " + e.ToString());
        }
    }
    private void OnReceivedHead(IAsyncResult ar)
    {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try
        {
            SocketError errCode = SocketError.Success;
            int read = socket.EndReceive(ar, out errCode);
            // 丢失连接
            if (read < 0)
            {
                DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
                LogMgr.LogError("OnReceivedHead");
                return;
            }

            // 暂时将包头存储到buffer中，开始接受body的时候正式转移到head中
            buffer.realLength += read;
            // 包头必须读满
            if (buffer.realLength < NWDef.PACKAGE_HEAD_SIZE)
            {
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
            }
            else
            {
                // 处理包头
                buffer.package.head.Decode(buffer.buffer, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
                socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, new AsyncCallback(OnReceivedBody), buffer);
            }
        }
        catch (Exception e)
        {
            DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
            LogMgr.LogError("OnReceivedHead Failed : " + e.ToString());
        }
    }
    private void OnReceivedBody(IAsyncResult ar)
    {
        NWBuffer buffer = (NWBuffer)ar.AsyncState;
        try
        {
            SocketError errCode = SocketError.Success;
            int read = socket.EndReceive(ar, out errCode);
            // 断开连接
            if (read < 0)
            {
                DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
                LogMgr.LogError("OnReceivedBody");
                return;
            }

            buffer.realLength += read;
            // 消息体不满足长度
            if (buffer.realLength < buffer.package.head.msgSize + NWDef.PACKAGE_HEAD_SIZE)
            {
                socket.BeginReceive(buffer.buffer, buffer.realLength, buffer.package.head.msgSize - (buffer.realLength - NWDef.PACKAGE_HEAD_SIZE), SocketFlags.None, new AsyncCallback(OnReceivedBody), buffer);
            }
            else
            {
                // 入队
                buffer.package.body.Decode(buffer.buffer, NWDef.PACKAGE_HEAD_SIZE, NWDef.PACKAGE_HEAD_SIZE + buffer.package.head.msgSize - 1);
                TryProcessPackage(buffer);
                receivedQueue.Enqueue(buffer.package);

                // 保存已接收的数据
                int remainLength = buffer.realLength - buffer.package.head.msgSize - NWDef.PACKAGE_HEAD_SIZE;
                Buffer.BlockCopy(buffer.buffer, buffer.package.head.msgSize, buffer.buffer, 0, remainLength);
                buffer.realLength = remainLength;
                socket.BeginReceive(buffer.buffer, buffer.realLength, NWDef.PACKAGE_HEAD_SIZE - buffer.realLength, SocketFlags.None, new AsyncCallback(OnReceivedHead), buffer);
            }
        }
        catch (Exception e)
        {
            DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
            LogMgr.LogError("OnReceivedBody Failed : " + e.ToString());
        }
    }
    private void TryProcessPackage(NWBuffer buffer)
    {
        if (buffer != null)
        {
            // to do: 账号互顶的问题[playerID换设备登录]
            lastInteractTime = TimeMgr.Instance.time; // 记录交互时间

            if ((EMsgType)buffer.package.head.msgType == EMsgType.Cslogin)
            {
                NWMgr.Instance.clients.Add(buffer.package.head.playerID, this);
                NWMgr.Instance.transfers.Add(this, buffer.package.head.playerID);
            }
            else if ((EMsgType)buffer.package.head.msgType == EMsgType.Cslogout)
            {
                NWMgr.Instance.clients.Remove(buffer.package.head.playerID);
                NWMgr.Instance.transfers.Remove(this);
            }
        }
    }

    #region // 收发数据
    public void Send(ulong playerID, ushort protoType, byte[] bytes)
    {
        if (!IsConnected) { return; }
        {
            // 必须要保证bytes的length要<=head中size的最大值，因为如果大于的话，将来在接收方 分包 的时候就会出现分包错误的问题
            if (bytes.Length > NWDef.PACKAGE_BODY_MAX_SIZE)
            {
                LogMgr.LogError("bytes too long!");
                return;
            }

            NWPackage package = new NWPackage(protoType, bytes);
            byte[] packageBytes = package.Encode();
            try
            {
                // https://blog.csdn.net/kucoffee12/article/details/86482332
                LogMgr.Log("Server Send Message To Client... " + (EMsgType)protoType + " " + playerID.ToString());
                socket.BeginSend(packageBytes, 0, packageBytes.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception e)
            {
                DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
                LogMgr.LogError("Send Failed : " + e.ToString());
            }
        }
    }
    private void OnSend(IAsyncResult ar)
    {
        Console.WriteLine("=========>>>>>>>>" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
        try { socket.EndSend(ar); }
        catch (System.Exception e)
        {
            DelegateMgr<EventType>.Fire<NWTransfer>(EventType.OnConnectLost, this);
            Console.WriteLine("EndSend Failed : " + e.ToString());
        }
    }
    #endregion
}
