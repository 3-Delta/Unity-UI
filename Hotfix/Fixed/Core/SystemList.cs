using System.Collections.Generic;

namespace Logic.Hotfix.Fixed
{
    public static class SystemList
    {
        public readonly static List<ISysModule> list = new List<ISysModule>()
        {
            ServerTimeService.Instance,
            ReconnectService.Instance,

            SysAccount.Instance,
            SysTeam.Instance,
        };
    }
}
