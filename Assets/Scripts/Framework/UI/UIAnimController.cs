using System.Collections.Generic;
using UnityEngine;

// 注意：// 有时候动画没有播放完毕的时候，disable，然后重新enable的时候，动画状态不会重置
[DisallowMultipleComponent]
public class UIAnimController : MonoBehaviour {
    public float maxOpenAnimTime;
    public float maxCloseAnimTime;

    public Animation[] anims = new Animation[0];

    public void PlayOpen() {
        foreach (var anim in anims) {
            if (anim != null && anim.isActiveAndEnabled) {
                anim.Play("Open");
            }
        }
    }

    public void PlayClose() {
        foreach (var anim in anims) {
            if (anim != null && anim.isActiveAndEnabled) {
                anim.Play("Close");
            }
        }
    }

    public void SetActive(bool toActive) {
        foreach (var anim in anims) {
            anim.enabled = toActive;
        }
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(Collect))]
    public void Collect() {
        // 计算最大动画时长
        anims = GetComponentsInChildren<Animation>(true);
    }
#endif
}
