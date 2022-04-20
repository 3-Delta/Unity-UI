using System;
using UnityEngine;

// https://catlikecoding.com/unity/tutorials/basics/game-objects-and-scripts/
[DisallowMultipleComponent]
public class NowClock : MonoBehaviour {
    public const float HoursToDegrees = -30f;
    public const float MinutesToDegrees = -6f;
    public const float SecondsToDegrees = -6f;

    public Transform hour;
    public Transform min;
    public Transform sec;

    private void Update() {
        TimeSpan time = DateTime.Now.TimeOfDay;
        hour.localRotation =
            Quaternion.Euler(0f, 0f, HoursToDegrees * (float)time.TotalHours);
        min.localRotation =
            Quaternion.Euler(0f, 0f, MinutesToDegrees * (float)time.TotalMinutes);
        sec.localRotation =
            Quaternion.Euler(0f, 0f, SecondsToDegrees * (float)time.TotalSeconds);
    }
}
