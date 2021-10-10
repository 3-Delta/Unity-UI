
namespace Unity.DebugWatch.WatchContext
{
    public class GlobalContext : ScopeContext
    {
        static GlobalContext _Instance;
        public static GlobalContext Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new GlobalContext();
                }
                return _Instance;
            }
        }
        public GlobalContext()
            : base(null, "", new ContextMemberInfo(ContextFieldInfo.Make(""), ContextTypeInfo.Make(typeof(GlobalContext))))
        {
            Variables.Add("World", new WorldDictionary(this, "World", ContextFieldInfo.Make("World")));
            Variables.Add("Scene", new SceneDictionary(this, "Scene", ContextFieldInfo.Make("Scene")));

        }
    }


}