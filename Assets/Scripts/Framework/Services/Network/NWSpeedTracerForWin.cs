using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using UnityEngine;

// https://github.com/mdevoldere/NetworkSpeed
[Serializable]
public class NetState {
#if UNITY_EDITOR
    public bool IsUp;
#else
    public bool IsUp { get; protected set; }
#endif

    public NetState(bool isUp) {
        IsUp = isUp;
    }

    public NetState() : this(false) { }

    public NetState(OperationalStatus oStatus) : this(oStatus == OperationalStatus.Up) { }

    public NetState(NetworkInterface @interface) : this(@interface.OperationalStatus) { }

    public void SetState(OperationalStatus oStatus) {
        IsUp = (oStatus == OperationalStatus.Up);
    }
}

[Serializable]
public class NetAdapter {
#if UNITY_EDITOR
    public NetState State;
#else
    public NetState State { get; protected set; }
#endif

    public NetworkInterface NetInterface { get; protected set; }

#if UNITY_EDITOR
    [SerializeField]
#endif
    private string _Name;

#if UNITY_EDITOR
    [SerializeField]
#endif
    private string _Mac;

    public string Mac {
        get { return _Mac; }
    }

    public long Speed {
        get { return NetInterface.Speed / 1000000; }
    }

    public IPInterfaceProperties IpProperties { get; protected set; }

    public UnicastIPAddressInformation Ipv4Address { get; protected set; }

    public UnicastIPAddressInformation Ipv6Addess { get; protected set; }

    public string Ipv4 {
        get { return Ipv4Address?.Address.ToString() ?? "0.0.0.0"; }
    }

    public string Ipv4Mask {
        get { return Ipv4Address?.IPv4Mask.ToString() ?? "255.255.255.255"; }
    }

    public string Ipv4Prefix {
        get { return Ipv4Address?.PrefixLength.ToString() ?? "32"; }
    }

    public string Ipv6 {
        get { return Ipv6Addess?.Address.ToString() ?? "fe80::"; }
    }

    public string Ipv6Prefix {
        get { return Ipv6Addess?.PrefixLength.ToString() ?? "64"; }
    }

    public IPv4InterfaceStatistics IpStatistic { get; protected set; }

    private long _PreBytesSend = 0; // bytes sent storage
    private long _PreBytesReceived = 0; // bytes received storage

    public long BytesSent {
        get { return IpStatistic?.BytesSent ?? 0; }
    }

    public long BytesReceived {
        get { return IpStatistic?.BytesReceived ?? 0; }
    }

#if UNITY_EDITOR
    // b/s
    public long SpeedUp;
    public long SpeedDown;
#else
    public long SpeedUp { get; private set; } = 0;
    public long SpeedDown { get; private set; } = 0;
#endif

    public string StrSpeedUp {
        get { return SpeedUp.BytesFormat("B/s"); }
    }

    public string StrSpeedDown {
        get { return SpeedDown.BytesFormat("B/s"); }
    }

    public string StrTraffice {
        get { return ("Up: " + StrSpeedUp + " / Down: " + SpeedDown.BytesFormat("B/s")); }
    }

    public string UnicastPacketsSent {
        get { return IpStatistic?.UnicastPacketsSent.ToString() ?? "0"; }
    }

    public string UnicastPacketsReceived {
        get { return IpStatistic?.UnicastPacketsReceived.ToString() ?? "0"; }
    }

    public NetAdapter(NetworkInterface @interface, int id) {
        NetInterface = @interface;
        State = new NetState();

        _Name = NetInterface.Name ?? "UnknownNetwork";
        _Mac = NetInterface.GetPhysicalAddress().HumanReadable() ?? "00:00:00:00:00:00";

        Update();
    }

    public void Update() {
        State.SetState(NetInterface.OperationalStatus);
        IpProperties = NetInterface.GetIPProperties();

        if (IpProperties == null) {
            return;
        }

        foreach (UnicastIPAddressInformation ip in IpProperties.UnicastAddresses) {
            if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6 && Ipv6Addess == null) {
                Ipv6Addess = ip;
            }
            else if (ip.Address.AddressFamily == AddressFamily.InterNetwork && Ipv4Address == null) {
                Ipv4Address = ip;
            }
        }
    }

    public void UpdateTraffice() {
        if (State.IsUp) {
            IpStatistic = NetInterface?.GetIPv4Statistics();
            if (IpStatistic != null) {
                SpeedUp = (IpStatistic.BytesSent - _PreBytesSend);
                SpeedDown = (IpStatistic.BytesReceived - _PreBytesReceived);

                _PreBytesSend = IpStatistic.BytesSent;
                _PreBytesReceived = IpStatistic.BytesReceived;
            }
        }
    }

    private static StringBuilder sb = new StringBuilder();

    public override string ToString() {
        sb.Clear();

        sb.Append(NetInterface.Name).Append(" - ").Append(Speed.ToString()).AppendLine(" Mbps");
        sb.AppendLine(Mac).Append(Ipv4).Append("/").AppendLine(Ipv4Prefix);
        sb.AppendLine(Ipv4Mask);

        return sb.ToString();
    }
}

[DisallowMultipleComponent]
public class NWSpeedTracerForWin : MonoBehaviour {
    public static void GetNetworks(in List<NetAdapter> NetAdapters) {
        NetAdapters.Clear();
        var netArray = NetworkInterface.GetAllNetworkInterfaces();
        for (int i = 0, length = netArray.Length; i < length; i++) {
            NetworkInterface nic = netArray[i];
            if (nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback) {
                NetAdapter na = new NetAdapter(nic, i);
                na.Update();
                NetAdapters.Add(na);
            }
        }
    }

    public double NetInterval = 15000;
    public double NetSpeedInterval = 3000;
    protected Timer NetTimer = new Timer { Enabled = false };
    protected Timer NetSpeedTimer = new Timer { Enabled = false };
#if UNITY_EDITOR
    [SerializeField] protected List<NetAdapter> NetAdapters = new List<NetAdapter>();
#else
    protected readonly List<NetAdapter> NetAdapters = new List<NetAdapter>();
#endif

    private void Awake() {
        NetTimer.AutoReset = true;
        NetSpeedTimer.AutoReset = true;

        NetTimer.Elapsed += OnNetTimed;
        NetSpeedTimer.Elapsed += OnNetSpeedTimed;
    }

    private void OnDestroy() {
        NetTimer.Elapsed -= OnNetTimed;
        NetSpeedTimer.Elapsed -= OnNetSpeedTimed;

        End();

        NetTimer.Dispose();
        NetSpeedTimer.Dispose();
    }

    [ContextMenu(nameof(Begin))]
    public void Begin() {
        GetNetworks(in NetAdapters);

        NetTimer.Interval = NetInterval;
        NetSpeedTimer.Interval = NetSpeedInterval;

        NetTimer.Start();
        NetSpeedTimer.Start();
    }

    [ContextMenu(nameof(End))]
    public void End() {
        NetTimer.Stop();
        NetSpeedTimer.Stop();
    }

    private void OnNetTimed(object source, ElapsedEventArgs e) {
        foreach (NetAdapter ni in NetAdapters) {
            if (ni != null) {
                ni.Update();
            }
        }
    }

    private void OnNetSpeedTimed(object source, ElapsedEventArgs e) {
        foreach (NetAdapter ni in NetAdapters) {
            if (ni != null) {
                ni.UpdateTraffice();
            }
        }
    }
}
