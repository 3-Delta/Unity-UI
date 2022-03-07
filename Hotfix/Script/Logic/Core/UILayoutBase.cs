using System;
using UnityEngine;

namespace Logic.Hotfix
{
    [Serializable]
    public abstract class UILayoutBase
    {
        public Transform transform;

        public UILayoutBase TryBind(Transform target)
        {
            this.transform = target;

            this.FindByPath(target);

            var binder = target.GetComponent<UIBindComponents>();
            if (binder != null)
            {
                FindByIndex(binder);
            }

            return this;
        }

        protected virtual void FindByIndex(UIBindComponents binder) { }

        protected virtual void FindByPath(Transform transform) { }

        // public interface IListener { }
        // public void Listen(IListener listener) { }
    }

    // 用于实现空对象模式
    [Serializable]
    public class EmptyUILayout : UILayoutBase { }
}
