using System.Collections.Generic;
using UnityEngine;

public class TransList : MonoBehaviour {
    public string listTag;

    // 需要显示的objs
    public List<Transform> actives = new List<Transform>();

    // 需要隐藏的objs
    public List<Transform> deactives = new List<Transform>();

    [SerializeField] private TransListRegistry _collector;

    public TransListRegistry Collector {
        get {
            if (_collector == null) {
                _collector = GetComponentInParent<TransListRegistry>();
            }

            return _collector;
        }
    }

    protected void OnEnable() {
        if (Collector != null) {
            Collector.RegisterToggle(this);
        }
    }

    protected void OnDisbale() {
        if (Collector != null) {
            Collector.UnregisterToggle(this);
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
