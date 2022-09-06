using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 键鼠输入
[DisallowMultipleComponent]
public class KeyboardMoveInput : MoveInput {
    public override void GatherInput(ref OpInput input) {
        // 方向控制
        input.ctrl = ECtrlKey.Nil;
        if (Input.GetKey(KeyCode.W)) {
            input.ctrl |= ECtrlKey.Forward;
        }
        else if (Input.GetKey(KeyCode.S)) {
            input.ctrl |= ECtrlKey.Backward;
        }

        if (Input.GetKey(KeyCode.A)) {
            input.ctrl |= ECtrlKey.Right;
        }
        else if (Input.GetKey(KeyCode.D)) {
            input.ctrl |= ECtrlKey.Left;
        }

        // 技能输入
        input.skill = ESkillKey.Nil;
        if (Input.GetKey(KeyCode.J)) {
            input.skill = ESkillKey.NAttack1;
        }
        
        // other输入
        input.other = EOtherKey.Nil;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            input.other = EOtherKey.KeyCode_Esc;
        }
    }
}