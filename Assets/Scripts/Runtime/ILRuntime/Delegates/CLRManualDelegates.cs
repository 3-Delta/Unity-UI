using System;
using System.Collections.Generic;
using System.Reflection;

using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.Generated {
    public static class CLRManualDelegates {
        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            RegisterMethodDelegate(appDomain);
            // Func
            RegisterFuncDelegate(appDomain);
            // Converter
            RegisterDelegateConverter(appDomain);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {

        }

        private static void RegisterMethodDelegate(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            appDomain.DelegateManager.RegisterMethodDelegate<object>();
            appDomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            
            appDomain.DelegateManager.RegisterMethodDelegate<UInt32>();
            appDomain.DelegateManager.RegisterMethodDelegate<Int32>();
            appDomain.DelegateManager.RegisterMethodDelegate<String>();
            appDomain.DelegateManager.RegisterMethodDelegate<Boolean>();
            appDomain.DelegateManager.RegisterMethodDelegate<Single>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();
            appDomain.DelegateManager.RegisterMethodDelegate<int>();
            appDomain.DelegateManager.RegisterMethodDelegate<string>();

            appDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.KeyValuePair<System.Int32, object>, System.Int32>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.KeyValuePair<ValueType, object>, System.Int32>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.KeyValuePair<object, object>>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.Collections.Generic.KeyValuePair<object, object>, System.Int32>();
        }
        private static void RegisterFuncDelegate(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            appDomain.DelegateManager.RegisterFunctionDelegate<object>();
            appDomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance>();
            
            appDomain.DelegateManager.RegisterFunctionDelegate<int, string>();
            
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.Int32, object>, System.Int32, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<System.Collections.Generic.KeyValuePair<System.Int32, ILRuntime.Runtime.Intepreter.ILTypeInstance>, System.Int32, System.Boolean>();
        }
        private static void RegisterDelegateConverter(ILRuntime.Runtime.Enviorment.AppDomain appDomain) {
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) => {
                return new UnityEngine.Events.UnityAction(() => {
                    ((System.Action)act)();
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Reflection.ConstructorInfo>>((act) => {
                return new System.Predicate<System.Reflection.ConstructorInfo>((obj) => {
                    return ((Func<System.Reflection.ConstructorInfo, System.Boolean>)act)(obj);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<float>>((action) => {
                return new UnityEngine.Events.UnityAction<float>((a) => {
                    ((System.Action<float>)action)(a);
                });
            });
        }
    }
}
