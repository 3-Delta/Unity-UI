using UnityEngine;

// 接受键鼠/遥感的输入
// 将来从某个统一的地方接收输入，便于统一管理输入的开启,关闭
// 监听InputMgr的各种action
[DisallowMultipleComponent]
public class LocalInputReceiver : MonoBehaviour {
    // 后续细分各种操作，比如遥感，键盘，鼠标
    public virtual void GatherInput(ref OpInput input) {
        input.move = EMoveKey.Nil;
        if (Input.GetKey(KeyCode.W)) {
            input.move |= EMoveKey.Forward;
        }

        if (Input.GetKey(KeyCode.S)) {
            input.move |= EMoveKey.Backward;
        }

        if (Input.GetKey(KeyCode.A)) {
            input.move |= EMoveKey.Left;
        }

        if (Input.GetKey(KeyCode.D)) {
            input.move |= EMoveKey.Right;
        }

        input.skill = ESkillKey.Nil;
    }
}
