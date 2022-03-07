using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Logic.Hotfix
{
    public class UIMain : UIBaseWithLayout<UIMain.Layout>, UIMain.Layout.IListener, IReload
    {
        public class Layout : UILayoutBase
        {
            // [0] Path: "Image"
            public UnityEngine.UI.Button btnOpenUI { get; private set; } = null;

            protected override void FindByIndex(UIBindComponents binder)
            {
                this.btnOpenUI = binder.Find<UnityEngine.UI.Button>(0);
            }

            // 后续想不热更prefab,只热更脚本的形式获取组件,再次函数内部添加查找逻辑即可
            protected override void FindByPath(Transform transform)
            {
                // this.btnOpenUI = transform.Find("Image").GetComponent<UnityEngine.UI.Button>();
            }

            public interface IListener
            {
                void OnBtnClickedbtnOpenUI();
            }

            public void Listen(IListener listener, bool toListen = true)
            {
                this.btnOpenUI.onClick.AddListener(listener.OnBtnClickedbtnOpenUI);
            }
        }

        protected override void OnLoaded(Transform transform)
        {
            base.OnLoaded(transform);

            layout.Listen(this);
        }

        public void OnBtnClickedbtnOpenUI()
        {
            UIMgr.Open(EUIType.UILogin);
        }

        public void OnReload()
        {
            // 表格重载，进行动态表格更新
        }
    }
}
