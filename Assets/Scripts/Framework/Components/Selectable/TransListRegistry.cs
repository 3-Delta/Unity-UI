using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
// 模仿UGUI的registry设计
public class TransListRegistry : MonoBehaviour {
    private List<TransList> ls = new List<TransList>();

    public void RegisterToggle(TransList cpt) {
        if (!ls.Contains(cpt)) {
            ls.Add(cpt);
        }
    }

    public void UnregisterToggle(TransList cpt) {
        ls.Remove(cpt);
    }

    // UI表现
    // 如果传递null, 那么相当于全部!toSwicth,可以妙用
    public void SwitchTo(TransList tt, bool toSwicth) {
        for (int i = 0, length = ls.Count; i < length; i++) {
            ls[i].ShowHideBySetActive(toSwicth);
        }

        if (tt != null) {
            tt.ShowHideBySetActive(!toSwicth);
        }
    }
}
