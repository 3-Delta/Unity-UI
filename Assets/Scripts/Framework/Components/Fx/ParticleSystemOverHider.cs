using System;
using UnityEngine;

// 给非循环类型的fx挂载
[DisallowMultipleComponent]
public class AutoHider : MonoBehaviour {
    public enum EAutoOp {
        Hide,
        Destroy,
    }

    public EAutoOp op = EAutoOp.Hide;
    public bool useFxDuration = true;
    [Range(0.02f, 10f)] public float duration = 0.02f;

    private float maxDuration;

    public const string FUNCNAME = "Auto";

    private void Start() {
        if (useFxDuration) {
            var ls = GetComponentsInChildren<ParticleSystem>();
            maxDuration = 0f;
            foreach (var one in ls) {
                if (one.main.duration > maxDuration) {
                    maxDuration = one.main.duration;
                }
            }

            if (maxDuration <= 0.02f) {
                maxDuration = 0.02f;
            }
        }
    }

    private void OnEnable() {
        CancelInvoke(FUNCNAME);
        Invoke(FUNCNAME, useFxDuration ? maxDuration : duration);
    }

    private void Auto() {
        if (op == EAutoOp.Hide) {
            gameObject.SetActive(false);
        }
        else if (op == EAutoOp.Destroy) {
            Destroy(gameObject);
        }
    }
}
