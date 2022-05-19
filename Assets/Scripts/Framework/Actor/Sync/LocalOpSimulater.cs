using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LocalOpSimulater : OpSimulater {
    public LocalOpInputer localOpInputer;
    public InputOpSender inputSender;
    public bool isLocalPredicted = true;

    protected override void OnSimulate() {
        OpCmd opCmd = GetCmd();
        // 获取指令
        this.localOpInputer.GatherInput(ref opCmd.input);

        // Server直接执行指令
        // 客户端如果需要预表现，也直接执行
        if (isLocalPredicted && opCmd.HasInput) {
            this.opInvoker.Exec(ref opCmd, this.target);

            opCmd.opFlag |= EOpFlag.Executed;
            this.inputSender.opQueue.Enqueue(opCmd);
        }
    }
}
