using System;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Serialization;

// 网络连接状态监测
// wifi断开，4g断开，wifi/4g/断网切换，弱网络环境
[DisallowMultipleComponent]
public class NetTracer : MonoBehaviour {
    public enum ENetSpeed {
        Low,
        Mid,
        High,
    }

    [SerializeField] private ENetSpeed speedType = ENetSpeed.Mid;
    [Range(0.3f, 99f)] public float CycleTime = 2f;

    [Range(0.1f, 99f)] public long minSpeed = 10;
    [Range(0.1f, 99f)] public long maxSpeed = 20;

    public Action<ENetSpeed, ENetSpeed, double> onSpeedChanged;

#if UNITY_EDITOR
    // 单位：kb/s
    [SerializeField] private double currentSpeed;
    [SerializeField] private bool isTimeOut;
#else
    public double current { get; private set; }
    public bool isTimeOut { get; private set; }
#endif

    // 内外网url应该不一样
    [SerializeField] private string _url = "https://www.baidu.com";
    [SerializeField] private string _ip = "127.0.0.1";
    [SerializeField] private int _port;

    private HttpClient _net;

    private void OnDestroy() {
        _net?.Dispose();
        _net = null;
    }

    [ContextMenu(nameof(Begin))]
    public void Begin() {
        Begin(_url, _ip, _port);
    }

    public void Begin(string url, string ip, int port) {
        _url = url;
        _ip = ip;
        _port = port;
        _net = new HttpClient();
        _net.Timeout = TimeSpan.FromSeconds(CycleTime);

        CancelInvoke(nameof(_Send));
        _Send();
    }

    [ContextMenu(nameof(End))]
    public void End() {
        CancelInvoke(nameof(_Send));
        _net.Dispose();
        _net = null;
    }

    private async void _Send() {
        isTimeOut = false;
        try {
            DateTime begin = DateTime.Now;
            byte[] data = await _net.GetByteArrayAsync(_url);
            DateTime end = DateTime.Now;
            double totalSec = (end - begin).TotalSeconds;

            isTimeOut = totalSec >= CycleTime;

            currentSpeed = Math.Round((double)data.Length / 1024 / totalSec, 2);
        }
        catch (Exception _) {
            currentSpeed = 0;
        }

        var old = speedType;
        if (currentSpeed < minSpeed) {
            speedType = ENetSpeed.Low;
        }
        else if (currentSpeed < maxSpeed) {
            speedType = ENetSpeed.Mid;
        }
        else {
            speedType = ENetSpeed.High;
        }

        onSpeedChanged?.Invoke(old, speedType, currentSpeed);

        Invoke(nameof(_Send), CycleTime);
    }
}
