using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class XRegistry<T> : MonoBehaviour where T : Component {
    [SerializeField] protected List<T> list = new List<T>();

    public virtual void Register(T cd) {
        if (!list.Contains(cd)) {
            list.Add(cd);
        }
    }

    public virtual void Unregister(T cd) {
        list.Remove(cd);
    }
}

[DisallowMultipleComponent]
public class CDSelectableRegistry : XRegistry<CDSelectable> {
    [Range(0.1f, 5f)] public float cdTime = 0.4f;
    
    public void OnAnyClicked() {
        foreach (var one in list) {
            one.DisableImmediately();
        }
    }
}
