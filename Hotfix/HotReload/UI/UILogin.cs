using Logic.Hotfix.Fixed;

using UnityEngine.UI;

namespace Logic.Hotfix.HotReload
{
    public class UILogin : UIBaseWithLayout<UILogin.Layout>
    {
        public class Layout : UILayoutBase
        {
            // [0] Path: "Text"
            public Text x { get; private set; } = null;

            protected override void FindByIndex(UIBindComponents binder)
            {
                x = binder.Find<Text>(0);
            }


            public interface IListener
            {
            }

            public void Listen(IListener listener, bool toListen = true)
            {
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            layout.x.GetComponent<Text>().text = "=====ccccccc======";
        }
    }
}
