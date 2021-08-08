using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class MD5Service {
    // 字符串获取
    public static string GetByString(string input) {
        string hashString = "";
        UTF8Encoding UTF8Encode = new UTF8Encoding();
        byte[] inputBytes = UTF8Encode.GetBytes(input);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }

    // 文件或者文件夾获取
    public static string GetByFile(string fileName) {
        try {
            FileStream reader = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            return GetByFile(reader);
        }
        catch (Exception ex) {
            throw new Exception("GetByFile() fail, error: " + ex.Message);
        }
    }

    // 文件或者文件夾获取
    public static string GetByFile(FileStream fileStream) {
        string hashString = "";
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(fileStream);

        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }
}
