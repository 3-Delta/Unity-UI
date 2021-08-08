using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestAsset : Request<RequestAsset> {
    public RequestBundle requestBundle;
}

public class EditorRequestAsset : RequestAsset {
    protected override void OnBeginLoad() {

    }
}

public class DeviceRequestAsset : RequestAsset {

}
