using UnityEngine;

// 网络连接状态监测
// wifi断开，4g断开，wifi/4g/断网切换，弱网络环境
[DisallowMultipleComponent]
public class NetTracer : MonoBehaviour {
    // 最小网络传输速度， 小于则认为网络环境很差
    public float minNetworkSpeed;

    private void OnLowNetworkSpeed() {
    }
}