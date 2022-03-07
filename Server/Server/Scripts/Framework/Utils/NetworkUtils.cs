using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public static class NetworkUtils
{
    public static bool IsIPv6(AddressFamily addressFamily) { return addressFamily == AddressFamily.InterNetworkV6; }
    public static Socket BuildSocket4TCP(AddressFamily addressFamily)
    {
        Socket socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        return socket;
    }
    public static string GetMacAddress()
    {
        string ret = "00-00-00-00-00-00";
        try
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                ret = BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                break;
            }
        }
        catch (Exception) { }
        return ret;
    }

    public static bool HasValue(int nullableEnum) { return nullableEnum != default(int); }
}
