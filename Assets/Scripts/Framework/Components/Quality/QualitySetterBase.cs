using System;
using System.Collections.Generic;
using UnityEngine;

public enum EQualityLevel {
    Low,
    Medium,
    High,
}

[DisallowMultipleComponent]
public abstract class QualitySetterBase : MonoBehaviour {
    public abstract void SetQualityLevel(EQualityLevel qualityLevel);

    public virtual void Start() {
        // SetQualityLevel(QualityMgr.CurrentQualityLevel);
    }
}
