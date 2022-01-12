using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 子类继承，然后判断是否需要执行特定的逻辑
[DisallowMultipleComponent]
public abstract class LimitedMonoBehaviour : MonoBehaviour {
    public bool useAwake;
    public bool useDestroy;

    public bool useEnable;
    public bool useDisable;

    public bool useStart;

    public bool useUpdate;
    public bool useLateUpdate;
    public bool useFixedUpdate;

    public bool useApplicationQuit;
    public bool useApplicationPauseFalse;
    public bool useApplicationPauseTrue;

    public bool useGUI;
}
