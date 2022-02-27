using System;
using System.IO;

// https://github.com/Ourpalm/ILRuntimeU3D
public static class PathService {
    public static string Combine(string left, string right, string extension = null) {
        string ret = null;
        if (left != null && right != null) {
            if (string.IsNullOrEmpty(extension)) {
                ret = string.Format("{0}/{1}", left, right);
            }
            else {
                ret = string.Format("{0}/{1}{2}", left, right, extension);
            }
        }

        return ret;
    }

    public static string ReplaceSlash(this string path) {
        if (!string.IsNullOrEmpty(path)) {
            path = path.Replace("\\", "/");
        }

        return path;
    }

    public static string AddWebPrefix(this string path) {
        if (path != null) {
            path = "file:///" + path;
        }

        return path;
    }

    public static byte[] GetFileBytes(string filePath) {
        byte[] ret = null;
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
            ret = File.ReadAllBytes(filePath);
        }

        return ret;
    }
}
