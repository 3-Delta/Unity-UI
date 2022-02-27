using System.Collections.Generic;
using UnityEngine;

public enum ESpeedStatus {
    Front_Accelerated,
    Front_Normal,
    Front_Slowed,

    Paused,

    Reversed_Accelerated,
    Reversed_Normal,
    Reversed_Slowed,
}

public class SpeedAdjuster : MonoBehaviour {
    public static ESpeedStatus GetTimeState(float timeScale) {
        if (timeScale > 0f) {
            if (timeScale < 1f) {
                return ESpeedStatus.Front_Slowed;
            }
            else if (timeScale == 1f) {
                return ESpeedStatus.Front_Normal;
            }
            else {
                return ESpeedStatus.Front_Accelerated;
            }
        }
        else if (timeScale == 0f) {
            return ESpeedStatus.Paused;
        }
        else {
            float t = Mathf.Abs(timeScale);
            if (timeScale < 1f) {
                return ESpeedStatus.Reversed_Slowed;
            }
            else if (timeScale == 1f) {
                return ESpeedStatus.Reversed_Normal;
            }
            else {
                return ESpeedStatus.Reversed_Accelerated;
            }
        }
    }

    // group: {key: SpeedAdjustObserver}
    public static Dictionary<string, List<SpeedAdjustObserver>> container { get; private set; } = new Dictionary<string, List<SpeedAdjustObserver>>();

    public static void Register(SpeedAdjustObserver observer) {
        if (observer != null) {
            if (!container.TryGetValue(observer.group, out List<SpeedAdjustObserver> ls)) {
                ls = new List<SpeedAdjustObserver>();
                ls.Add(observer);
            }
            else {
                if (!ls.Contains(observer)) {
                    ls.Add(observer);
                }
            }
        }
    }

    public static void UnRegister(SpeedAdjustObserver observer) {
        if (observer != null) {
            if (container.TryGetValue(observer.group, out List<SpeedAdjustObserver> ls)) {
                ls.Remove(observer);
            }
        }
    }

    public static void AdjustSpeed(string group, float timeScale) {
        if (group != null) {
            if (container.TryGetValue(group, out List<SpeedAdjustObserver> ls)) {
                foreach (var elem in ls) {
                    elem.Adjust(timeScale);
                }
            }
        }
    }
}
