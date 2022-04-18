using System;
using UnityEngine;

[DisallowMultipleComponent]
public class QualityLevelCtrl : QualitySetterBase {
    [Serializable]
    public class QualityItem {
        public GameObject[] gameobjects = new GameObject[0];
        public EQualityLevel quality = EQualityLevel.Medium;
    }

    public EQualityLevel currentQuality = EQualityLevel.Medium;
    public QualityItem[] qualitySetters = new QualityItem[0];

    public override void SetQualityLevel(EQualityLevel currentQuality) {
        this.currentQuality = currentQuality;

        for (int i = 0, count = qualitySetters.Length; i < count; ++i) {
            if (currentQuality >= qualitySetters[i].quality) {
                for (int j = 0, innerCount = qualitySetters[i].gameobjects.Length; j < innerCount; ++j) {
                    if (qualitySetters[i].gameobjects[j] != null) {
                        qualitySetters[i].gameobjects[j].SetActive(true);
                    }
                }
            }
            else {
                for (int j = 0, innerCount = qualitySetters[i].gameobjects.Length; j < innerCount; ++j) {
                    if (qualitySetters[i].gameobjects[j] != null) {
                        qualitySetters[i].gameobjects[j].SetActive(false);
                    }
                }
            }
        }
    }
}
