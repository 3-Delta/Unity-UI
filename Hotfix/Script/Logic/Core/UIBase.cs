using System;
using UnityEngine;

namespace Logic.Hotfix
{
    [Serializable]
    public class UIBaseWithLayout<TLayout> : FUIBase where TLayout : UILayoutBase, new()
    {
        [SerializeField] protected TLayout layout;

        protected override void OnLoaded(Transform transform)
        {
            base.OnLoaded(transform);

            this.layout = new TLayout();
            this.layout.TryBind(transform);
        }
    }
}