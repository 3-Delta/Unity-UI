using System;
using UnityEngine;

public class MFramer : ICancelable {
    public int duration { get; private set; }
    public Func<bool> isLooped { get; set; }

    public bool isPaused {
        get { return this.frameElapsedBeforePause.HasValue; }
    }

    public bool isCancelled {
        get { return this.frameElapsedBeforeCancel.HasValue; }
    }

    public bool isCompleted { get; private set; }

    public bool isDone {
        get { return this.isCompleted || this.isCancelled || this.isOwnerDestroyed; }
    }

    public static MFramer Register(int duration, System.Action onComplete, Func<bool> isLooped, System.Action<int> onUpdate = null, MonoBehaviour autoDestroyOwner = null) {
        MFramer framer = new MFramer(duration, onComplete, onUpdate, isLooped, autoDestroyOwner);
        FramerRegistry.Instance.Register(framer);
        return framer;
    }

    public static void Cancel(MFramer framer) {
        framer?.Cancel();
    }

    public static void Pause(MFramer framer) {
        framer?.Pause();
    }

    public static void Resume(MFramer framer) {
        framer?.Resume();
    }

    public void Cancel() {
        if (this.isDone) {
            return;
        }

        this.frameElapsedBeforeCancel = this.GetFrameElapsed();
        this.frameElapsedBeforePause = null;
    }

    public void Pause() {
        if (this.isPaused || this.isDone) {
            return;
        }

        this.frameElapsedBeforePause = this.GetFrameElapsed();
    }

    public void Resume() {
        if (!this.isPaused || this.isDone) {
            return;
        }

        this.frameElapsedBeforePause = null;
    }

    public int GetFrameElapsed() {
        if (this.isCompleted || this.GetCurrentFrame() >= this.GetFireTime()) {
            return this.duration;
        }

        return this.frameElapsedBeforeCancel ?? this.frameElapsedBeforePause ?? this.GetCurrentFrame() - this.startFramer;
    }

    public float GetTimeRemaining() {
        return this.duration - this.GetFrameElapsed();
    }

    public float GetRatioComplete() {
        return this.GetFrameElapsed() / this.duration;
    }

    public float GetRatioRemaining() {
        return this.GetTimeRemaining() / this.duration;
    }

    private bool isOwnerDestroyed {
        get { return this.hasAutoDestroyOwner && this.autoDestroyOwner == null; }
    }

    private readonly System.Action onComplete;
    private readonly System.Action<int> onUpdate;
    private int startFramer;

    private int? frameElapsedBeforeCancel;
    private int? frameElapsedBeforePause;

    private readonly MonoBehaviour autoDestroyOwner;
    private readonly bool hasAutoDestroyOwner;

    private MFramer(int duration, System.Action onComplete, System.Action<int> onUpdate, Func<bool> isLooped, MonoBehaviour autoDestroyOwner) {
        this.duration = duration;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        this.isLooped = isLooped;

        this.autoDestroyOwner = autoDestroyOwner;
        this.hasAutoDestroyOwner = autoDestroyOwner != null;

        this.startFramer = this.GetCurrentFrame();
    }

    private int GetCurrentFrame() {
        return Time.frameCount;
    }

    private float GetFireTime() {
        return this.startFramer + this.duration;
    }

    public void Update() {
        if (this.isDone) {
            return;
        }

        if (this.isPaused) {
            ++this.startFramer;
            return;
        }

        this.onUpdate?.Invoke(this.GetFrameElapsed());

        if (this.GetCurrentFrame() >= this.GetFireTime()) {
            this.onComplete?.Invoke();
            if (this.isLooped != null && this.isLooped.Invoke()) {
                this.startFramer = this.GetCurrentFrame();
            }
            else {
                this.isCompleted = true;
            }
        }
    }
}
