using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
// 模仿UGUI的registry设计
public class TransListRegistry : XRegistry<TransList> {
    // UI表现
    // 如果传递null, 那么相当于全部!toSwicth,可以妙用
    public void SwitchTo(TransList tt, bool toSwicth) {
        for (int i = 0, length = list.Count; i < length; i++) {
            list[i].ShowHideBySetActive(toSwicth);
        }

        if (tt != null) {
            tt.ShowHideBySetActive(!toSwicth);
        }
    }
}
