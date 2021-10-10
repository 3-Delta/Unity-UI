using System;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public class SceneContext : ScopeContext
    {
        public UnityEngine.SceneManagement.Scene Scene;
        public SceneContext(IWatchContext parent, string relativePath, UnityEngine.SceneManagement.Scene scene, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(SceneContext)))
            )
        {
            Scene = scene;
            Variables.Add("GameObject", new GameObjectDictionary(this, ParserUtils.MakePathRelative("GameObject"), Scene, ContextFieldInfo.Make("GameObject")));
        }

        public class GameObjectDictionary : DictionaryContext
        {

            public UnityEngine.SceneManagement.Scene Scene;
            public GameObjectDictionary(IWatchContext parent, string relativePath, UnityEngine.SceneManagement.Scene scene, ContextFieldInfo fieldInfo)
                : base(parent
                      , relativePath
                      , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(GameObjectDictionary)))
                )
            {
                Scene = scene;
            }
            public override bool TryParseIndex(string path, RangeInt cursor, out IWatchContext ctx)
            {
                if (ParserUtils.TryParseScopeSequenceRange(path, ref cursor, ParserUtils.DelemiterQuote, out var nameRangeIn, out var nameRangeOut))
                {
                    string name = path.Substring(nameRangeIn);
                    foreach (var go in Scene.GetRootGameObjects())
                    {
                        if (go.name == name)
                        {
                            GameObject go2 = go;
                            ctx = new GameObjectContext(this, $"[\"{name}\"]", go, ContextFieldInfo.MakeOperator($"[\"{name}\"]"));
                            //ctx = new PropertyPathContext<GameObject, GameObject>(this, this, $"[\"{name}\"]", ref go2, null, ContextFieldInfo.MakeOperator($"[\"{name}\"]"));
                            //ctx = new PropertyBagContext<GameObject>(this, $"[\"{name}\"]", ref go2, ContextFieldInfo.MakeOperator($"[\"{name}\"]"));
                            return true;
                        }
                    }
                }
                ctx = default;
                return false;
            }
            public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
            {

                foreach (var go in Scene.GetRootGameObjects())
                {
                    var info = MakeMemberInfo<GameObjectContext>($"\"{go.name}\"");
                    if (!visitor(info)) return false;

                }
                return true;
            }
        }
    }


}