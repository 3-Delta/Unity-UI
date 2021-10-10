
namespace Unity.DebugWatch.WatchContext
{
    public class WorldContext : ScopeContext
    {
        public Entities.World World;
        public WorldContext(IWatchContext parent, string relativePath, Entities.World world, ContextFieldInfo fieldInfo)
            : base(parent
                  , relativePath
                  , new ContextMemberInfo(fieldInfo, ContextTypeInfo.Make(typeof(WorldContext)))
            )
        {
            World = world;
            Variables.Add("Entity", new EntityDictionary(this, ".Entity", World, ContextFieldInfo.Make("Entity")));
        }
        //public override string Scope(string path)
        //{
        //    string value = $"[{World.Name}]" + path;
        //    if (Parent != null) return Parent.Scope(value);
        //    return value;
        //}
    }


}