using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

// https://www.jianshu.com/p/313cd68f5594
public class LogMgr : ManagerBase<LogMgr>
{
    private const string LogTAG = "[Log]";
    private const string LogWarnTAG = "[Warn]";
    private const string LogErrorTAG = "[Error]";

    public static void Log(string msg, params object[] values) { LogMessage(ConsoleColor.Green, LogTAG + GetStackFrameLocationInfo() + msg, values); }
    public static void LogWarn(string msg, params object[] values) { LogMessage(ConsoleColor.Yellow, LogWarnTAG + GetStackFrameLocationInfo() + msg, values); }
    public static void LogError(string msg, params object[] values) { LogMessage(ConsoleColor.Red, LogErrorTAG + GetStackFrameLocationInfo() + msg, values); }
    private static void LogMessage(ConsoleColor color, string msg, params object[] values)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg, values);
    }
    public static String GetStackInfo()
    {
        StackTrace stackTrace = new StackTrace(2, true);
        return String.Format(@" [{0}]: {1} ", DateTime.Now.ToLongTimeString(), stackTrace.ToString());
    }
    public static String GetStackFrameLocationInfo()
    {
        StackTrace stackTrace = new StackTrace(1, true);
        StackFrame stackFrame = stackTrace.GetFrame(3);
        if (stackFrame == null) { return "[unknownClass]"; }
        string className = Path.GetFileNameWithoutExtension(stackFrame.GetFileName());
        return String.Format(@" [{0} {1} -> {2} line->({3} line:{4} column)]: ", DateTime.Now.ToLongTimeString(), className, stackFrame.GetMethod(), stackFrame.GetFileLineNumber(), stackFrame.GetFileColumnNumber());
    }
}
