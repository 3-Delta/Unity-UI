using System;
using Unity.Entities;
using Unity.Properties;

namespace Unity.DebugWatch.WatchContext
{

    [Serializable]
    public class EntityPropertyWatch<TValue> : PropertyWatch<EntityContainer, TValue>, IWorldWatch
    {
        string worldName;
        public EntityPropertyWatch(IWatchContext context, EntityContainer container, PropertyPath propPath)
            :base(context, container, propPath)
        {
            worldName = container.EntityManager.World.Name;
        }
        //return if watch target is valid
        public bool UpdateWorldReference()
        {
            if (!Container.EntityManager.IsCreated)
            {

                foreach(var w in Entities.World.All)
                {
                    if(w.Name == worldName)
                    {
                        Container = new EntityContainer(w.EntityManager, Container.Entity);
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public override string GetContextName()
        {
            if (!UpdateWorldReference()) return "Invalid world";
            if (Context != null)
            {
                return Context.Scope("");
            }
            return $"World:{Container.EntityManager.World.Name}, Entity:{Container.Entity.Index}";
        }
        World IWorldWatch.GetWorld()
        {
            if (!UpdateWorldReference()) return null;
            return Container.EntityManager.World;
        }
        public override string GetName()
        {
            if (Context != null)
            {
                return Context.Scope("." + PropPath.ToString());
            }
            return $"Entity[{Container.Entity.Index}].{PropPath.ToString()}";
        }



        public override bool TryGet(out TValue value)
        {
            if (!UpdateWorldReference())
            {
                value = default;
                return false;
            }
            return base.TryGet(out value);
        }
        public override bool TrySet(TValue value)
        {
            if (!UpdateWorldReference()) return false;
            return base.TrySet(value);
        }
    }
}