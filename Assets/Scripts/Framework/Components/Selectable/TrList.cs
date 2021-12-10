using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class ActiverGroup {
    // 需要显示的objs
    public List<Transform> actives = new List<Transform>(0);
    // 需要隐藏的objs
    public List<Transform> deactives = new List<Transform>(0);
}

[DisallowMultipleComponent]
public class TrList : MonoBehaviour {
    public string ActiverTag;
    public ActiverGroup activer;
    
    [ReadOnly] [SerializeField] private TrListRegistry _collector;

    public TrListRegistry Collector {
        get {
            if (_collector == null) {
                _collector = GetComponentInParent<TrListRegistry>();
            }

            return _collector;
        }
    }

    protected void OnEnable() {
        if (Collector != null) {
            Collector.Register(this);
        }
    }

    protected void OnDisbale() {
        if (Collector != null) {
            Collector.Unregister(this);
        }
    }

    public void ShowHideBySetActive(bool active) {
        foreach (Transform t in activer.actives) {
            t.gameObject.SetActive(active);
        }

        foreach (Transform t in activer.deactives) {
            t.gameObject.SetActive(!active);
        }
    }
}
