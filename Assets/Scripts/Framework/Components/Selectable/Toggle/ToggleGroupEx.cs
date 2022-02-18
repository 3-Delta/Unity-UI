using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ToggleGroupEx : ToggleGroup {
    public Action<int, ToggleEx> onValueChanged;
        
    // id = -1表示all disable
    public void SwitchTo(int id) {
        
    }
}
