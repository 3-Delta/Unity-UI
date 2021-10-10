using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public class GameObjectContext : ScopeContext
    {

        public GameObject GO;
        GameObjectComponentDictionary components;

        public GameObjectContext(IWatchContext parent, string relativePath, GameObject go, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(GameObjectContext)))
            )
        {
            GO = go;
            this.Variables.Add("Components", new GameObjectComponentDictionary(this, ".Components", go, ContextFieldInfo.Make("Components")));
        }
    }

    public class GameObjectComponentDictionary : DictionaryContext
    {

        public GameObject GO;


        public GameObjectComponentDictionary(IWatchContext parent, string relativePath, GameObject go, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(GameObjectContext)))
            )
        {
            GO = go;
        }
        public override bool TryParseIndex(string path, RangeInt cursor, out IWatchContext ctx)
        {

            if (ParserUtils.TryParseIntAt(path, ref cursor, out var index))
            {

                var comps = GO.GetComponents<Component>();
                Component comp = comps[index];

                //Type PropertyBagContextType = typeof(PropertyBagContext<>);
                //Type[] typeParams = new Type[] { comp.GetType() };
                //Type PropertyBagContextTypeGen = PropertyBagContextType.MakeGenericType(typeParams);
                //
                //ctx = (IWatchContext)Activator.CreateInstance(PropertyBagContextTypeGen, this, $"[{index}]", comp, ContextFieldInfo.MakeOperator($"[{index}]"));


                Type PropertyPathContextType = typeof(PropertyPathContext<,>);
                Type[] typeParams = new Type[] { comp.GetType(), comp.GetType() };
                Type PropertyBagContextTypeGen = PropertyPathContextType.MakeGenericType(typeParams);

                //public PropertyPathContext(IWatchContext parent, IWatchContext rootPathContext, string relativePath, ref TContainer container, PropertyPath propPath, ContextFieldInfo fieldInfo)
                ctx = (IWatchContext)Activator.CreateInstance(PropertyBagContextTypeGen, this, this, $"[{index}]", comp, null, ContextFieldInfo.MakeOperator($"[{index}]"));

                if (ctx != null)
                return ctx != null;

            }
            ctx = default;
            return false;
        }
        public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            var comps = GO.GetComponents<Component>();
            for(int i = 0; i != comps.Length; ++i)
            {
                var info = new ContextMemberInfo(
                    ContextFieldInfo.MakeOperator($"[{i}]"),
                    ContextTypeInfo.Make(comps[i].GetType())
                    );
                if (!visitor(info)) return false;
            }
            return true;
        }
    }

}