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
    
    // UI打开之后设置定时器自动关闭
    [Serializable]
    public class UIBaseDelayDestroy<TLayout> : UIBaseWithLayout<TLayout> where TLayout : UILayoutBase, new()
    {
        // 
    }
}
