using System;
using System.IO;

// https://github.com/Ourpalm/ILRuntimeU3D
public static class IOService
{
    public static byte[] GetFileBytes(string filePath)
    {
        byte[] ret = null;
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            ret = File.ReadAllBytes(filePath);
        }
        return ret;
    }
}