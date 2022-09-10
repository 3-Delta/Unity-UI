using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hook {
    public class Hook_GameObject_SetActive : HookBase {
        public const string MethodName = "SetActive";

        protected override void DoHook() {
            Type type = typeof(GameObject);

            MethodInfo targetMethod = type.GetMethod(MethodName, (BindingFlags)(0x0fffffff));

            type = this.GetType();

            MethodInfo replaceMethod = type.GetMethod(nameof(_Replace), (BindingFlags)(0x0fffffff));

            MethodInfo proxyMethod = type.GetMethod(nameof(_Proxy), (BindingFlags)(0x0fffffff));

            Debug.Assert(targetMethod != null);
            Debug.Assert(replaceMethod != null);
            Debug.Assert(proxyMethod != null);

            MethodHooker = new MethodHook(targetMethod, replaceMethod, proxyMethod);
            MethodHooker.Install();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _Replace(bool value) {
            GameObject gameObject = (this as object) as GameObject;
            if (HookedGameObject.Equals(gameObject)) {
                HookLog.LogFormat($" {MethodName} \"{gameObject}\", old:{gameObject.activeSelf} , new:{value}");
            }

            _Proxy(value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _Proxy(bool value) {
            HookLog.LogFormat("Dummy");
        }
    }
}
