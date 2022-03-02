using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class FUIGuarder : MonoBehaviour {
    public bool enableGuard = false;

    public List<FUIBase> uiDict = new List<FUIBase>();
    public List<FUIStack> uiStack = new List<FUIStack>();
    public List<FUIEntry> uiRegistry = new List<FUIEntry>();

    private void Awake() {
        hideFlags = HideFlags.DontSaveInBuild;
    }
    
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
            
            uiRegistry.Clear();
            foreach (var kvp in FUIEntryRegistry.registry) {
                uiRegistry.Add(kvp.Value);
            }

            gameObject.name = string.Format("{0} {1} - {2}", FUIMgr.UI_PARENT_NAME, uiDict.Count.ToString(), uiStack.Count.ToString());
        }
    }
}
#endif
