using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class NWConnecter : MonoBehaviour {
    public string ip = "127.0.0.3";
    public int port = 14456;

    [SerializeField] private NWTransfer transfer = new NWTransfer();

    [ContextMenu(nameof(Connect))]
    private void Connect() {
        Connect(ip, port);
    }

    [ContextMenu(nameof(DisConnect))]
    private void DisConnect() {
        transfer.DisConnect(true);
    }

    private void Connect(string ip, int port) {
        this.ip = ip;
        this.port = port;

        transfer.Connect(ip, port);
    }
}
