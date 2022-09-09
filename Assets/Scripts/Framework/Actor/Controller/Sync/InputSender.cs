using System.Collections.Generic;
using UnityEngine;

// https://zhuanlan.zhihu.com/p/49483467
// 操作指令同步给server
[DisallowMultipleComponent]
public class InputSender : MonoBehaviour {
    [Range(3, 999)] public int SendFrequency = 10;
    public Queue<OpCmd> opQueue = new Queue<OpCmd>(0);
    
    private void FixedUpdate() {
        // 每10帧发送一次
        if (Time.frameCount % SendFrequency == 0) {
            // target
        }
    }
}
