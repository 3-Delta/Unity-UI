using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class MTimer : ICancelable {
    public static readonly Func<bool> True = delegate { return true; };
    public static readonly Func<bool> False = delegate { return false; };

    public float duration { get; private set; }
    public Func<bool> isLooped { get; set; }
    public bool usesRealTime { get; private set; }

    public bool isPaused {
        get { return this.timeElapsedBeforePause.HasValue; }
    }

    public bool isCancelled {
        get { return this.timeElapsedBeforeCancel.HasValue; }
    }

    /// <summary>
    /// Whether or not the timer completed running. This is false if the timer was cancelled.
    /// </summary>
    public bool isCompleted { get; private set; }

    /// <summary>
    /// Get whether or not the timer has finished running for any reason.
    /// </summary>
    public bool isDone {
        get { return this.isCompleted || this.isCancelled || this.isOwnerDestroyed; }
    }

    #region Public Static Methods
    /// <summary>
    /// Register a new timer that should fire an event after a certain amount of time
    /// has elapsed.
    ///
    /// Registered timers are destroyed when the scene changes.
    /// </summary>
    /// <param name="duration">The time to wait before the timer should fire, in seconds.</param>
    /// <param name="onComplete">An action to fire when the timer completes.</param>
    /// <param name="onUpdate">An action that should fire each time the timer is updated. Takes the amount
    /// of time passed in seconds since the start of the timer's current loop.</param>
    /// <param name="isLooped">Whether the timer should repeat after executing.</param>
    /// <param name="useRealTime">Whether the timer uses real-time(i.e. not affected by pauses,
    /// slow/fast motion) or game-time(will be affected by pauses and slow/fast-motion).</param>
    /// <param name="autoDestroyOwner">An object to attach this timer to. After the object is destroyed,
    /// the timer will expire and not execute. This allows you to avoid annoying <see cref="NullReferenceException"/>s
    /// by preventing the timer from running and accessessing its parents' components
    /// after the parent has been destroyed.</param>
    /// <returns>A timer object that allows you to examine stats and stop/resume progress.</returns>
    public static MTimer Register(float duration, System.Action onComplete, Func<bool> isLooped = null, System.Action<float> onUpdate = null, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null) {
        MTimer timer = new MTimer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
        TimerRegistry.Instance.Register(timer);
        return timer;
    }

    public static MTimer Reuse(ref MTimer timer, float duration, System.Action onComplete, Func<bool> isLooped = null, System.Action<float> onUpdate = null, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null) {
        timer?.Cancel();
        TimerRegistry.Instance.UnRegister(timer);
        timer?.Reuse(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
        TimerRegistry.Instance.Register(timer);
        return timer;
    }

    public static MTimer CreateOrReuse(ref MTimer timer, float duration, System.Action onComplete, Func<bool> isLooped = null, System.Action<float> onUpdate = null, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null) {
        if (timer == null) {
            timer = Register(duration, onComplete, isLooped, onUpdate, useRealTime, autoDestroyOwner);
        }
        else {
            timer = Reuse(ref timer, duration, onComplete, isLooped, onUpdate, useRealTime, autoDestroyOwner);
        }

        return timer;
    }

    public static void Cancel(MTimer timer) {
        timer?.Cancel();
    }

    public static void Pause(MTimer timer) {
        timer?.Pause();
    }

    public static void Resume(MTimer timer) {
        timer?.Resume();
    }

    /// <summary>
    /// Stop a timer that is in-progress or paused. The timer's on completion callback will not be called.
    /// </summary>
    public void Cancel() {
        if (this.isDone) {
            return;
        }

        this.timeElapsedBeforeCancel = this.GetTimeElapsed();
        this.timeElapsedBeforePause = null;
    }

    /// <summary>
    /// Pause a running timer. A paused timer can be resumed from the same point it was paused.
    /// </summary>
    public void Pause() {
        if (this.isPaused || this.isDone) {
            return;
        }

        this.timeElapsedBeforePause = this.GetTimeElapsed();
    }

    /// <summary>
    /// Continue a paused timer. Does nothing if the timer has not been paused.
    /// </summary>
    public void Resume() {
        if (!this.isPaused || this.isDone) {
            return;
        }

        this.timeElapsedBeforePause = null;
    }

    /// <summary>
    /// Get how many seconds have elapsed since the start of this timer's current cycle.
    /// </summary>
    /// <returns>The number of seconds that have elapsed since the start of this timer's current cycle, i.e.
    /// the current loop if the timer is looped, or the start if it isn't.
    ///
    /// If the timer has finished running, this is equal to the duration.
    ///
    /// If the timer was cancelled/paused, this is equal to the number of seconds that passed between the timer
    /// starting and when it was cancelled/paused.</returns>
    public float GetTimeElapsed() {
        if (this.isCompleted || this.GetCurrentTime() >= this.GetFireTime()) {
            return this.duration;
        }

        return this.timeElapsedBeforeCancel ?? this.timeElapsedBeforePause ?? this.GetCurrentTime() - this.startTime;
    }

    /// <summary>
    /// Get how many seconds remain before the timer completes.
    /// </summary>
    /// <returns>The number of seconds that remain to be elapsed until the timer is completed. A timer
    /// is only elapsing time if it is not paused, cancelled, or completed. This will be equal to zero
    /// if the timer completed.</returns>
    public float GetTimeRemaining() {
        return this.duration - this.GetTimeElapsed();
    }

    /// <summary>
    /// Get how much progress the timer has made from start to finish as a ratio.
    /// </summary>
    /// <returns>A value from 0 to 1 indicating how much of the timer's duration has been elapsed.</returns>
    public float GetRatioComplete() {
        return this.GetTimeElapsed() / this.duration;
    }

    /// <summary>
    /// Get how much progress the timer has left to make as a ratio.
    /// </summary>
    /// <returns>A value from 0 to 1 indicating how much of the timer's duration remains to be elapsed.</returns>
    public float GetRatioRemaining() {
        return this.GetTimeRemaining() / this.duration;
    }
    #endregion

    #region Private Properties/Fields
    public bool isOwnerDestroyed {
        get { return this.hasAutoDestroyOwner && this.autoDestroyOwner == null; }
    }

    public Action onComplete;
    public Action<float> onUpdate;
    private float startTime;
    private float lastUpdateTime;

    // for pausing, we push the start time forward by the amount of time that has passed.
    // this will mess with the amount of time that elapsed when we're cancelled or paused if we just
    // check the start time versus the current world time, so we need to cache the time that was elapsed
    // before we paused/cancelled
    private float? timeElapsedBeforeCancel;
    private float? timeElapsedBeforePause;

    // after the auto destroy owner is destroyed, the timer will expire
    // this way you don't run into any annoying bugs with timers running and accessing objects
    // after they have been destroyed
    public MonoBehaviour autoDestroyOwner { get; private set; }
    private bool hasAutoDestroyOwner;
    #endregion

    private MTimer(float duration, System.Action onComplete, System.Action<float> onUpdate = null, Func<bool> isLooped = null, bool usesRealTime = false, MonoBehaviour autoDestroyOwner = null) {
        this.isCompleted = false;
        this.duration = duration;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        this.isLooped = isLooped;
        this.usesRealTime = usesRealTime;

        this.autoDestroyOwner = autoDestroyOwner;
        this.hasAutoDestroyOwner = autoDestroyOwner != null;

        this.timeElapsedBeforePause = null;
        this.timeElapsedBeforeCancel = null;

        this.startTime = this.GetCurrentTime();
        this.lastUpdateTime = this.startTime;
    }

    public MTimer Reuse(float duration, System.Action onComplete, System.Action<float> onUpdate = null, Func<bool> isLooped = null, bool usesRealTime = false, MonoBehaviour autoDestroyOwner = null) {
        this.isCompleted = false;
        this.duration = duration;
        this.onComplete = onComplete;
        this.onUpdate = onUpdate;

        this.isLooped = isLooped;
        this.usesRealTime = usesRealTime;

        this.autoDestroyOwner = autoDestroyOwner;
        this.hasAutoDestroyOwner = autoDestroyOwner != null;

        this.timeElapsedBeforePause = null;
        this.timeElapsedBeforeCancel = null;

        this.startTime = this.GetCurrentTime();
        this.lastUpdateTime = this.startTime;

        return this;
    }

    public void ChangeDuration(long timeDiff) {
        this.duration += timeDiff;
    }

    private float GetCurrentTime() {
        return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetFireTime() {
        return this.startTime + this.duration;
    }

    private float GetTimeDelta() {
        return this.GetCurrentTime() - this.lastUpdateTime;
    }

    public void Update() {
        if (this.isDone) {
            return;
        }

        if (this.isPaused) {
            this.startTime += this.GetTimeDelta();
            this.lastUpdateTime = this.GetCurrentTime();
            return;
        }

        this.lastUpdateTime = this.GetCurrentTime();
        this.onUpdate?.Invoke(this.GetTimeElapsed());

        if (this.GetCurrentTime() >= this.GetFireTime()) {
            this.onComplete?.Invoke();
            if (this.isLooped != null && this.isLooped.Invoke()) {
                this.startTime = this.GetCurrentTime();
            }
            else {
                this.isCompleted = true;
            }
        }
    }
}
