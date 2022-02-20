using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIInjector : Injector<FUIBase> {
    private void Start() {
        hideFlags = HideFlags.DontSaveInBuild;
    }
}
