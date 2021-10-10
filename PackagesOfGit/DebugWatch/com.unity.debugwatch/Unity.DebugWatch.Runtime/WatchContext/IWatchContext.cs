
using System;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public interface IWatchContext
    {

        // returns if path string, at cursor, begins with a scope token this parser handles
        // cursor will point after the '.' after the matching token
        // cursor is left untouched if fails to parse.
        bool TryParse(string path, ref RangeInt cursor, out IWatchContext ctx);
        string Scope(string path);
        //bool TryCreateWatch(string path, RangeInt cursor, out IWatch watch);
        bool TryCreateWatch(out IWatch watch);
        bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor);

        ContextMemberInfo MemberInfo { get; }

    }

}