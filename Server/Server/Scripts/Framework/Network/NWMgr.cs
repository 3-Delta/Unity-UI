using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

// https://github.com/Headbangeerr/ForestWar/blob/master/GameServer/GameServer/Servers/Client.cs
// https://www.jianshu.com/p/fa959d16eaed
public class NWMgr : ManagerBase<NWMgr>
{
    private Socket socket;
    public Dictionary<ulong, NWTransfer> clients { get; private set; } = new Dictionary<ulong, NWTransfer>();
    public Dictionary<NWTransfer, ulong> transfers { get; private set; } = new Dictionary<NWTransfer, ulong>();

    public override void OnInit()
    {
        Listen(NWDef.IPv4, NWDef.PORT, NWDef.ListenMax);
    }
    public override void OnExit()
    {
        foreach (var kvp in transfers) { kvp.Key.OnExit(); }
        transfers.Clear();
        clients.Clear();
        socket?.Close();
        socket = null;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        // 清除已经断线的socket
        foreach (var kvp in clients)
        {
            // 心跳时长60秒
            if (kvp.Value.lastInteractTime + 60000 < TimeMgr.Instance.time)
            {
                OnConnectLost(kvp.Value);
            }
        }
    }

    #region // 连接
    public void Listen(string ip, int port, int listenCount) { Listen(IPAddress.Parse(ip), port, listenCount); }
    public void Listen(IPAddress ip, int port, int listenCount) { Listen(new IPEndPoint(ip, port), listenCount); }
    public void Listen(IPEndPoint ipe, int listenCount)
    {
        socket = NetworkUtils.BuildSocket4TCP(ipe.AddressFamily);
        try
        {
            socket.Bind(ipe);
            socket.Listen(listenCount);
            LogMgr.Log("Server Network Launch Success! Start Listenning...");
            socket.BeginAccept(new System.AsyncCallback(OnAccepted), null);
        }
        catch (Exception e)
        {
            LogMgr.LogError("Listen Failed : " + e.Message);
        }
    }
    #endregion

    #region // 回调
    private void OnAccepted(IAsyncResult ar)
    {
        try
        {
            // 取得客户端连接
            Socket client = socket.EndAccept(ar);
            LogMgr.Log("Server Network Accepted Client! Start Receiving... " + client.GetHashCode());
            new NWTransfer(client).BeginReceive();
        }
        catch (Exception e)
        {
            LogMgr.LogError("Connect Failed : " + e.Message);
        }

        // 继续监听其他客户端socket
        socket.BeginAccept(new System.AsyncCallback(OnAccepted), null);
    }
    private void OnConnectSuccess(NWTransfer transfer)
    {

    }
    private void OnConnectFailed(NWTransfer transfer)
    {

    }
    private void OnConnectLost(NWTransfer transfer)
    {
        if (transfer != null && transfers.ContainsKey(transfer))
        {
            LogMgr.Log("OnConnectLost" + transfer.ToString());
            ulong playerID = transfers[transfer];
            clients.Remove(playerID);
            transfers.Remove(transfer);
            transfer.DisConnect();
        }
    }
    #endregion

    #region // 发数据包
    public void Send(NWDef.ESendType sendType, ulong playerID, ushort protoType, byte[] bytes)
    {
        switch (sendType)
        {
            case NWDef.ESendType.Specified:
                clients[playerID]?.Send(playerID, protoType, bytes);
                break;
            case NWDef.ESendType.Other:
                foreach (var kvp in clients)
                {
                    if (playerID != kvp.Key)
                    {
                        kvp.Value?.Send(kvp.Key, protoType, bytes);
                    }
                }
                break;
            case NWDef.ESendType.All:
                foreach (var kvp in clients)
                {
                    kvp.Value?.Send(kvp.Key, protoType, bytes);
                }
                break;
        }
    }
    #endregion
}
