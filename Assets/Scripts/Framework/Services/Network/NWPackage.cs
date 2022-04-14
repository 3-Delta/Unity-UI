using System;
using System.Net;

/// <summary>
/// 之前存在一个疑问：就是为什么只是针对于NW_PackageHead的size和protoType进行大小端的转换，而不对于NW_PackageBody的bodyBytes进行大小端的转换呢？
/// 首先需要知道大小端其实是对于大于一个字节的数据类型的内部字节顺序的问题。那么后面的因为都是byte，都是一个字节，所以就不需要进行大小端的转换。
/// </summary>
public struct NWPackageHead
{
    // 2字节,控制单个包体大小[不包括包头], 最大32768*2字节,
    public ushort msgSize;
    public ushort msgType;
    public ushort sequence; // serverID * 10000 + playerID

    //public bool GM; // gm消息

    public NWPackageHead(ushort protoType, ushort size, ushort sequence)
    {
        this.msgType = protoType;
        this.msgSize = size;
        this.sequence = sequence;
    }
    public byte[] Encode()
    {
        // 针对大小端设备统一进行字节顺序转换
        short t = IPAddress.HostToNetworkOrder((short)msgSize);
        byte[] sizeBytes = BitConverter.GetBytes((ushort)t);
        t = IPAddress.HostToNetworkOrder((short)msgType);
        byte[] typeBytes = BitConverter.GetBytes((ushort)t);
        t = IPAddress.HostToNetworkOrder((short)sequence);
        byte[] sequenceBytes = BitConverter.GetBytes((ushort)t);

        int byteCount = NWDef.PACKAGE_HEAD_SIZE;
        byte[] headBytes = new byte[byteCount];

        Buffer.BlockCopy(sizeBytes, 0, headBytes, 0, sizeBytes.Length);
        Buffer.BlockCopy(typeBytes, 0, headBytes, sizeof(ushort), typeBytes.Length);
        Buffer.BlockCopy(sequenceBytes, 0, headBytes, sizeof(ushort) + sizeof(ushort), sequenceBytes.Length);

        return headBytes;
    }
    public void Decode(byte[] bytes, int startIndex, int endIndex)
    {
        if (bytes != null && 0 <= startIndex && startIndex <= endIndex && endIndex < bytes.Length)
        {
            msgSize = BitConverter.ToUInt16(bytes, startIndex);
            msgSize = (ushort)IPAddress.NetworkToHostOrder((short)msgSize);
            msgType = BitConverter.ToUInt16(bytes, startIndex + sizeof(short));
            msgType = (ushort)IPAddress.NetworkToHostOrder((short)msgType);
            sequence = BitConverter.ToUInt16(bytes, startIndex + sizeof(ushort) + sizeof(ushort));
            sequence = (ushort)IPAddress.NetworkToHostOrder((short)sequence);
        }
    }
}

public struct NWPackageBody
{
    // bodyBytes的长度不定，但是存在一个上限值，如果超过上限值，如何处理？
    public byte[] bodyBytes;

    public NWPackageBody(byte[] bytes, int startIndex, int endIndex)
    {
        bodyBytes = new byte[0];
        Decode(bytes, startIndex, endIndex);
    }
    public byte[] Encode() { return bodyBytes; }
    public void Decode(byte[] bytes, int startIndex, int endIndex)
    {
        if (bytes != null && 0 <= startIndex && startIndex <= endIndex && endIndex < bytes.Length)
        {
            int bodySize = endIndex - startIndex + 1;
            bodyBytes = new byte[bodySize];
            Buffer.BlockCopy(bytes, startIndex, bodyBytes, 0, bodySize);
        }
    }
}

public struct NWPackage
{
    public NWPackageHead head;
    public NWPackageBody body;

    public NWPackage(ushort protoType, byte[] bytes, ushort sequence)
    {
        head = new NWPackageHead(protoType, (ushort)bytes.Length, sequence);
        body = new NWPackageBody(bytes, 0, bytes.Length - 1);
        body.Decode(bytes, 0, bytes.Length - 1);
    }
    public void Clear() { }
    public byte[] Encode()
    {
        int bodySize = body.bodyBytes.Length < 0 ? 0 : body.bodyBytes.Length;
        byte[] totalBytes = new byte[NWDef.PACKAGE_HEAD_SIZE + bodySize];
        byte[] headBytes = head.Encode();
        byte[] bodyBytes = body.Encode();
        Buffer.BlockCopy(headBytes, 0, totalBytes, 0, headBytes.Length);
        if (bodySize > 0)
        {
            Buffer.BlockCopy(bodyBytes, 0, totalBytes, headBytes.Length, bodySize);
        }
        return totalBytes;
    }
    public void Decode(byte[] bytes)
    {
        head.Decode(bytes, 0, NWDef.PACKAGE_HEAD_SIZE - 1);
        body.Decode(bytes, NWDef.PACKAGE_HEAD_SIZE, bytes.Length - 1);
    }
}
