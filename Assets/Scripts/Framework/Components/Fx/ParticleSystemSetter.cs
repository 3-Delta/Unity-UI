using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ParticleSystemSetter : MonoBehaviour {
    [SerializeField] private ParticleSystem[] fxList;

    private void Start() {
        fxList = GetComponentsInChildren<ParticleSystem>();

        foreach (var fx in fxList) {
            // fx.main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            // fx.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
    }
}
