using System;

// https://answer.uwa4d.com/question/62f5cfc8343a54268c841b28
// app所在的设备的信息
public static class DeviceService
{
    // 当前机器是64bit吗
    public static bool Is64bit => IntPtr.Size == 64;

    public static bool Is32bit => IntPtr.Size == 32;
}
