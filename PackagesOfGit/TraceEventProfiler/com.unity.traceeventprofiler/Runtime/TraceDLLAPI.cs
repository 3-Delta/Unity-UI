using System.Runtime.InteropServices;

namespace TraceEventProfiler
{
    internal class TraceDLLAPI
    {
#if UNITY_IOS
        const string  profilerDllImportName = "__Internal";
#else
        const string profilerDllImportName = "traceeventprofiler";
#endif

        [DllImport(profilerDllImportName)]
        extern public static int BeginCapture(string filename, int maxMemoryMB);

        [DllImport(profilerDllImportName)]
        extern public static int EndCapture();

        [DllImport(profilerDllImportName)]
        extern public static int RegisterAsyncEvent(string eventName);

        [DllImport(profilerDllImportName)]
        extern public static int AcquireUniqueAsyncId();

        [DllImport(profilerDllImportName)]
        extern public static void AddAsyncEvent(bool isBeginEvent, int asyncEventId, int asyncEventInstanceId);

        [DllImport(profilerDllImportName)]
        extern public static void GetLastProfilerError(byte[] error, int maxLen);
    }
}