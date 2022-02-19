using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class FUIGuarder : MonoBehaviour {
    public bool enableGuard = true;

    public List<FUIBase> uiDict = new List<FUIBase>();
    public List<FUIStack> uiStack = new List<FUIStack>();

    private void Update() {
        if (enableGuard) {
            uiDict.Clear();
            foreach (var kvp in FUIMgr.uiDict) {
                uiDict.Add(kvp.Value);
            }

            uiStack.Clear();
            foreach (var kvp in FUIMgr.stacks) {
                uiStack.Add(kvp.Value);
            }
        }
    }
}
#endif
