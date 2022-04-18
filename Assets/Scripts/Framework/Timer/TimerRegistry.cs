using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;

[DisallowMultipleComponent]
public class TimerRegistry : MonoBehaviour {
    public static TimerRegistry Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

    public int Count {
        get { return timers.Count; }
    }

    private static List<MTimer> timers = new List<MTimer>();

    // 这里为何使用这个额外的缓冲去记录新增的timer，原因是：在foreach中，每个timer执行的过程中，如果直接新增一个整个times，就会导致整个迭代器失效，
    // 所以一般都是先等整个timers全部foreach完毕，然后才去新增新的timer
    private static List<MTimer> timersToAdd = new List<MTimer>();

    public void Register(MTimer timer) {
        timersToAdd.Add(timer);
    }

    public void UnRegister(MTimer timer) {
        int index = timers.IndexOf(timer);
        int lastIndex = 0;
        if (index != -1) {
            if (timers.Count > 1) {
                lastIndex = timers.Count - 1;
                if (index != lastIndex) {
                    timers[index] = timers[lastIndex];
                }
            }
            else {
                timers.RemoveAt(lastIndex);
            }
        }

        index = timersToAdd.IndexOf(timer);
        if (index == -1) {
            lastIndex = timersToAdd.Count - 1;
            if (index != lastIndex) {
                timersToAdd[index] = timersToAdd[lastIndex];
            }

            timersToAdd.RemoveAt(lastIndex);
        }
    }

    public void CancelAll() {
        foreach (var item in timers) {
            item.Cancel();
        }

        timers.Clear();
        timersToAdd.Clear();
    }

    public void PauseAll() {
        timers.ForEach(timer => { timer.Pause(); });
    }

    public void ResumeAll() {
        timers.ForEach(timer => { timer.Resume(); });
    }

    public void Update() {
        UpdateAll();
    }

    private void UpdateAll() {
        if (timersToAdd.Count > 0) {
            timers.AddRange(timersToAdd);
            timersToAdd.Clear();
        }

        foreach (var item in timers) {
            item.Update();
        }

        // 之前误认为cancel的timer不能被remove掉，原来可以，这里判断如果是isDone的话就直接remove了
        timers.RemoveAll(t => t.isDone);
    }
}
