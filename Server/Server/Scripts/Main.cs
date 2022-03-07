using System.Runtime.InteropServices;

class Program
{
    public delegate bool ControlCtrlDelegate(int CtrlType);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
    private static ControlCtrlDelegate CloseHandler = new ControlCtrlDelegate(HandlerRoutine);

    public static bool HandlerRoutine(int ctrlType)
    {
        switch (ctrlType)
        {
            case 0:
                LogMgr.Log("App exit by ctrl+c!");
                App.Exit();
                break;
            case 2:
                LogMgr.Log("App exit by close!");
                App.Exit();
                break;
        }
        return false;
    }


    public static void Main(string[] args)
    {
        // 监听Application关闭
        SetConsoleCtrlHandler(CloseHandler, true);

        App.Init();
        App.Update();
    }
}
