using System;
using UnityEngine;
namespace Unity.DebugWatch.WatchContext
{

    public class SceneDictionary : DictionaryContext
    {
        public SceneDictionary(IWatchContext parent, string relativePath, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(SceneDictionary)))
            )
        {
        }
        public override bool TryParseIndex(string path, RangeInt cursor, out IWatchContext ctx)
        {
            if (ParserUtils.TryParseScopeSequenceRange(path, ref cursor, ParserUtils.DelemiterQuote, out var nameRangeIn, out var nameRangeOut))
            {
                string name = path.Substring(nameRangeIn);
                for (int i = 0; i != UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
                {
                    var s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                    if (s.name == name)
                    {
                        ctx = new SceneContext(this, $"[\"{name}\"]", s, ContextFieldInfo.MakeOperator($"[\"{name}\"]"));
                        return true;
                    }
                }
            }
            ctx = default;
            return false;
        }
        public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            for (int i = 0; i != UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                var s = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                var info = MakeMemberInfo<SceneContext>($"\"{s.name}\"");
                if (!visitor(info)) return false;

            }
            return true;
        }
    }


}