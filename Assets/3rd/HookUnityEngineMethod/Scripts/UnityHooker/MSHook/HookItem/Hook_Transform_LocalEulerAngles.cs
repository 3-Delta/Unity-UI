using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hook {
    public class Hook_Transform_LocalEulerAngles : HookBase {
        public const string MethodName = "localEulerAngles";
        
        protected override void DoHook() {
            Type type = typeof(Transform);

            var targetPro = type.GetProperty(MethodName, (BindingFlags)(0x0fffffff));

            var targetMethod = targetPro.SetMethod;

            type = this.GetType();

            var replaceMethod = type.GetMethod(nameof(this._Replace), (BindingFlags)(0x0fffffff));

            var proxyMethod = type.GetMethod(nameof(this._Proxy), (BindingFlags)(0x0fffffff));

            Debug.Assert(targetMethod != null);
            Debug.Assert(replaceMethod != null);
            Debug.Assert(proxyMethod != null);

            MethodHooker = new MethodHook(targetMethod, replaceMethod, proxyMethod);
            MethodHooker.Install();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _Replace(Vector3 vec) {
            var transform = (this as object) as Transform;

            var gameObject = transform.gameObject;

            if (HookedGameObject.Equals((gameObject)))
            {
                HookLog.LogFormat($"{gameObject.name}\", {MethodName}:{vec.ToString("f3")}");
            }

            _Proxy(vec);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _Proxy(Vector3 vec) {
            HookLog.LogFormat("Dummy");
        }
    }
}
