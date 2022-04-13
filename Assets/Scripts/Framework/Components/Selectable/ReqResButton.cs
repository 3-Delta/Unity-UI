using System;
using UnityEngine;
using UnityEngine.UI;

// 某些按钮请求协议之后，需要暂时灰掉，不能被点击，只有回包回来之后才允许再次点击
[DisallowMultipleComponent]
public class ReqResButton : MonoBehaviour {
    public Button button;
    public ushort res;

    private bool _hasReq = false;

    private void OnDestroy() {
        NWDelegateService.emiter.Handle<ushort>(res, OnReceRes, false);
    }

    [ContextMenu(nameof(Req))]
    public void Req() {
        NWDelegateService.emiter.Handle<ushort>(res, OnReceRes, false);
        NWDelegateService.emiter.Handle<ushort>(res, OnReceRes, true);

        button.interactable = false;
        _hasReq = true;
    }

    [ContextMenu(nameof(End))]
    public void End() {
        NWDelegateService.emiter.Handle<ushort>(res, OnReceRes, false);

        button.interactable = true;
        _hasReq = false;
    }

    public void OnReceRes(ushort res) {
        if (_hasReq && res == this.res) {
            End();
        }
    }
}
