using UnityEngine;

// 其实ReplaceComponent<T> 是T的warpper,代理
[DisallowMultipleComponent]
public class ReplaceComponent<T> : MonoBehaviour where T : MonoBehaviour {
    public static T finalT = null;
    public static T defaultT = null;

    private T cur;

    private void Awake() {
        if (defaultT == null) {
            // 首个
            defaultT = gameObject.AddComponent<T>();
        }

        if (finalT == null) {
            // 首个
            finalT = defaultT;
        }
        else {
            // false当前的final
            finalT.enabled = false;
            // 设置新的final
            finalT = gameObject.AddComponent<T>();
        }

        cur = GetComponent<T>();
    }

    private void OnDestroy() {
        if (cur != defaultT) {
            // 销毁final，final重新指向当前
            if (cur == finalT) {
                finalT = defaultT;
                finalT.enabled = true;
            }
        }
        else {
            // 其实 不支持动态修改default,也就是说default不能被销毁，只能长存
            // 这里为了健壮性，指向final

            // 销毁默认,则默认指向fnial
            defaultT = finalT;
        }
    }

    // public static void TryReplace(GameObject target) {
    //     if (defaultT == null) {
    //         // 首个
    //         defaultT = target.AddComponent<T>();
    //     }
    //
    //     if (finalT == null) {
    //         // 首个
    //         finalT = defaultT;
    //     }
    //     else {
    //         // 在这里设计，主要是PHASEListener不能同时存在两个激活的，所以只能第三方处理
    //         
    //         // false当前的final
    //         finalT.enabled = false;
    //         // 设置新的final
    //         finalT = target.AddComponent<T>();
    //     }
    // }
}