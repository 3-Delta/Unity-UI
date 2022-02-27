using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    partial class CLRBindings
    {
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain appDomain, bool defaultFlag = true)
        {
            ManualBinding_UnityEngine_GameObject.Register(appDomain);
        }
    }
}
