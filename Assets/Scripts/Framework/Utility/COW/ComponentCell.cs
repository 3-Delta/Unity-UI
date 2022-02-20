using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[DisallowMultipleComponent]
public class ComponentCell<T> : MonoBehaviour where T : Component {
    public T component;
    
    // 在Init的时候赋值vd
    // 绑定的vd解析器,在热更层 as成确切的vd
    public Object arg; 
}
