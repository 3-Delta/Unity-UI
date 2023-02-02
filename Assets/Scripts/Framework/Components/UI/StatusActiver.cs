using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusActiver : MonoBehaviour {
    [SerializeField] private int curStatusIndex = 0;
    public List<ActiverSingleGroup> groups = new List<ActiverSingleGroup>(0);

    public Action<int, int, bool> onStatusChanged;

    [ContextMenu(nameof(Set))]
    public void Set() {
        Set(curStatusIndex);
    }

    public void Set(int statusIndex, bool toTrigger = true) {
        int old = curStatusIndex;
        curStatusIndex = statusIndex;

        int length = groups.Count;
        if (0 <= statusIndex && statusIndex < length) {
            for (int i = 0; i < length; ++i) {
                groups[i].SetActive(i == statusIndex);
            }
        }

        if (toTrigger) {
            onStatusChanged?.Invoke(old, curStatusIndex, old != curStatusIndex);
        }
    }
}
