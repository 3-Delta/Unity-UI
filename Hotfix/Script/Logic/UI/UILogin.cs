using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : UIBaseWithLayout<UILogin.Layout> {
    public class Layout : UILayoutBase {
        // [0] Path: "Text"
        public UnityEngine.UI.Text x { get; private set; } = null;

        protected override void FindByIndex(UIBindComponents binder) {
            this.x = binder.Find<UnityEngine.UI.Text>(0);
        }
        
        
        public interface IListener {
        }

        public void Listen(IListener listener, bool toListen = true) {
        }
    }

    protected override void OnOpened() {
        base.OnOpened();
        layout.x.GetComponent<Text>().text = "===========";
    }
}
