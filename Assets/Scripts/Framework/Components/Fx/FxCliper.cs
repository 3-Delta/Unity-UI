using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// 裁剪UI中的特效
[DisallowMultipleComponent]
public class FxCliper : MonoBehaviour {
    public CompareFunction compare = CompareFunction.Equal;
    public int refValue = 2;

    [SerializeField] private ParticleSystemRenderer[] renders;
    [SerializeField] private List<Material> mats = new List<Material>();

    public const string StencilRefValueKey = "_Stencil";
    public const string StencilCompKey = "_StencilComp";

    private void Start() {
        renders = GetComponentsInChildren<ParticleSystemRenderer>();
        mats.Clear();

        foreach (var oneRender in renders) {
            var ls = oneRender.materials;

            foreach (var mat in ls) {
                mat.SetFloat(StencilRefValueKey, refValue);
                mat.SetFloat(StencilCompKey, (int) compare);

                mats.Add(mat);
            }
        }
    }

    private void OnDestroy() {
        // 需要destroy，因为mat是实例化出来的，否则会mat泄漏
        // Unity Profiler中观察
        if (Application.isEditor) {
            foreach (var mat in mats) {
                DestroyImmediate(mat);
            }
        }
        else {
            foreach (var mat in mats) {
                Destroy(mat);
            }
        }
    }
}