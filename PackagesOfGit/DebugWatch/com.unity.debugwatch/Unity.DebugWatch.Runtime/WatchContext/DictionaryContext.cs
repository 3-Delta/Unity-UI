
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public abstract class DictionaryContext : WatchContext
    {
        public DictionaryContext(IWatchContext parent, string relativePath, ContextMemberInfo info)
            : base(parent, relativePath, info)
        {
        }
        public abstract bool TryParseIndex(string path, RangeInt cursor, out IWatchContext ctx);
        public override bool TryParse(string path, ref RangeInt cursor, out IWatchContext ctx)
        {
            var c = cursor;
            if (ParserUtils.TryParseScopeSequenceRange(path, ref c, ParserUtils.DelemiterSquareBracket, out var seqIn, out var seqOut))
            {
                if (TryParseIndex(path, seqIn, out ctx))
                {
                    cursor = c;
                    return true;
                }
            }
            ctx = null;
            return false;
        }
        protected ContextMemberInfo MakeMemberInfo<TMemberType>(string index)
        {
            return new ContextMemberInfo(ContextFieldInfo.MakeOperator($"[{index}]"), ContextTypeInfo.Make(typeof(TMemberType)));
        }

    }


}