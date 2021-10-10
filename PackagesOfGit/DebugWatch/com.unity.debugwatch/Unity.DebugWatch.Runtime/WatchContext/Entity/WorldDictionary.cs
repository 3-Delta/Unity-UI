using System;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public class WorldDictionary : DictionaryContext
    {
        public WorldDictionary(IWatchContext parent, string relativePath, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(WorldDictionary)))
            )
        {
        }
        public override bool TryParseIndex(string path, RangeInt cursor, out IWatchContext ctx)
        {
            if (ParserUtils.TryParseScopeSequenceRange(path, ref cursor, ParserUtils.DelemiterQuote, out var worldNameRangeIn, out var worldNameRangeOut))
            {
                string worldName = path.Substring(worldNameRangeIn);
                foreach (var w in Entities.World.All)
                {
                    if (w.Name == worldName)
                    {
                        ctx = new WorldContext(this, $"[\"{worldName}\"]", w, ContextFieldInfo.MakeOperator($"[\"{worldName}\"]"));
                        return true;
                    }
                }
            }
            ctx = default;
            return false;
        }
        public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            foreach (var w in Entities.World.All)
            {
                var info = MakeMemberInfo<WorldContext>($"\"{w.Name}\"");
                if (!visitor(info)) return false;
            }
            return true;
        }
    }


}