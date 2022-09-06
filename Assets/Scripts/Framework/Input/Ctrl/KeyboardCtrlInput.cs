using UnityEngine;

// 键鼠输入
[DisallowMultipleComponent]
public class KeyboardCtrlInput : CtrlInput {
    public override void GatherCtrlInput(ref ECtrlKey input) {
        if (Input.GetKey(KeyCode.W)) {
            input |= ECtrlKey.Forward;
        }
        else if (Input.GetKey(KeyCode.S)) {
            input |= ECtrlKey.Backward;
        }

        if (Input.GetKey(KeyCode.A)) {
            input |= ECtrlKey.Right;
        }
        else if (Input.GetKey(KeyCode.D)) {
            input |= ECtrlKey.Left;
        }
    }

    public override void GatherSkillInput(ref ESkillKey input) {
        if (Input.GetKey(KeyCode.J)) {
            input = ESkillKey.NAttack1;
        }
    }

    public override void GatherOtherInput(ref EOtherKey input) {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            input = EOtherKey.KeyCode_Esc;
        }
    }
}
