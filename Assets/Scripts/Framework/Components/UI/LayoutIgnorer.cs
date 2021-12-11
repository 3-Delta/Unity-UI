using UnityEngine;
using UnityEngine.UI;

// 在查看GridLayoutGroup的源码时，发现添加ILayoutIgnorer Interface 即可从GetChildren中忽略
public class LayoutIgnorer : MonoBehaviour, ILayoutIgnorer {
    public bool ignoreLayout {
        get {
            return true;
        }
    }
}
