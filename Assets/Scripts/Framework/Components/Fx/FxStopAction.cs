using System;
using UnityEngine;

// https://docs.unity3d.com/2019.4/Documentation/ScriptReference/ParticleSystemStopAction.Callback.html
[RequireComponent(typeof(ParticleSystem))]
[DisallowMultipleComponent]
public class FxStopAction : MonoBehaviour {
    public ParticleSystem ps;
    public ParticleSystemStopAction stopAction = ParticleSystemStopAction.Disable;
    // 作为FxStopAction的父系节点 
    // FxLoader动态加载特效之后给赋值，或者fx静态挂载到prefab中拖拽赋值
    public FxIndexer indexer;

    private void Start() { 
        if (this.ps == null) {
            this.ps = this.GetComponent<ParticleSystem>();
        }
 
        // ref: https://docs.unity3d.com/2019.4/Documentation/ScriptReference/MonoBehaviour.OnParticleSystemStopped.html
        var main = this.ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    // 特效没有播放完毕的时候，setactive(false)然后setactive(true)，特效会从0重新开始，而不是continue
    private void OnParticleSystemStopped() {
        if (this.indexer != null) {
            if (this.stopAction == ParticleSystemStopAction.Disable) {
                this.indexer.gameObject.SetActive(false);
            }
        }
    }
}
