
using System;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public abstract class WatchContext : IWatchContext
    {
        public IWatchContext Parent;
        public string RelativePath;
        protected ContextMemberInfo memberInfo;
        public ContextMemberInfo MemberInfo { get { return memberInfo; } }
        public WatchContext(IWatchContext parent, string relativePath, ContextMemberInfo info)
        {
            Parent = parent;
            RelativePath = relativePath;
            memberInfo = info;
        }
        public static bool TryParseDeepest(IWatchContext ctxFirst, string path, ref RangeInt cursor, out IWatchContext ctxResult)
        {
            var c = cursor;
            IWatchContext curCtx = ctxFirst;
            while (c.length > 0)
            {
                var nextCursor = c;
                if (ParserUtils.TryExtractPathPart(path, ref nextCursor, out var partRange))
                {
                    if (curCtx.TryParse(path, ref partRange, out var nextCtx))
                    {
                        c = nextCursor;
                        curCtx = nextCtx;
                        continue;
                    }
                }
                break;
            }
            cursor = c;
            ctxResult = curCtx;
            return true;
        }

        public bool TryParseDeepest(string path, ref RangeInt cursor, out IWatchContext ctx)
        {
            return TryParseDeepest(this, path, ref cursor, out ctx);
        }
        public abstract bool TryParse(string path, ref RangeInt cursor, out IWatchContext ctx);
        public virtual string Scope(string path)
        {
            if (Parent != null)
            {
                return Parent.Scope(RelativePath + path);
            }
            return RelativePath + path;
        }
        public bool TryCreateWatch(string path, RangeInt cursor, out IWatch watch)
        {

            if (WatchContext.TryParseDeepest(this, path, ref cursor, out var ctrxResult))
            {
                return ctrxResult.TryCreateWatch(out watch);
            }
            watch = null;
            return false;
        }
        public virtual bool TryCreateWatch(out IWatch watch)
        {
            watch = null;
            return false;
        }
        public virtual bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            return true;
        }
    }


}