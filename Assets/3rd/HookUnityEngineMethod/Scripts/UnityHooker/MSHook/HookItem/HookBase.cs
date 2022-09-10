using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hook {
    public class HookBase {
        protected MethodHook MethodHooker;
        protected static GameObject HookedGameObject;

        public bool TryHook(GameObject target) {
            HookedGameObject = target;
            MethodHooker?.Uninstall();

            if (target != null && MethodHooker == null) {
                DoHook();
                return true;
            }

            return false;
        }

        protected virtual void DoHook() { }

        public void UnHook() {
            MethodHooker?.Uninstall();
        }
    }
}
