using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TrList : MonoBehaviour {
    public string listTag;

    // 需要显示的objs
    public List<Transform> actives = new List<Transform>();

    // 需要隐藏的objs
    public List<Transform> deactives = new List<Transform>();

    [SerializeField] private TrListRegistry _collector;

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
        foreach (Transform t in actives) {
            t.gameObject.SetActive(active);
        }

        foreach (Transform t in deactives) {
            t.gameObject.SetActive(!active);
        }
    }
}
