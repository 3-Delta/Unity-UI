using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class LocalInputSimulater : OpSimulater {
    public LocalInputReceiver localInputReceiver;
    public InputInputSender inputSender;
    public bool isLocalPredicted = true;

    protected override void OnSimulate() {
        OpCmd opCmd = GetCmd();
        // 获取指令
        this.localInputReceiver.GatherInput(ref opCmd.input);

        // Server直接执行指令
        // 客户端如果需要预表现，也直接执行
        if (isLocalPredicted && opCmd.HasInput) {
            this.inputInvoker.Exec(ref opCmd, this.target);

            opCmd.opFlag |= EOpFlag.Executed;
            this.inputSender.opQueue.Enqueue(opCmd);
        }
    }
}
