using System;
using UnityEngine;

// 输入
[DisallowMultipleComponent]
public abstract class CtrlInput : MonoBehaviour {
    public virtual void GatherCtrlInput(ref ECtrlKey input) {
    }
    
    public virtual void GatherSkillInput(ref ESkillKey input) {
    }
    
    public virtual void GatherOtherInput(ref EOtherKey input) {
    }
}
