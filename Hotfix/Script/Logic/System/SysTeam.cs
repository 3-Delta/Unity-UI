public class SysTeam : SysBase<SysAccount> {
    public uint teamId = 0;
    public ulong camptainId = 0;

    public bool hasTeam {
        get { return teamId != 0; }
    }

    public bool isCamptain {
        get {
            // return camptainId == 账号id;
            return true;
        }
    }

    public bool CanOp {
        get { return ((!hasTeam) || (hasTeam && isCamptain)); }
    }
}
