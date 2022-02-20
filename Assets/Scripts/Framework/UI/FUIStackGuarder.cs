using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[DisallowMultipleComponent]
public class FUIStackGuarder : MonoBehaviour
{
    public bool enableGuard = true;
    public EUILayer uiLayer = EUILayer.NormalStack;
    
    public List<FUIBase> uiList = new List<FUIBase>();

    private void Start() {
        hideFlags = HideFlags.DontSaveInBuild;
    }
    
    private void Update() {
        if (enableGuard) {
            uiList.Clear();
            if (FUIMgr.stacks.TryGetValue(uiLayer, out FUIStack stack)) {
                foreach (var ui in stack.stack) {
                    uiList.Add(ui);
                }
                
                gameObject.name = string.Format("{0} {1}", FUIMgr.roots[uiLayer].name, uiList.Count.ToString());
            }
        }
    }
}
#endif
