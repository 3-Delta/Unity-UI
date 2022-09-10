using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hook {
    public class Hook_Behaviour_Enable : HookBase {
        public const string MethodName = "enabled";
        
        protected override void DoHook() {
            Type type = typeof(Behaviour);
            
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
        private void _Replace(bool value) {
            var behaviour = (this as object) as Behaviour;

            var gameObject = behaviour.gameObject;

            if (HookedGameObject.Equals((gameObject)))
            {
                HookLog.LogFormat($" {MethodName} \"{gameObject}\", old:{behaviour.enabled} , new:{value}");
            }

            _Proxy(value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _Proxy(bool value) {
            HookLog.LogFormat("Dummy");
        }
    }
}
