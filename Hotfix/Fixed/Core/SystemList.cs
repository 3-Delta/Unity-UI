using Logic.Hotfix;

using System.Collections;
using System.Collections.Generic;

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
