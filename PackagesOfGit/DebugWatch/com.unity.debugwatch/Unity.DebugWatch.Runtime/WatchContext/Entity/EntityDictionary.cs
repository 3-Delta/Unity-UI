
using System;
using Unity.Entities;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    public class EntityDictionary : ScopeContext
    {
        public Entities.World World;
        public EntityDictionary(IWatchContext parent, string relativePath, Entities.World world, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(EntityDictionary)))
            )
        {
            World = world;
        }
        public override bool TryParse(string path, ref RangeInt cursor, out IWatchContext ctx)
        {
            var c = cursor;
            if (ParserUtils.TryParseScopeSequenceRange(path, ref c, ParserUtils.DelemiterSquareBracket, out var seqIn, out var seqOut))
            {
                if (ParserUtils.TryParseIntAt(path, ref seqIn, out var index))
                {

                    var es = World.EntityManager.GetAllEntities();
                    for (int i = 0; i != es.Length; ++i)
                    {
                        if (es[i].Index == index)
                        {
                            cursor = c;
                            var ctnr = new EntityContainer(World.EntityManager, es[i]);
                            ctx = new PropertyPathContext<EntityContainer, EntityContainer>(this, this, $"[{es[i].Index}]", ctnr, null, ContextFieldInfo.MakeOperator($"[{es[i].Index}]"));
                            return true;
                        }
                    }
                }
            }
            ctx = null;
            return false;
        }
        public override bool VisitAllMembers(Func<ContextMemberInfo, bool> visitor)
        {
            foreach (var e in World.EntityManager.GetAllEntities())
            {
                var info = new ContextMemberInfo(ContextFieldInfo.MakeOperator($"[{e.Index}]"), ContextTypeInfo.Make(typeof(PropertyPathContext<EntityContainer, EntityContainer>)));
                if (!visitor(info)) return false;
            }
            return true;
        }
        //public override string Scope(string path)
        //{
        //    string value = $"Entity" + path;
        //    if (Parent != null) return Parent.Scope("." + value);
        //    return value;
        //}
    }
    public class EntityContext : ScopeContext
    {

        Entity entity;
        public EntityContext(IWatchContext parent, string relativePath, Entity entity, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(WorldContext)))
            )
        {
            this.entity = entity;
            //Variables.Add("Components", new EntityDictionary(this, ".Entity", World, ContextFieldInfo.Make("Entity")));
        }
    }

}