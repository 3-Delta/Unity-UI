using UnityEngine;

[DisallowMultipleComponent]
public class QualityLevelDispatcher : QualitySetterBase {
    public Transform lowQualityRoot;
    public Transform midQualityRoot;
    public Transform highQualityRoot;

    public EQualityLevel currentQuality = EQualityLevel.Medium;

    private void SetQualityRootActive(Transform trans, bool value) {
        trans?.gameObject.SetActive(value);
    }

    public override void SetQualityLevel(EQualityLevel qualityLevel) {
        this.currentQuality = currentQuality;

        switch (qualityLevel) {
            case EQualityLevel.Medium:
                SetQualityRootActive(lowQualityRoot, true);
                SetQualityRootActive(midQualityRoot, true);
                SetQualityRootActive(highQualityRoot, false);
                break;
            case EQualityLevel.High:
                SetQualityRootActive(lowQualityRoot, true);
                SetQualityRootActive(midQualityRoot, true);
                SetQualityRootActive(highQualityRoot, true);
                break;
            default:
                SetQualityRootActive(lowQualityRoot, true);
                SetQualityRootActive(midQualityRoot, false);
                SetQualityRootActive(highQualityRoot, false);
                break;
        }
    }
}
