using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusActiver : MonoBehaviour {
    [SerializeField] private int curStatusIndex = 0;
    public List<ActiverSingleGroup> groups = new List<ActiverSingleGroup>(0);

    public void Set(int statusIndex) {
        curStatusIndex = statusIndex;
        if (0 <= statusIndex && statusIndex < groups.Count) {
            for (int i = 0, length = groups.Count; i < length; ++i) {
                groups[i].SetActive(i == statusIndex);
            }
        }
    }
}
