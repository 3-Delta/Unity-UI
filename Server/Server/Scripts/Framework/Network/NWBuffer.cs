using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public class NWBuffer
{
    public NWPackage package = new NWPackage();
    public int realLength { get; set; } = 0;
    // 缓冲区尽量大，数据接收丢失
    public byte[] buffer { get; set; } = new byte[NWDef.PACKAGE_MAX_SIZE * 2];

    public NWBuffer()
    {
        this.Clear();
    }
    public void Clear()
    {
        realLength = 0;
        package.Clear();
    }
}
