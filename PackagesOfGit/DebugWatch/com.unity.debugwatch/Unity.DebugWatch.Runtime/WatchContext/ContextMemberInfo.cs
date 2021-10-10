
namespace Unity.DebugWatch.WatchContext
{
    public struct ContextMemberInfo
    {
        public ContextFieldInfo FieldInfo;
        public ContextTypeInfo TypeInfo;
        public string AddTo(string root)
        {
            if (FieldInfo.IsOperator)
            {
                return root + FieldInfo.Name;
            }
            return root + "." + FieldInfo.Name;
        }
        public string AsLocal()
        {
            return FieldInfo.Name;
        }
        public ContextMemberInfo(ContextFieldInfo fieldInfo, ContextTypeInfo typeInfo)
        {
            FieldInfo = fieldInfo;
            TypeInfo = typeInfo;
        }
        public static ContextMemberInfo MakeNotImplemented()
        {
            return new ContextMemberInfo(ContextFieldInfo.Make("Not Implemented"), ContextTypeInfo.MakeUnknown());
        }
    }


}