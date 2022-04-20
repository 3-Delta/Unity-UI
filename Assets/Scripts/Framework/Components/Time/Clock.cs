using System;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public class Clock : MonoBehaviour {
    public const float HoursToDegrees = -30f;
    public const float MinutesToDegrees = -6f;
    public const float SecondsToDegrees = -6f;

    public long seconds;
    public bool auto = false;
    public Transform hour;
    public Transform min;
    public Transform sec;

    private TimeSpan span;
    private bool first = false;
    private DateTime beginTime;

    public Action onEnd;

    private void Start() {
        enabled = false;
        this.first = false;

        if (auto) {
            Begin();
        }
    }

    [ContextMenu(nameof(Begin))]
    private void Begin() {
        Begin(seconds);
    }

    [ContextMenu(nameof(End))]
    private void End() {
        enabled = false;
        first = false;
    }

    public void Begin(long seconds) {
        this.seconds = seconds;
        span = new TimeSpan(seconds * 10000000L);
        enabled = true;
        first = true;
    }

    private void Update() {
        TimeSpan time = TimeSpan.Zero;
        if (!first) {
            var diff = DateTime.Now - beginTime;
            time += diff;
        }
        else {
            beginTime = DateTime.Now;
            first = false;
        }

        hour.localRotation = Quaternion.Euler(0f, 0f, HoursToDegrees * (float)time.TotalHours);
        min.localRotation = Quaternion.Euler(0f, 0f, MinutesToDegrees * (float)time.TotalMinutes);
        sec.localRotation = Quaternion.Euler(0f, 0f, SecondsToDegrees * (float)time.TotalSeconds);

        if (time >= this.span) {
            this.End();
            onEnd?.Invoke();
        }
    }
}
