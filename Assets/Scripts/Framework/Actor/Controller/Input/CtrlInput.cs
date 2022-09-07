using System;
using UnityEngine;

// 输入
public abstract class CtrlInput : MonoBehaviour {
    public void GatherAll(ref OpInput input) {
        GatherCtrlInput(ref input.ctrl);
        GatherSkillInput(ref input.skill);
        GatherOtherInput(ref input.other);
    }

    public virtual void GatherCtrlInput(ref ECtrlKey input) {
    }

    public virtual void GatherSkillInput(ref ESkillKey input) {
    }

    public virtual void GatherOtherInput(ref EOtherKey input) {
    }
}
