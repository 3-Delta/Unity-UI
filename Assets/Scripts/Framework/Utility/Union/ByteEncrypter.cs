using System;
using System.Runtime.InteropServices;

public class ByteEncrypter {
    [DllImport("Encrypt")]
    public static extern void Encrypt(byte[] array, int len);

    [DllImport("Encrypt")]
    public static extern void Decrypt(byte[] array, int len);
}
