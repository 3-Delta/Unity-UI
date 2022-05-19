using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FxSetter : MonoBehaviour {
    [SerializeField] private ParticleSystem[] fxList;

    private void Start() {
        this.fxList = this.GetComponentsInChildren<ParticleSystem>();

        foreach (var fx in fxList) {
            // fx.main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            // fx.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
    }
}
