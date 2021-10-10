
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public class ScopeContext : WatchContext
    {
        protected Dictionary<string, IWatchContext> Variables = new Dictionary<string, IWatchContext>();
        public ScopeContext(IWatchContext parent, string relativePath, ContextMemberInfo info)
            : base(parent, relativePath, info)
        {
        }

        public void AddVariable(string name, IWatchContext ctx)
        {
            Variables.Add(name, ctx);
        }
        public override bool TryParse(string path, ref RangeInt cursor, out IWatchContext ctx)
        {
            var c = cursor;
            ParserUtils.TryParseAt(path, ref c, ".");
            foreach (var v in Variables)
            {
                if (ParserUtils.TryParseToken(path, ref c, v.Key))
                {
                    cursor = c;
                    ctx = v.Value;
                    return true;
                }
            }
            ctx = null;
            return false;
        }
        public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            foreach (var v in Variables)
            {
                if (!visitor(v.Value.MemberInfo)) return false;
            }
            return true;
        }
    }


}