using System.Collections.Generic;

namespace Logic.Hotfix.Fixed
{
    public static class SystemList
    {
        public readonly static List<ISysModule> list = new List<ISysModule>()
        {
            RemoteTimeService.Instance,
            ReconnectService.Instance,

            SysAccount.Instance,
            SysTeam.Instance,
        };
    }
}
