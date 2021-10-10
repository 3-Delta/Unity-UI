
namespace Unity.DebugWatch.WatchContext
{

    public struct ContextFieldInfo
    {

        public string Name;
        public bool IsOperator;
        public static ContextFieldInfo Make(string name)
        {
            return new ContextFieldInfo()
            {
                Name = name,
                IsOperator = false
            };
        }
        public static ContextFieldInfo MakeOperator(string name)
        {
            return new ContextFieldInfo()
            {
                Name = name,
                IsOperator = true
            };
        }
    }

}