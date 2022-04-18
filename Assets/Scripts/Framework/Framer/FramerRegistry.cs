using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class FramerRegistry : MonoBehaviour {
    public static FramerRegistry Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void OnDestroy() {
        Instance = null;
    }

    private static List<MFramer> framers = new List<MFramer>();
    private static List<MFramer> framersToAdd = new List<MFramer>();

    public void Register(MFramer timer) {
        framersToAdd.Add(timer);
    }

    public void CancelAll() {
        framers.ForEach(framer => { framer.Cancel(); });
        framers.Clear();
        framersToAdd.Clear();
    }

    public void PauseAll() {
        framers.ForEach(framer => { framer.Pause(); });
    }

    public void ResumeAll() {
        framers.ForEach(framer => { framer.Resume(); });
    }

    public void Update() {
        UpdateAll();
    }

    private void UpdateAll() {
        if (framersToAdd.Count > 0) {
            framers.AddRange(framersToAdd);
            framersToAdd.Clear();
        }

        framers.ForEach(framer => { framer.Update(); });
        framers.RemoveAll(t => t.isDone);
    }
}
