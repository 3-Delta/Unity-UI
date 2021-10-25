using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// 裁剪UI中的特效
[DisallowMultipleComponent]
public class RendererCliper<T> : MonoBehaviour where T : Renderer {
    public CompareFunction compare = CompareFunction.Equal;
    public int refValue = 2;

    [SerializeField] private T[] renders;
    [SerializeField] private List<Material> mats = new List<Material>();

    public static readonly int StencilRefValueKey = Shader.PropertyToID("_Stencil");
    public static readonly int StencilCompKey = Shader.PropertyToID("_StencilComp");

    private void Start() {
        Collect();
    }

    public void Collect() {
        Clean();

        renders = GetComponentsInChildren<T>();
        foreach (var oneRender in renders) {
            var ls = oneRender.materials;

            foreach (var mat in ls) {
                mat.SetFloat(StencilRefValueKey, refValue);
                mat.SetFloat(StencilCompKey, (int)compare);

                mats.Add(mat);
            }
        }
    }

    public void Clean() {
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

        mats.Clear();
    }

    private void OnDestroy() {
        Clean();
    }
}
